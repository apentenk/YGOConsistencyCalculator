var MainDeck = []; //Array for holding cards currently in the main deck
var ExtraDeck = [];//Array for holding cards currently in the main deck
var DeckCombo; //Combo Schema object for holding data about current combo pieces
var cardId = {}; //Used for storing combo id values
var Combos;
const APIURL = "https://localhost:44386/api/"

//List of cards types that belong in the extra Deck
const ExtraDeckCards = ["xyz", "synchro", "fusion", "link", "fusion_pendulum", "xyz_pendulum", "synchro_pendulum"]
function LoadDeck(id) {
    getDeck(id);
}

function createEventListeners(DeckId, UserId) {
    /*$(document).on("mouseenter", ".card-container", function () {
        cardHover(this); //display larger card image on hover
    });*/

    $(document).on("click", "#main-deck>.deck-card", function () {
        mainDeckCardButtonLeftClick(this); //assign card combo value
    });

    $(document).on("contextmenu", ".deck-card", function () {
        mainDeckCardButtonRightClick(this); //remove card from main deck
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
    });
}

//Checks which combo category the user has chosen and assigns it to the clicked card
function mainDeckCardButtonLeftClick(element) {
    if ($("input[type=radio]:checked").length === 1) {
        let comboPieceCategory = $("input[type=radio]:checked").val();
        let result = 2;
        let queryString = '.deck-card[data-id="' + element.getAttribute("data-id") + '"]';
        if (ComboPieces.includes(comboPieceCategory) && element.parentNode.id === "main-deck") {
            result = DeckCombo.updateValue(comboPieceCategory, parseInt(element.getAttribute("data-id")), $(queryString).length);
        }
        let newQuery = queryString + ">.combo-category";
        if (result === 0) {
            $(newQuery).addClass(comboPieceCategory);
            $(newQuery).text(comboPieceCategory);
            updateChances();
        }
        else if (result === 1) {
            $(newQuery).removeClass(comboPieceCategory);
            $(newQuery).text("");
            updateChances();
        }
    }
}

//Removes the clicked card from the deck, and from the combo
function mainDeckCardButtonRightClick(element) {
    let cardId = element.getAttribute("data-id");
    let card = {
        CardId: parseInt(cardId),
        Category: DeckCombo.getCategory(cardId)
    }
    element.remove();
    DeckCombo.removeCard(card);
    updateChances();
}

//Adds the card to the main or extra deck when clicked
function searchResultCardButtonLeftClick(element) {
    let card = {
        CardId: parseInt(element.getAttribute("data-id")),
        CardNumber: element.getAttribute("data-Number"),
    }
    let cardNumber = element.getAttribute("data-Number");
    if (DeckCombo.canAddCard(card.CardId)) {
        if (element.classList.contains("extra-deck")) {
            card.Category = "Extra";
            DeckCombo.addCard(card);
            let cardPicture = createCardContainer(card.CardId, card.CardNumber, 70);
            cardPicture.classList.add("deck-card");
            document.getElementById("extra-deck").appendChild(cardPicture);
        } else {
            card.Category = "Engine";
            DeckCombo.addCard(card);
            let cardPicture = createCardContainer(card.CardId, card.CardNumber, 90);
            cardPicture.classList.add("deck-card");
            document.getElementById("main-deck").appendChild(cardPicture);
            updateChances();
        }
    }
}

/*Displays larger scale of card when hovered
function cardHover(element) {
    let container = document.getElementById("card-view");
    if (container.firstElementChild) {
        if (container.firstElementChild.getAttribute("data-number") !== element.getAttribute("data-number")) {
            container.replaceChildren();
        }
        else {
            return;
        }
    }
    let bigCard = document.createElement("img");
    bigCard.src = "https://localhost:44386/Images/CardPics/" + element.getAttribute("data-number") + ".jpg";
    bigCard.classList.add('bigCard');
    bigCard.setAttribute("data-number", element.getAttribute("data-number"))
    container.appendChild(bigCard);
}*/

//Gets the combo pieces for the deck and assigns them their values
function getComboSchema(id) {
    let URL = APIURL + "ComboData/GetCombosInDeck/" + id;
    var xhr = new XMLHttpRequest();

    xhr.open("GET", URL, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            Combos = (JSON.parse(xhr.responseText));
            DeckCombo = new ComboSchema();
            newDeck = true;
            if (Combos.length > 0) {
                for (key in Combos) {
                    let card = Combos[key];
                    if (card.Category === "Extra") {
                        let cardPicture = createCardContainer(card.CardId, card.CardNumber, 70);
                        cardPicture.classList.add('deck-card');
                        document.getElementById("extra-deck").appendChild(cardPicture);
                    } else {
                        let cardPicture = createCardContainer(card.CardId, card.CardNumber, 90, card.Category);
                        cardPicture.classList.add('deck-card');
                        document.getElementById("main-deck").appendChild(cardPicture);
                    }
                    DeckCombo.addCard(card);
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
        DeckName: document.getElementById("deck-name").value,
        DeckList: "",
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
            UpdateCombos(DeckId);
        }
    }
    xhr.send(JSON.stringify(deck));
}

//Updates the Combo pieces of the deck in the database
function UpdateCombos(id) {
    let fullDeck = DeckCombo.getDeckWithCombo();
    let ComboPieces = [];
    for (let i = 0; i < fullDeck.length; i++) {
        let comboPiece = {
            CardId: fullDeck[i].CardId,
            DeckId: id,
            Category: fullDeck[i].Category
        }
        ComboPieces.push(comboPiece);
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
            window.location.replace("../MyDecks");
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
                            addSearchPictures(card.Id, card.Number, card.CardFrame);
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
function addSearchPictures(id, number, cardFrame) {
    let cardContainer = createCardContainer(id, number, 80);
    let containerName = "search-results";
    if (ExtraDeckCards.indexOf(cardFrame) >= 0) {
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
        let cardContainer = createCardContainer(cards[i].CardId, cards[i].CardNumber);
        cardContainer.classList.add("main-deck-card");
        
        container.appendChild(cardContainer);
    }
}

//Creates a container to hold card images
function createCardContainer(id, number, width, cardCategory) {
    let cardContainer = document.createElement("div");
    cardContainer.classList.add("card-container");
    cardContainer.classList.add("col");
    cardContainer.classList.add("m-1");
    let cardImage = document.createElement("img");
    let imagesource = "/Content/Images/CardPics/" + number + ".jpg";
    cardImage.src = imagesource;
    cardImage.width = width;
    let category = document.createElement("div");
    category.classList.add("combo-category");
    if (cardCategory !== undefined && cardCategory !== "Engine"){
        category.classList.add(cardCategory);
        category.innerText = cardCategory;
    }
    cardContainer.appendChild(cardImage);
    cardContainer.appendChild(category);
    cardContainer.setAttribute("data-id", id);
    cardContainer.setAttribute("data-number", number);
    return cardContainer;
}