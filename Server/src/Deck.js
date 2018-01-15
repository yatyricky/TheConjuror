class Deck {

    constructor() {
        this.cards = [];
    }

    drawRandom() {
        if (this.cards.length > 0) {
            const index = Math.floor(Math.random() * this.cards.length);
            let ret = this.cards[index];
            this.cards.splice(index, 1);
            return ret;
        } else {
            console.error("[E]Drawing card from empty deck");
            return null;
        }
    }

    size() {
        return this.cards.length;
    }

    add(card) {
        this.cards.push(card);
    }
}

module.exports = Deck;