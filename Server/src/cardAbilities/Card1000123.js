const Events = require('../constants/Events');

class Card1000123 {

    constructor(card) {
        this.card = card;
        this.effectCallback = this.doEffect.bind(this);
    }

    doEffect() {

    }

    doAction(player) {
        player.pushSelectAction(this.effectCallback);
        return {
            ename: Events.SELECT_TARGET,
            payload: {
                name: player.getName()
            }
        };
    }

}

module.exports = Card1000123;