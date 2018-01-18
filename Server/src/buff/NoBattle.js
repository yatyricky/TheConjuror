const BuffTypes = require('../constants/BuffTypes');
const {NewBuffGuid} = require('../utils/SNGenerator');

class NoBattle {

    constructor(player) {
        this.iconPath = 'no_battle';
        this.type = BuffTypes.NO_BATTLE;
        this.caster = player;
        this.guid = NewBuffGuid();
    }

    onApply(card) {
        let attr = card.getAttrs();
        if (!attr.hasOwnProperty(BuffTypes.NoBattle)) {
            attr[BuffTypes.NoBattle] = 0;
        }
        attr[BuffTypes.NoBattle] += 1;
    }

    onRemove(card) {
        let attr = card.getAttrs();
        if (!attr.hasOwnProperty(BuffTypes.NoBattle)) {
            console.error(`[E]NoBattle.onRemove: card:${JSON.stringify(card)} doesnt have NoBattle buff`);
        }
        attr[BuffTypes.NoBattle] -= 1;
        if (attr[BuffTypes.NoBattle] < 0) {
            attr[BuffTypes.NoBattle] = 0;
            console.error(`[E]NoBattle.onRemove: card:${JSON.stringify(card)} NoBattle value below 0`);
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

module.exports = NoBattle;