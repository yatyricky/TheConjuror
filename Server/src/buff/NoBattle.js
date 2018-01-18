const BuffTypes = require('../constants/BuffTypes');

let guid = 0;

class NoBattle {

    constructor(player) {
        this.iconPath = 'no_battle';
        this.type = BuffTypes.NO_BATTLE;
        this.caster = player;
        this.guid = guid++;
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