module.exports = function (grunt) {
    "use strict";

    var deploy = require('deploy-azure-cdn');
    var path = require('path'); 

    grunt.registerMultiTask('azure-cdn-deploy', 'Copy files to azure storage blob', function () {
        var files = [];
        this.files.forEach(function(file) {
            files = files.concat(file.src.map(function(src) {
                return {
                    path: src,
                    dest: file.dest
                }
            }));
        });
        var globalAsync = this.async();
        deploy(this.options(), files, grunt.log.debug, function (err) {
            if(err){
                grunt.log.error("Error while copying to azure " + err);
                globalAsync(false);
                return;
            }
            globalAsync();
        }); 
    });
    
};