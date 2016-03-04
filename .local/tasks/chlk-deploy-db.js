var request = require('request');

module.exports = function (grunt) {
    var compute = require('azure-mgmt-compute');

    function required(options, prop) {
        if (!options[prop])
            throw Error('Option ' + prop + ' is required');
    }

    grunt.registerTask('chlk-deploy-db', 'Deploy Chalkable Classroom Database', function() {
        var done = this.async();

        var options = this.options();
        required(options, 'sysAdminToken');
        required(options, 'serverUrl');

        var sysAdminToken = options.sysAdminToken,
            serverUrl = options.serverUrl;

        request.post(serverUrl + '/DbMaintenance/DatabaseDeployCI', {form: {key: sysAdminToken}},
            function (err, httpResponse, body) {
                if (err) {
                    grunt.log.error('Deployment failed: ' + err.message);
                    done(false);
                    return;
                }

                var timer = setInterval(function () {
                    request.post(serverUrl + '/DbMaintenance/DatabaseDeployCI')
                }, 2000);
            });
    });
};