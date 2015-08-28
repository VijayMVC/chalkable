var fs = require('fs');

module.exports = function (grunt) {
  var compute = require('azure-mgmt-compute');

  function required(options, prop) {
    if (!options[prop])
      throw Error('Option ' + prop + ' is required');
  }

  grunt.registerTask('azure-cs-deploy', 'Deploy Azure Cloud Service', function() {
    var done = this.async();
    
    var options = this.options();
    required(options, 'credentials');    
    required(options, 'serviceName');
    required(options, 'deploymentSlot');
    required(options, 'parameters');
  
    var credentials = compute.createCertificateCloudCredentials(options.credentials),
        serviceName = options.serviceName,
        deploymentSlot = options.deploymentSlot,
        parameters = options.parameters;

    credentials.pem = fs.readFileSync(credentials.pemPath, 'utf-8')

    var client = new compute.createComputeManagementClient(credentials);
    
    parameters.configuration = fs.readFileSync(parameters.configurationPath, 'utf-8').replace(/^\uFEFF/, '')
    
    var operation = client.deployments.upgradeBySlot(serviceName, deploymentSlot, parameters, function (err, result) {
      if (err) {
        grunt.log.error('Deployment failed: ' + err.message);
        done(false);
      } else {        
        grunt.log.ok('Deployment success: ' + JSON.strigify(result));
        done();
      }
    });
  });
};