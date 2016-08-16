/*eslint-env es6*/
"use strict";

/*
Change authenticationGuid to the brandbank Guid or create a file called auth.js that looks like 
module.exports = {
    guid: '{GUID}'
}
*/
const authenticationGuid = require('./auth.js').guid;

const _ = require('lodash');
const fs = require('fs');
const jsonfile = require('jsonfile');
const request = require('request');
const soap = require('soap');

const nameSpace = 'http://www.i-label.net/Partners/WebServices/DataFeed/2005/11';
const soapHeader = { 'ExternalCallerHeader': { 'ExternalCallerId': authenticationGuid } };
const url = 'https://api.brandbank.com/svc/feed/extractdata.asmx?WSDL';
const basePath = __dirname + '/../_data'

const allProducts = jsonfile.readFileSync(basePath + '/Unilever Products for REHACK.json');

checkDirectorySync(basePath + '/Images');
checkDirectorySync(basePath + '/Products');

soap.createClient(url, (err, client) => {
    client.addSoapHeader(soapHeader, 'name', 'tns', nameSpace);
    let take = 50;
    for (let skipCount = 0; skipCount <= allProducts.length; skipCount += take) {
        let products = _.take(_.drop(allProducts, skipCount), take);
        let args = { 'gtins': { 'gtin': getGtins(products) } };
        client.GetProductDataForGTINs(args, saveData);
    };
});

function getGtins(products) {
    return _.map(products, (product) => {
        return product.gtin;
    });
}

function checkDirectorySync(directory) {
    try {
        fs.statSync(directory);
    } catch (e) {
        fs.mkdirSync(directory);
    }
}

function saveData(err, result) {
    if (err) {
        console.log(err);
        return;
    }
    _.forEach(result.GetProductDataForGTINsResult.Message.Product, (product) => {
        let productCode = product.Identity.ProductCodes.Code[0].$value;
        let fileName = productCode + ' - ' + product.Identity.DiagnosticDescription.$value;
        saveImages(product, productCode);
        jsonfile.writeFileSync(basePath + '/products/' + fileName + '.json', product, { spaces: 2 });
    });
}

function saveImages(product, productCode) {
    let images = getImageAssets(product);
    _.forEach(images, (image, index) => {
        saveImage(image, productCode);
    })
}

function saveImage(image, productCode) {
    request
        .get(image.Url)
        .pipe(fs.createWriteStream(basePath + '/images/' + productCode + '-T' + image.attributes.ShotTypeId + '.jpg'));
}

function getImageAssets(product) {
    let image = product.Assets.Image;
    return _.isArray(image) ? image : [image];
}
