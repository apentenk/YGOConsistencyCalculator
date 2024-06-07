const ComboPieces = ["OneCard", "TwoCardA", "TwoCardB", "Extenders", "Bricks", "NonEngine"];
class ComboSchema {
    constructor(deck) {
        this.deck = deck;
        this.OneCard = [];
        this.TwoCardA = [];
        this.TwoCardB = [];
        this.Extenders = [];
        this.Bricks = [];
        this.NonEngine = [];
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
        else {
            return "EngineReq";
        }
    }

    hasCard(property, value) {
        return (this[property].includes(value));
    }

    removeCard(property, value) {
        let index = this[property].indexOf(value);
        this[property].splice(index, 1);
    }

    isEmpty() {
        return (this.TwoCardA.length === 0 && this.OneCard.length === 0);
    }

    updateValue(property, value) {
        let category = this.getCategory(value);
        if (category === property) {
            this.removeCard(property, value);
            return 1;
        }
        else if (category === "EngineReq") {
            this[property].push(value);
            return 0;
        }
        else {
            return 2;
        }
    }

    getHand(...args) {
        let hand = [];
        for (var i = 0; i < args.length; i++) {
            hand.push(this.getCounts(args[i], this.deck));
        }
        return hand;
    }

    getCopies(cards, value) {
        return this.deck.filter(x => cards.includes(value) && x === value).length
    }

    getCounts(cards) {
        let count = 0;
        for (let i = 0; i < cards.length; i++) {
            count += this.getCopies(cards, cards[i])
        }
        return count;
    }

    setDeck(newDeck) {
        this.deck = newDeck.slice();
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
