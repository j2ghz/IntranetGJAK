/*jslint unparam: true */
/*global window, $ */
$(function () {
    'use strict';


    var uploader = new qq.FineUploader({
        element: document.getElementById("fine-uploader"),
        template: 'qq-template',
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

        chunking: {
            enabled: true,                                                                  // true to enable file chunking
            partSize: 10000000                                                              // 10MB chunks (usually to small, but this is a demo)
        },

        validation: {
            allowedExtensions: ['jpeg', 'jpg', 'gif', 'png', 'pdf']
        },

        callbacks: {
            onComplete: function (id, name, response, xhr) {                                // Within normal file uploads the uuid provided
                if ((response) && (response.success)) {                                     // by the server is automatically applied to the file
                    var uuid = uploader.getUuid(id);                                        // by Fine Uploader. In file chunk mode this must be 
                    if (uuid != response.newUuid)                                           // done by hand. If we don't do this, the server side
                        uploader.setUuid(id, response.newUuid);                             // component cannot associate the file on delete requests.
                }
            }
        }
    });
});
