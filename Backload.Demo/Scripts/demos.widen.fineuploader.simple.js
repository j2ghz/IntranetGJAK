/*jslint unparam: true */
/*global window, $ */
$(function () {
    'use strict';

    // We use the upload handler integrated into Backload (endpoint parameter for each method):
    // In this example we set an objectContect (id) in the form parameter in request.params (also possible: url query parameter ).
    // You can use a user id as objectContext give users only access to their own uploads.

    var fine = '/Backload/Client/widen/fineuploader/';

    var uploader = new qq.FineUploader({
        element: document.getElementById("fine-uploader"),
        template: 'qq-simple-thumbnails-template',
        request: {
            endpoint: '/Backload/FileHandler',
            params: {                                                                       // Send a plugin param or set Fine Uploader in 
                plugin: "FineUploader",                                                     // Web.Backload.config as the default client plugin                                                    
                objectContext: "030357B624D9"
            }
        },

        deleteFile: {
            enabled: true,
            endpoint: '/Backload/FileHandler',
            params: {                                                                       // Send a plugin param or set Fine Uploader in 
                plugin: "FineUploader",                                                     // Web.Backload.config as the default client plugin                                                    
                objectContext: "030357B624D9"
            }
        },

        session: {                                                                          // Initial GET request to load existing files
            endpoint: '/Backload/FileHandler',
            params: {                                                                       // Send a plugin param or set Fine Uploader in 
                plugin: "FineUploader",                                                     // Web.Backload.config as the default client plugin                                                      
                objectContext: "030357B624D9"
            }
        },

        thumbnails: {
            placeholders: {
                waitingPath: fine + 'placeholders/waiting-generic.png',
                notAvailablePath: fine + 'placeholders/not_available-generic.png'
            }
        },

        chunking: {
            enabled: true,                                                                  // true to enable file chunking
            partSize: 10000000                                                              // 10MB chunks (usually to small, but this is a demo)
        },

        validation: {
            allowedExtensions: ['jpeg', 'jpg', 'gif', 'png', 'pdf']
        },

        callbacks: {
            onComplete: function (id, name, response, xhr) {                                 // Within normal file uploads the uuid provided
                if ((response) && (response.success)) {                                     // by the server is automatically applied to the file
                    var uuid = uploader.getUuid(id);                                 // by Fine Uploader. In file chunk mode this must be 
                    if (uuid != response.newUuid)                                           // done by hand. If we don't do this, the server side
                        uploader.setUuid(id, response.newUuid);                      // component cannot associate the file on delete requests.
                }
            }
        }
    });
});