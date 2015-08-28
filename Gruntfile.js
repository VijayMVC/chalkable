module.exports = function(grunt) {

  var buildNumber = grunt.option("build.number");
  var vcsRevision = grunt.option("vcs.revision");
  var vcsBranch = grunt.option("vcs.branch");

  var pkg = grunt.file.readJSON('package.json');
  pkg.version = buildNumber || pkg.version;

  // Project configuration.
  grunt.initConfig({
    pkg: pkg,
    
    teamcity: {
        options: {
          // Task-specific options go here.
        },
        all: {}
    },
    
    jshint: {
      options: {
        jshintrc: true,
      },
      
      source: [
        'Chalkable.Web/app/chlk/activities/*.js',
        'Chalkable.Web/app/chlk/controllers/*.js',
        'Chalkable.Web/app/chlk/controls/*.js',
        'Chalkable.Web/app/chlk/converters/*.js',
        'Chalkable.Web/app/chlk/lib/*.js',
        'Chalkable.Web/app/chlk/models/*.js',
        'Chalkable.Web/app/chlk/services/*.js',
        'Chalkable.Web/app/chlk/templates/*.js'
      ],
      
      compiled: [
        'Chalkable.Web/app/*.compiled.js'
      ]
    },
    
    replace: {
      raygun_deployment_version: {
        src: ['Release.tpl.yaml'],
        dest: ['Release.yaml'],
        replacements: [{
          from: 'private-build',
          to: buildNumber
        }, {
          from: 'vcs-revision',
          to: vcsRevision
        }, {
          from: '${date}',
          to: new Date().toISOString().slice(0, 10)
        }, {
          from: '${vcs.branch}',
          to: vcsBranch
        }]
      }
    },
    
    raygun_deployment: {
      options: {
        raygunApiKey: 'WV05DNwmIzBvTiSQ8pgNXQ==',
        useGit: false
      }
    }
  });

  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-raygun-deployment');
  grunt.loadNpmTasks('grunt-text-replace');
  grunt.loadNpmTasks('grunt-teamcity');

  // Default task(s).
  grunt.registerTask('raygun-create-deployment', ['replace:raygun_deployment_version', 'raygun_deployment']);
  
  grunt.registerTask('default', []);

};