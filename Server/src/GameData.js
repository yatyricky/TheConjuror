const xlsx = require('node-xlsx');

const ws = xlsx.parse(`${__dirname}/../assets/data.xlsx`);
const cards = {};
const decks = {};

for (let i = 0; i < ws.length; i++) {
    const element = ws[i];
    if (element.name == "Card") {
        // Card sheet
        const idIndex = 0;
        let nameIndex = -1;
        let colorIndex = -1;
        let ctypeIndex = -1;
        let costIndex = -1;
        let powerIndex = -1;
        let descIndex = -1;
        for (let j = 0; j < element.data[0].length; j++) {
            const item = element.data[0][j];
            if (item == "name") {
                nameIndex = j;
            }
            if (item == "color") {
                colorIndex = j;
            }
            if (item == "type") {
                ctypeIndex = j;
            }
            if (item == "cost") {
                costIndex = j;
            }
            if (item == "power") {
                powerIndex = j;
            }
            if (item == "description") {
                descIndex = j;
            }
        }
        let j = 1;
        while (j < element.data.length && element.data[j].length > 1) {
            cards[element.data[j][idIndex]] = {
                name: element.data[j].length > nameIndex ? element.data[j][nameIndex] : "",
                color: element.data[j].length > colorIndex ? element.data[j][colorIndex] : "",
                ctype: element.data[j].length > ctypeIndex ? element.data[j][ctypeIndex] : "",
                cost: element.data[j].length > costIndex ? element.data[j][costIndex] : "",
                description: element.data[j].length > descIndex ? element.data[j][descIndex] : "",
                power: element.data[j].length > powerIndex ? element.data[j][powerIndex] : ""
            };
            j ++;
        }
    } else if (element.name == "Decks") {
        // Decks sheet
        let numIndex = -1;
        const idIndex = 0;
        for (let j = 0; j < element.data[0].length; j++) {
            const item = element.data[0][j];
            if (item == "nums") {
                numIndex = j;
            }
        }
        if (numIndex == -1) {
            console.error("[E]Parse deck prototype data error");
        } else {
            let deck = null;
            for (let j = 0; j < element.data.length; j++) {
                const item = element.data[j];
                if (typeof item[idIndex] == "number") {
                    deck.push({
                        id: item[idIndex],
                        num: item[numIndex]
                    });
                } else {
                    deck = [];
                    decks[item[idIndex]] = deck;
                }
            }
        }
    }
}

const GameData = {
    cardBase: cards,
    deckBase: decks
};

module.exports = GameData;