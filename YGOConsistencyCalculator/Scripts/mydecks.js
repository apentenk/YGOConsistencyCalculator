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
            let id = JSON.parse(xhr.responseText);
            window.location.replace(("EditDeck/" + id));
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
            if (decks.length > 0) {
                for (key in decks) {
                    let deck = decks[key];
                    let values = [];
                    values.push(deck.DeckName);
                    values.push(deck.StarterChance);
                    values.push(deck.OneCardChance);
                    values.push(deck.TwoCardChance);
                    values.push(deck.ExtenderChance);
                    values.push(deck.EngineReqChance);
                    let container = document.createElement("section");
                    container.classList.add("col");
                    container.appendChild(showDeck(deck.DeckId, values));
                    deckListcontainer.appendChild(container);
                }
            }
            if (decks.length < 3) {
                console.log("here");
                for (i = 0; i < (3 - decks.length); i++) {
                    let container = document.createElement("section");
                    container.classList.add("col");
                    let newDeck = document.createElement("button");
                    newDeck.textContent = "New Deck";
                    newDeck.addEventListener('click', function () {
                        createDeck(id);
                    });
                    container.appendChild(newDeck);
                    deckListcontainer.appendChild(container);
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