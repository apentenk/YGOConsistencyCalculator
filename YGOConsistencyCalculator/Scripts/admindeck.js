const APIURL = "https://localhost:44386/api/";
function getCardsInDeck(id) {
    let URL = APIURL + "ComboData/GetCombosInDeck/" + id;
    var xhr = new XMLHttpRequest();

    xhr.open("GET", URL, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            Combos = (JSON.parse(xhr.responseText));
            if (Combos.length > 0) {
                for (key in Combos) {
                    let comboPiece = Combos[key];
                    let row = document.createElement("tr");
                    let id = document.createElement("th");
                    id.scope = "row";
                    let idAnchor = document.createElement("a");
                    idAnchor.href = "/ComboPiece/Details/" + comboPiece.ComboPieceId;
                    idAnchor.innerText = comboPiece.ComboPieceId;
                    id.appendChild(idAnchor);
                    let cardId = document.createElement("td");
                    cardId.innerText = comboPiece.CardId;
                    let cardName = document.createElement("td");
                    cardName.innerText = comboPiece.CardName;
                    let category = document.createElement("td");
                    category.innerText = comboPiece.Category;

                    row.appendChild(id);
                    row.appendChild(cardId);
                    row.appendChild(cardName);
                    row.appendChild(category);

                    document.getElementById("table-data").appendChild(row);
                }
            }
        }
    }
    xhr.send();
}

function getDecksForCards(id) {
    let URL = APIURL + "ComboData/GetCombosForCard/" + id;
    var xhr = new XMLHttpRequest();

    xhr.open("GET", URL, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            Combos = (JSON.parse(xhr.responseText));
            if (Combos.length > 0) {
                for (key in Combos) {
                    let comboPiece = Combos[key];
                    let row = document.createElement("tr");
                    let id = document.createElement("th");
                    id.scope = "row";
                    let idAnchor = document.createElement("a");
                    idAnchor.href = "/ComboPiece/Details/" + comboPiece.ComboPieceId;
                    idAnchor.innerText = comboPiece.ComboPieceId;
                    id.appendChild(idAnchor);
                    let deckId = document.createElement("td");
                    deckId.innerText = comboPiece.DeckId;
                    let deckName = document.createElement("td");
                    deckName.innerText = comboPiece.DeckName;
                    let UserId = document.createElement("td");
                    UserId.innerText = comboPiece.UserId;
                    let category = document.createElement("td");
                    category.innerText = comboPiece.Category;

                    row.appendChild(id);
                    row.appendChild(deckId);
                    row.appendChild(deckName);
                    row.appendChild(UserId);
                    row.appendChild(category);

                    document.getElementById("table-data").appendChild(row);
                }
            }
        }
    }
    xhr.send();
}