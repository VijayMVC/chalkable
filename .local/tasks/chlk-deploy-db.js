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

        request.post(serverUrl + '/DbMaintenance/DatabaseDeployCI', {form: {key: sysAdminToken}}, processQueueTask);

        var timer;
        function processQueueTask(err, httpResponse, body) {
            if (err) {
                grunt.log.error('Deployment failed: ' + err.message);
                done(false);
                return;
            }

            try {
                var response = JSON.parse(body);
            } catch (e) {
                grunt.log.error('Deployment failed: ' + body);
                done(false);
                return;
            }

            if (!response.success) {
                grunt.log.error('Deployment failed: ' + response.data.message);
                done(false);
                return;
            }

            var taskId = response.data;
            timer = setInterval(function () {
                request.post(serverUrl + '/DbMaintenance/GetTaskStateCI', {form: { key: sysAdminToken, id: taskId}}, handleTaskStatus);
            }, 2000);
        }

        function handleTaskStatus(err, httpResponse, body) {
            if (err) {
                grunt.log.error('Deployment failed: ' + err.message);
                done(false);
                return;
            }

            try {
                var response = JSON.parse(body);
            } catch (e) {
                grunt.log.error('Deployment failed: ' + body);
                done(false);
                return;
            }

            if (!response.success) {
                grunt.log.error('Deployment failed: ' + response.data.message);
                done(false);
                return;
            }

            var taskStatus = response.data;
            switch (taskStatus.toLowerCase()) {
                case 'created':
                case 'processing':
                    grunt.log.debug(taskStatus);
                    return ;

                case 'failed':
                case 'canceled':
                    grunt.log.error(taskStatus);
                    clearInterval(timer);
                    return done(false);

                case 'processed':
                    grunt.log.ok(taskStatus);
                    clearInterval(timer);
                    return done(true);
            }
        }
    });
};