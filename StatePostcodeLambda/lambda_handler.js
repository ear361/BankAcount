'use strict';

exports.handler = async (event) => {
    let state = '';
    let postcode = '';

    if (event) {
        console.log("request: " + JSON.stringify(event));
        if (event.state)
            state = event.state;
        if (event.post_code)
            postcode = event.post_code;
    }

    let responseBody = {
        input: event
    };

    let responseCode = 200;
    if(!validate(state, postcode)){
        responseCode = 403;
    }

    let response = {
        statusCode: responseCode,
        body: JSON.stringify(responseBody)
    };
    return response;
};

function validate(state, postcode) {
    let result = true;
    let regex;

    switch (state) {
        case "Please Select":
            return false;
        case "VIC":
            regex = new RegExp(/^(3|8)\d+/);
            break;
        case "NSW":
            regex = new RegExp(/^(1|2)\d+/);
            break;
        case "QLD":
            regex = new RegExp(/^(4|9)\d+/);
            break;
        case "NT":
            regex = new RegExp(/^0\d+/);
            break;
        case "WA":
            regex = new RegExp(/^6\d+/);
            break;
        case "SA":
            regex = new RegExp(/^5\d+/);
            break;
        case "TAS":
            regex = new RegExp(/^7\d+/);
            break;
        case "ACT":
            regex = new RegExp(/^0\d+/);
            break;
    }
    if(!postcode.match(regex)){
        result = false;
    }
    return result;
}
