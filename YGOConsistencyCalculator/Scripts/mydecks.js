//Creating a blank new deck and redirecting to editing it
function createDeck(id) {
    const URL = "https://localhost:44386/api/DeckData/CreateDeck/";
    var xhr = new XMLHttpRequest();

    var deck = {
        UserId: id,
        DeckName: "My Deck"
    }

    xhr.open("POST", URL, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            let deckid = JSON.parse(xhr.responseText);
            window.location.replace(("EditDeck/" + deckid));
        }
    }
    xhr.send(JSON.stringify(deck));
}

//Geting the decks and their information for decks belonging to a certain user
function getDecks(id) {
    const URL = "https://localhost:44386/api/DeckData/UserDecks/" + id;
    var xhr = new XMLHttpRequest();

    xhr.open("GET", URL, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            let decks = (JSON.parse(xhr.responseText));
            const deckListcontainer = document.getElementById("decklists");
            for (let i = 0; i < decks.length; i++) {
                let deck = decks[i];
                let query = "#deck-" + (i + 1);
                $(query).off();
                $(query + ">.card-title>h2").text(deck.DeckName);
                $(query + ">.card-effect").append(createEffectStat("Combo Chance:", deck.StarterChance))
                $(query + ">.card-effect").append(createEffectStat("One-Card Combo Chance:", deck.OneCardChance))
                $(query + ">.card-effect").append(createEffectStat("Two-Card Combo Chance:", deck.TwoCardChance))
                $(query + ">.card-effect").append(createEffectStat("Combo/Extender Chance:", deck.ExtenderChance))
                $(query + ">.card-effect").append(createEffectStat("Combo/NonEngine Chance:", deck.EngineReqChance))

                let navContainer = document.createElement('div');
                navContainer.classList.add("card-stats");
                navContainer.append(createCardLink("Edit Deck", "/Deck/EditDeck/" + deck.DeckId));
                navContainer.append(createCardLink("Delete Deck", "/Deck/DeleteConfirm/" + deck.DeckId));
                $(query + ">.card-effect").append(navContainer);
                fillDeckPreview($(query+">.card-img"),deck.DeckId);
            }

        }
    }
    xhr.send();
}

function createEffectStat(statName, statValue) {
    let container = document.createElement("div");
    container.classList.add("effect-stat");
    let name = document.createElement('p');
    name.innerText = statName;
    let value = document.createElement('p');
    value.innerText = statValue + "%";
    container.appendChild(name);
    container.appendChild(value);
    return container;
}

function createCardLink(linkName, url) {
    let anchor = document.createElement('a');
    anchor.href = url;
    anchor.innerText = linkName;
    return anchor
}

function fillDeckPreview(container, DeckId) {
    const URL = "https://localhost:44386/api/ComboData/GetCombosInDeck/" + DeckId;
    var xhr = new XMLHttpRequest();

    xhr.open("GET", URL, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            let Combos = (JSON.parse(xhr.responseText));
            if (Combos.length > 0) {
                for (key in Combos) {
                    let card = Combos[key];
                    let cardPicture = document.createElement("img");
                    cardPicture.src = "/Content/Images/CardPics/" + card.CardNumber + ".jpg";
                    cardPicture.width = "30";
                    if (card.Category !== "Extra") {
                        container.append(cardPicture);
                    }
                }
            }
        }
    }

    xhr.send();
}
//Deletes a user's deck
//Will be used to delete the deck once I add confirmation without redirecting to another page
function deleteDeck(id) {
    var URL = "https://localhost:44386/api/DeckData/DeleteDeck/" + id;

    var xhr = new XMLHttpRequest();

    //Generating DELETE request
    xhr.open("POST", URL, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            window.location.replace("/Deck/MyDecks");
        }
    }

    //Sending request
    xhr.send();
}

//Creates DOM elements for viewing Deck info
function showDeck(id, values) {
    const container = document.createElement("div");

    const nameContainer = document.createElement("div");
    nameContainer.classList.add("row");
    const deckName = document.createElement("h2");
    deckName.innerHTML = values[0];
    nameContainer.appendChild(deckName);
    container.appendChild(nameContainer);


    container.appendChild(createStat("Combo Chance:", values[1]));
    container.appendChild(createStat("One-Card Combo Chance:", values[2]));
    container.appendChild(createStat("Two-Card Combo Chance:", values[3]));
    container.appendChild(createStat("Combo and Extender Chance", values[4]));
    container.appendChild(createStat("Combo and NonEngine Chance:", values[5]));

    const edit = document.createElement("a");
    edit.href = "/Deck/EditDeck/" + id;
    edit.innerHTML = "Edit Deck";

    const deleteDeck = document.createElement("a");
    deleteDeck.href = "/Deck/DeleteConfirm/" + id;
    deleteDeck.innerHTML = "Delete Deck";

    container.appendChild(edit);
    container.appendChild(deleteDeck);
    return (container);
}

//Creates DOM elements for viewing stats
function createStat(statText, value) {
    const statKey = document.createElement("h5");
    statKey.classList.add("col-9");
    statKey.innerHTML = statText;
    const statValue = document.createElement("h5");
    statValue.classList.add("col");
    statValue.innerHTML = value;
    const container = document.createElement("div");
    container.classList.add("row");
    container.appendChild(statKey);
    container.appendChild(statValue);
    return container;
}