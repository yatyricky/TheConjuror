class Card1000023 {

    constructor(card) {
        this.card = card;
    }

    doAction(player, callBack) {
    }

    getBattleModifier(isAttacker) {
        if (isAttacker) {
            return 0 - this.card.getPower();
        } else {
            return 0;
        }
    }

}

module.exports = Card1000023;