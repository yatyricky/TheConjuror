let buffGuid = 0;

const NewBuffGuid = function() {
    buffGuid++;
    return buffGuid - 1;
};

module.exports = {
    NewBuffGuid
};