const ComboPieces = ["OneCard", "TwoCardA", "TwoCardB", "Extenders", "Bricks", "NonEngine"];
class ComboSchema {
    constructor() {
        this.deck = [];
        this.OneCard = [];
        this.TwoCardA = [];
        this.TwoCardB = [];
        this.Extenders = [];
        this.Bricks = [];
        this.NonEngine = [];
        this.Engine = [];
        this.Extra = [];
    }

    canAddCard(id) {
        return this.deck.filter(x => x === id).length < 3;
    }

    addCard(card) {
        this.deck.push(card.CardId);
        if (this[card.Category].includes(card.CardId)) {
            return;
        }
        this[card.Category].push(card.CardId);
    }

    removeCard(card) {
        if (this.deck.includes(card.CardId)) {
            let index = this.deck.indexOf(card.CardId);
            this.deck.splice(index, 1);
            if (!this.deck.includes(card.CardId)) {
                index = this[card.Category].indexOf(card.cardId);
                this[card.Category].splice(index, 1);
            }
        }
    }

    removeFromCategory(category, value) {
        let index = this[category].indexOf(value);
        this[category].splice(index, 1);
    }

    updateValue(property, value) {
        let category = this.getCategory(value);
        if (category === property) {
            this.removeFromCategory(property, value);
            return 1;
        }
        else if (category === "Engine") {
            this[property].push(value);
            return 0;
        }
        else {
            return 2;
        }
    }

    getDeck() {
        let deckCopy = [];
        for (let i = 0; i < this.deck.length; i++) {
            let cardCopy = {
                CardId: this.deck[i].CardId,
                Category: (this.deck[i]).Category
            }
            deckCopy.push(cardCopy);
        }
        return deckCopy;
    }

    getDeckWithCombo() {
        let deckCopy = [];
        for (let i = 0; i < this.deck.length; i++) {
            let cardId = this.deck[i];
            let cardCopy = {
                CardId: cardId,
                Category: this.getCategory(cardId)
            }
            deckCopy.push(cardCopy);
        }
        return deckCopy;

    }

    isIncluded(value) {
        return (this.OneCard.includes(value) || this.TwoCardA.includes(value) || this.TwoCardB.includes(value) || this.Extenders.includes(value) || this.Bricks.includes(value) || this.NonEngine.includes(value));
    }

    getCategory(value) {
        if (this.OneCard.includes(value)) {
            return "OneCard";
        }
        else if (this.TwoCardA.includes(value)) {
            return "TwoCardA";
        }
        else if (this.TwoCardB.includes(value)) {
            return "TwoCardB";
        }
        else if (this.Extenders.includes(value)) {
            return "Extenders";
        }
        else if (this.Bricks.includes(value)) {
            return "Bricks";
        }
        else if (this.NonEngine.includes(value)) {
            return "NonEngine";
        }
        else if (this.Extra.includes(value)) {
            return "Extra";
        }
        else {
            return "Engine";
        }
    }

    hasCard(property, value) {
        return (this[property].includes(value));
    }


    isEmpty() {
        return (this.TwoCardA.length === 0 && this.OneCard.length === 0);
    }


    getHand(...args) {
        let hand = [];
        for (var i = 0; i < args.length; i++) {
            hand.push(this.getCounts(args[i]));
        }
        return hand;
    }

    getCopies(cards, value) {
        return this.deck.filter(card => cards.includes(value) && card === value).length
    }

    getCounts(cards) {
        let count = 0;
        for (let i = 0; i < cards.length; i++) {
            count += this.getCopies(cards, cards[i])
        }
        return count;
    }

    getStarterChance() {
        let handSize = 5;
        let oneTwoCardCombo = calculate(this.getHand(this.TwoCardA, this.TwoCardB, this.OneCard), this.getCounts(this.Bricks), this.deck.length, handSize);
        return (this.getOneCardChance() + this.getTwoCardChance() - oneTwoCardCombo);
    }

    getOneCardChance() {
        let handSize = 5;
        return calculate([this.getCounts(this.OneCard)], this.getCounts(this.Bricks), this.deck.length, handSize);
    }

    getTwoCardChance() {
        let handSize = 5;
        return calculate(this.getHand(this.TwoCardA, this.TwoCardB), this.getCounts(this.Bricks), this.deck.length, handSize);
    }

    getExtenderChance() {
        let handSize = 5;
        let twoCardExtender = calculate(this.getHand(this.TwoCardA, this.TwoCardB, this.Extenders), this.getCounts(this.Bricks), this.deck.length, handSize);
        let oneCardExtender = calculate(this.getHand(this.OneCard, this.Extenders), this.getCounts(this.Bricks), this.deck.length, handSize);
        let oneTwoCardExtender = calculate(this.getHand(this.OneCard, this.TwoCardA, this.TwoCardB, this.Extenders), this.getCounts(this.Bricks), this.deck.length, handSize);
        return (twoCardExtender + oneCardExtender - oneTwoCardExtender);
    }

    getNonEngineChance() {
        let handSize = 5;
        let twoCardNonEngine = calculate(this.getHand(this.TwoCardA, this.TwoCardB, this.NonEngine), this.getCounts(this.Bricks), this.deck.length, handSize);
        let oneCardNonEngine = calculate(this.getHand(this.OneCard, this.NonEngine), this.getCounts(this.Bricks), this.deck.length, handSize);
        let oneTwoCardNonEngine = calculate(this.getHand(this.OneCard, this.TwoCardA, this.TwoCardB, this.NonEngine), this.getCounts(this.Bricks), this.deck.length, handSize);
        return (twoCardNonEngine + oneCardNonEngine - oneTwoCardNonEngine);
    }
}

function calculate(cards, bricks, deckSize, handSize) {
    let otherCards = deckSize - cards.reduce((a, b) => a + b);
    otherCards -= bricks;
    let numerator = recursiveCalculate([], [], bricks, 0, cards, otherCards, deckSize, handSize);
    return (numerator / bionmialCoeffecient(deckSize, handSize)) * 100;
}

function recursiveCalculate(cardsToDraw, maxCopiesDrawn, cardsNotToDraw, numberOfCardsInHand, cards, otherCards, deckSize, maxHandSize) {
    let probability = 0;
    if (cards.length === 0) {
        if (numberOfCardsInHand > maxHandSize) {
            return 0;
        }
        probability = 1;
        for (let i = 0; i < cardsToDraw.length; i += 1) {
            probability *= bionmialCoeffecient(cardsToDraw[i], maxCopiesDrawn[i]);
        }

        if (cardsNotToDraw > 0) {
            probability *= (bionmialCoeffecient(cardsNotToDraw, 0));
        }

        if (numberOfCardsInHand < maxHandSize) {
            probability *= bionmialCoeffecient(otherCards, maxHandSize - numberOfCardsInHand);
        } 
        return probability;
    }

    let cardToDraw = cards.pop();

    for (let i = 1; i <= cardToDraw; i++) {

        cardsToDraw.push(cardToDraw);
        maxCopiesDrawn.push(i);

        probability += recursiveCalculate(cardsToDraw, maxCopiesDrawn, cardsNotToDraw, numberOfCardsInHand + i, cards, otherCards, deckSize, maxHandSize);

        cardsToDraw.pop();
        maxCopiesDrawn.pop();

    }

    cards.push(cardToDraw);

    return probability;
}


function bionmialCoeffecient(n, k) {
    return (factorial(n) / (factorial(k) * factorial(n - k)));
}

function factorial(x) {
    if (x <= 0) {
        return 1;
    }
    else {
        return (x * factorial(Math.abs(x) - 1));
    }
}
