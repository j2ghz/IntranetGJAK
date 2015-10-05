/*
 * jQuery File Upload Plugin JS Example 8.9.1
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/* global $, window */

$(function () {
    'use strict';

    // In this example we use a custom Generic Handler
    // We send an objectCOntext parameter which can be a user id. The server side component is configured 
    // to accept requests only if this parameter is send with the request (see Web.Backload.config)
    var url = '/Other/Handler/FileHandler.ashx?objectContext=C5F260DD3787';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        url: url,
        acceptFileTypes: /(jpg)|(jpeg)|(png)|(gif)$/i              // Allowed file types
    })


    // Load existing files:
    $('#fileupload').addClass('fileupload-processing');
    $.ajax({
        // Uncomment the following to send cross-domain cookies:
        // xhrFields: {withCredentials: true},
        url: url,
        dataType: 'json',
        context: $('#fileupload')[0]
    }).always(function () {
        $(this).removeClass('fileupload-processing');
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, $.Event('done'), { result: result });
    });
});
