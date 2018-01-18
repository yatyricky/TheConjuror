const BuffTypes = require('../constants/BuffTypes');
const {NewBuffGuid} = require('../utils/SNGenerator');

class Weaken {

    constructor(player) {
        this.iconPath = 'weaken';
        this.type = BuffTypes.WEAKEN;
        this.caster = player;
        this.guid = NewBuffGuid();
        this.mod = 0;
    }

    onApply(card) {
        this.mod =  1 - card.getPower();
        card.modPower(this.mod);
    }

    onRemove(card) {
        if (card.getPower() > 0) {
            card.modPower(0 - this.mod);
        }
    }

    shouldFadeEndTurn() {
        return true;
    }

    getCaster() {
        return this.caster;
    }

    getType() {
        return this.type;
    }

    getData() {
        return {
            guid: this.guid,
            icon: this.iconPath
        };
    }

}

module.exports = Weaken;