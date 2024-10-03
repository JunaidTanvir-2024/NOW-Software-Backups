/**
 * Provides functionalities to other JS modules.
 * 
 * @author micheled
 * Created on 13.07.2017
 * 
 */
'use strict';
var Utils = function () {

    // The vars
    window.args = {
        data: new Object(),
        beforeSend: function () { },
        complete: function () { },
        success: function (result) {
            return result;
        }
    };

    /*******************
     * Private methods *
     ******************/

     /**
	 * Performs a GET request.
     * 
	 * @param {string} Resource to invoke
     * @param {object} Configurable behaviour through the passed data
     * 
	 */
    function doGet(uri, args) {
        
        $.get(uri, function (result) {
            args.success(result);
        });
    };

    /**
	 * Performs an Ajax POST request.
     * 
	 * @param {string} Resource to invoke
     * @param {object} Configurable behaviour through the passed data
     * 
	 */
    function doPost (uri, args) {
        $.post({
            url: uri,
            type: 'POST',
            data: args.data,
            dataType: 'json',
            async: true,
            beforeSend: args.beforeSend(),
            complete: args.complete()
        }, function (result) {
            args.success(result);
        });
    }

    /******************
     * Public methods *
     ******************/

     /**
	 * Exposes a public method for GET requests.
     * 
	 * @param {string} Resource to invoke
     * @param {object} Configurable behaviour through the passed data
     * 
	 */
    this.Get = function (uri, args) {
        
        doGet(uri, args);
    };

    /**
	 * Exposes a public method for POST requests.
     * 
	 * @param {string} Resource to invoke
     * @param {object} Configurable behaviour through the passed data
     * 
	 */
    this.Post = function (uri, args) {
        doPost(uri, args);
    };

    /**
     * Get an HTML file for frontend templates.
     * 
     * @param {string} Full path to the file
     * @return {object} The response object
     * 
     */
    this.fetchHTMLTemplate = function (uri) {
        $.get(uri, function (data) {
            return data;
        });
    };
};
