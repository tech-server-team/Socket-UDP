module.exports = class JsonInit{
    constructor () {
        this.jsonObj = {
            "type" : null,

            "address" : null, 
            "port" : null,
            "uuid" : null,

            ribal : {
                "address" : null, 
                "port" : null,
                "uuid" : null,
            }
        }
    }
    get() {
        return this.jsonObj;
    } 
}