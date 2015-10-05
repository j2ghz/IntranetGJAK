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

    // We use the upload handler integrated into Backload:
    // In this example we set an objectContect (id) in the url query (or as form parameter).
    // You can use a user id as objectContext give users only access to their own uploads.
    var url = '/Backload/FileHandler?objectContext=C5F260DD3787';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        url: url,
        maxChunkSize: 10000000,                                     // Optional: file chunking with 10MB chunks
        acceptFileTypes: /(jpg)|(jpeg)|(png)|(gif)|(pdf)$/i,        // Allowed file types
        disableImageResize: /Android(?!.*Chrome)|Opera/             // Disable browser integrated image resizing
            .test(window.navigator.userAgent)

    })
    .bind('fileuploadsubmit', function (e, data) {
        // Optional: We add a random uuid form parameter. On chunk uploads the uuid is used to store the chunks.
        data.formData = { uuid: Math.random().toString(36).substr(2, 8) };
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
        var up = $(this).fileupload('option', 'done')
        up.call(this, $.Event('done'), { result: result });
    });



    // Initialize the jQuery UI theme switcher:
    $('#theme-switcher').change(function () {
        var theme = $('#theme');
        theme.prop(
            'href',
            theme.prop('href').replace(
                /[\w\-]+\/jquery-ui.css/,
                $(this).val() + '/jquery-ui.css'
            )
        );
    });
});
