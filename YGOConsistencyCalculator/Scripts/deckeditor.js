var MainDeck = []; //Array for holding cards currently in the main deck
var ExtraDeck = [];//Array for holding cards currently in the main deck
var DeckCombo; //Combo Schema object for holding data about current combo pieces
var cardId = {}; //Used for storing combo id values****
const APIURL = "https://localhost:44386/api/"

//List of cards types that belong in the extra Deck
const ExtraDeckCards = ["xyz", "synchro", "fusion", "link", "fusion_pendulum", "xyz_pendulum", "synchro_pendulum"]
function LoadDeck(id) {
    getDeck(id);
}

function createEventListeners(DeckId, UserId) {
    $(document).on("mouseenter", ".card-container", function () {
        cardHover(this); //display larger card image on hover
    });

    $(document).on("click", ".main-deck-card", function () {
        mainDeckCardButtonLeftClick(this); //assign card combo value
    });

    $(document).on("contextmenu", ".main-deck-card", function () {
        mainDeckCardButtonRightClick(this); //remove card from main deck
        return false;
    });

    $(document).on("contextmenu", ".extra-deck-card", function () {
        mainDeckCardButtonRightClick(this); //remove card from extra deck
        return false;
    });

    $(document).on("click", ".search-results-card", function () {
        searchResultCardButtonLeftClick(this); //add card to main deck
    });

    $('#search').on('keyup', function (e) {
        if (e.key === 'Enter') {
            getSearchResults(this); //search when enter is pressed
        }
    });

    $('#save-button').on('click', function () {
        UpdateDeck(DeckId, UserId); //saves deck information
        UpdateCombos(DeckId); //saves combo information
    });
}

//Checks which combo category the user has chosen and assigns it to the clicked card
function mainDeckCardButtonLeftClick(element) {
    if ($("input[type=radio]:checked").length === 1) {
        let comboPieceCategory = $("input[type=radio]:checked").val();
        let result = 2;
        let queryString = '.main-deck-card[data-id="' + element.getAttribute("data-id") + '"]';
        if (ComboPieces.includes(comboPieceCategory) && element.parentNode.id === "main-deck") {
            result = DeckCombo.updateValue(comboPieceCategory, element.getAttribute("data-id"), $(queryString).length);
        }

        if (result === 0) {
            $(queryString).addClass(comboPieceCategory);
            updateChances();
        }
        else if (result === 1) {
            $(queryString).removeClass(comboPieceCategory);
            updateChances();
        }
    }
}

//Removes the clicked card from the deck, and from the combo
function mainDeckCardButtonRightClick(element) {
    let cardNumber = element.getAttribute("data-id");
    element.remove();
    let index = MainDeck.findIndex(id => id === cardNumber);
    if (index > -1) {
        MainDeck.splice(index, 1);
    }
    if (!MainDeck.includes(cardNumber)) {
        category = DeckCombo.getCategory();
        if (category !== "EngineReq") {
            DeckCombo.removeCard(category, value);
        }
    }
    updateChances();
}

//Adds the card to the main or extra deck when clicked
function searchResultCardButtonLeftClick(element) {
    let cardNumber = element.getAttribute("data-id");
    if (element.classList.contains("extra-deck-card")) {
        if (ExtraDeck.length <= 15 && ExtraDeck.filter(x => x === cardNumber).length < 3) {
            addPictures("extra-deck", [cardNumber]);
        }
    } else {
        if (MainDeck.length <= 60 && MainDeck.filter(x => x === cardNumber).length < 3) {
            console.log(cardNumber);
            addPictures("main-deck", [cardNumber]);
            MainDeck.push(cardNumber);
            updateChances();
        }
    }
}

//Displays larger scale of card when hovered
function cardHover(element) {
    let container = document.getElementById("card-view");
    if (container.firstElementChild) {
        if (container.firstElementChild.getAttribute("data-id") !== element.getAttribute("data-id")) {
            container.replaceChildren();
        }
        else {
            return;
        }
    }
    let bigCard = document.createElement("img");
    bigCard.src = "https://localhost:44386/Images/CardPics/" + element.getAttribute("data-id") + ".jpg";
    bigCard.classList.add('bigCard');
    bigCard.setAttribute("data-id", element.getAttribute("data-id"))
    container.appendChild(bigCard);
}

//Gets the combo pieces for the deck and assigns them their values
function getComboSchema(id) {
    let URL = APIURL + "ComboData/GetComboPieces/" + id;
    var xhr = new XMLHttpRequest();

    xhr.open("GET", URL, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            let deckCards = (JSON.parse(xhr.responseText));
                DeckCombo = new ComboSchema(MainDeck);
                newDeck = true;
            if(deckCards.length > 0) {
                for (key in deckCards) {
                    let card = deckCards[key];
                    if (card.Category !== "EngineReq" && MainDeck.indexOf(card.CardNumber.toString() > -1)) {
                        DeckCombo.updateValue(card.Category, card.CardNumber.toString());
                        let queryString = '.main-deck-card[data-id="' + card.CardNumber + '"]';
                        $(queryString).addClass(card.Category);
                    }
                    cardId[card.CardNumber] = card.ComboPieceId;
                }
                newDeck = false;
            }
            updateChances();
        }
    }
    xhr.send();
}

//Updates the deck stats with new values
function updateChances() {
    document.getElementById("comboChance").innerHTML = DeckCombo.getStarterChance().toFixed(2) + "%";
    document.getElementById("oneComboChance").innerHTML = DeckCombo.getOneCardChance().toFixed(2) + "%";
    document.getElementById("twoComboChance").innerHTML = DeckCombo.getTwoCardChance().toFixed(2) + "%";
    document.getElementById("extenderChance").innerHTML = DeckCombo.getExtenderChance().toFixed(2) + "%";
    document.getElementById("nonEngineChance").innerHTML = DeckCombo.getNonEngineChance().toFixed(2) + "%";
}

//Updates the deck in the database with the new decklist, and stats
//Will change the deckname to not be hard coded later
function UpdateDeck(DeckId, UserId) {
    var URL = APIURL + "DeckData/UpdateDeck/" + DeckId;
    let xhr = new XMLHttpRequest(); 

    let deck = {
        DeckId: DeckId,
        UserId: UserId,
        DeckName: "My Deck",
        DeckList: MainDeck.join(" ") + " #extra " +ExtraDeck.join(" "),
        StarterChance: DeckCombo.getStarterChance(),
        OneCardChance: DeckCombo.getOneCardChance(),
        TwoCardChance: DeckCombo.getTwoCardChance(),
        ExtenderChance: DeckCombo.getExtenderChance(),
        EngineReqChance: DeckCombo.getNonEngineChance()
    }

    

    xhr.open("POST", URL, true);
    xhr.setRequestHeader("Content-type", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 204) {
            //window.location.replace("../MyDecks");
        }
    }
    xhr.send(JSON.stringify(deck));
}

//Updates the Combo pieces of the deck in the database
function UpdateCombos(id) {
    let ComboPieces = [];
    let ComboPiecesNumbers = [];
    for (let i = 0; i < MainDeck.length; i++) {
        if (ComboPiecesNumbers.indexOf(MainDeck[i]) < 0) {
            let comboPiece = {
                ComboPieceId: cardId[MainDeck[i]],
                CardNumber: MainDeck[i],
                DeckId: id,
                Category: DeckCombo.getCategory(MainDeck[i])
            }
            ComboPieces.push(comboPiece);
            ComboPiecesNumbers.push(MainDeck[i]);
        }
    }
    updateComboPieces(id, ComboPieces);
}

//Creates the XML request for updating the combo pieces
function updateComboPieces(id, comboPieces) {
    var URL = APIURL + "ComboPieceData/UpdateComboPieces/" + id;
    let xhr = new XMLHttpRequest();

    xhr.open("POST", URL, true);
    xhr.setRequestHeader("Content-type", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 204) {
            //window.location.replace("MyDecks");
        }
    }
    xhr.send(JSON.stringify(comboPieces));
}

//Searches based on text in the search textbox
function getSearchResults(element) {
    if (element.value !== "") {
        let URL = APIURL + "CardData/NameSearch/" + element.value;
        var xhr = new XMLHttpRequest();

        xhr.open("GET", URL, true);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                let response = (JSON.parse(xhr.responseText));
                if (response.length !== 0) {
                    let searchResults = (JSON.parse(xhr.responseText));
                    if (searchResults.length > 0) {
                        $("#search-results").empty();
                        for (key in searchResults) {
                            let card = searchResults[key];
                            addSearchPictures(card.Number, card.CardFrame);
                        }
                    }
                }
            }
        }
        xhr.send();
    }
}

//Splits the stored decklist into the Main and Extra deck
function splitDeck(fullDeck) {
    let deck = fullDeck.split(" #extra ");
    MainDeck = deck[0].split(" ");
    ExtraDeck = deck[1].split(" ");
    addPictures("main-deck", MainDeck);
    addPictures("extra-deck", ExtraDeck);
}

//Creates DOM elements for view Search Results
function addSearchPictures(number, cardFrame) {
    let cardContainer = createCardContainer(number);
    let containerName = "search-results";
    if (ExtraDeckCards.indexOf(cardFrame) > 0) {
        cardContainer.classList.add("extra-deck");
    }
    cardContainer.classList.add(containerName + "-card");
    const container = document.getElementById(containerName);
    container.appendChild(cardContainer);
}

//Creates DOM elements for view the decklist
function addPictures(containerName, cards) {
    const container = document.getElementById(containerName);
    for (let i = 0; i < cards.length; i++) {
        let cardContainer = createCardContainer(cards[i]);
        cardContainer.classList.add("main-deck-card");
        
        container.appendChild(cardContainer);
    }
}

//Creates a container to hold card images
function createCardContainer(card) {
    let cardContainer = document.createElement("div");
    cardContainer.classList.add("card-container");
    cardContainer.classList.add("col");
    cardContainer.classList.add("m-1");
    let cardImage = document.createElement("img");
    cardImage.src = "https://localhost:44386//Images/CardPics/" + card + ".jpg";
    cardImage.width = 76;
    cardContainer.appendChild(cardImage);
    cardContainer.setAttribute("data-id", card);
    return cardContainer;
}