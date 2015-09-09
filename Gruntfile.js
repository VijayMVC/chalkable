module.exports = function(grunt) {

  var encryptionSecret = process.env.PEM_ENCRYPTION_SECRET;

  var buildNumber = grunt.option("build.number");
  var vcsRevision = grunt.option("vcs.revision");
  var vcsBranch = grunt.option("vcs.branch");
  
  var today = new Date().toISOString().slice(0, 10);
  
  var azureStorageCredentials = ['chalkablestat', process.env.AZURE_STORAGE_ACCESS_KEY];

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
    
    uglify: {
      options: {
        preserveComments: 'some'
      },
      'chalkable.web': {
        files: {
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine.min.js' : 'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine.js', 
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/languages/jquery.validationEngine-en.min.js' : 'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine-en.js', 
          'Chalkable.Web/app/chlk/index/main.min.js': 'Chalkable.Web/app/chlk/index/main.js',
          'Chalkable.Web/app/chlk/index/sign_in_forms.min.js': 'Chalkable.Web/app/chlk/index/sign_in_forms.js',
          'Chalkable.Web/app/chlk/shared.min.js': 'Chalkable.Web/app/chlk/shared.js',
          'Chalkable.Web/app/chlk/chlk-messages.min.js': 'Chalkable.Web/app/chlk/chlk-messages.js',
          'Chalkable.Web/app/chlk/chlk-constants.min.js': 'Chalkable.Web/app/chlk/chlk-constants.js',
          'Chalkable.Web/app/jquery/jquery.menu-aim.min.js': 'Chalkable.Web/app/jquery/jquery.menu-aim.js',
          'Chalkable.Web/app/jquery/jquery.quicksand.min.js': 'Chalkable.Web/app/jquery/jquery.quicksand.js',
          'Chalkable.Web/app/jquery/jquery.cycle.min.js': 'Chalkable.Web/app/jquery/jquery.cycle.js',
          'Chalkable.Web/app/jquery/jquery.fancybox.min.js': 'Chalkable.Web/app/jquery/jquery.fancybox.js',
          'Chalkable.Web/app/chlk/index/html5shiv.min.js': 'Chalkable.Web/app/chlk/index/html5shiv.js',
          
          'Chalkable.Web/Scripts/api/chlk-post-message-api.min.js': 'Chalkable.Web/Scripts/api/chlk-post-message-api.js'
        }
      }
    },
    
    cssmin: {
      options: {
        shorthandCompacting: false,
        roundingPrecision: -1
      },
      'chalkable.web': {
        files : {
          'Chalkable.Web/Content/index-layout.min.css': [
            'Chalkable.Web/Content/index.css', 
            'Chalkable.Web/app/jquery/validation/css/template.css'
          ],
          'Chalkable.Web/Content/role-layout.min.css': [
            'Chalkable.Web/app/bower/chosen/chosen.min.css',
            'Chalkable.Web/app/bower/jquery-ui/themes/smoothness/jquery-ui.css',
            'Chalkable.Web/app/jquery/snippet/jquery.snippet.min.css',
            'Chalkable.Web/app/chlk/index/prettify.css'
          ],
          'Chalkable.Web/Content/devdocs-layout.min.css': [
            'Chalkable.Web/app/jquery/snippet/jquery.snippet.min.css',
            'Chalkable.Web/app/bower/jquery-ui/themes/smoothness/jquery-ui.css',
            'Chalkable.Web/app/bower/jQuery-Validation-Engine/css/validationEngine.jquery.css',
            'Chalkable.Web/app/jquery/validation/css/template_index.css'
          ]
        }
      }
    },
    
    concat: {
      options: {
        separator: ';\n\n',
        banner: '/*! <%= pkg.name %> - v<%= pkg.version %> - ' + 
                '<%= grunt.template.today("yyyy-mm-dd") %> */',
      },
      'index-layout': {
        src: [
          'Chalkable.Web/app/bower/jquery/dist/jquery.min.js', 
          'Chalkable.Web/app/bower/jquery-ui/jquery-ui.min.js', 
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine.min.js', 
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine-en.min.js', 
          'Chalkable.Web/app/chlk/index/retina-1.1.0.min.js', 
          'Chalkable.Web/app/chlk/index/main.min.js', 
          'Chalkable.Web/app/chlk/index/sign_in_forms.min.js'
          ],
        dest: 'Chalkable.Web/app/index-layout.min.js',
      },
      'role-layout': {
        src: [
          'Chalkable.Web/app/chlk/shared.min.js',
          'Chalkable.Web/app/chlk/chlk-messages.min.js',
          'Chalkable.Web/app/chlk/chlk-constants.min.js',
          'Chalkable.Web/app/lib/date-en-US.js',
          'Chalkable.Web/app/bower/jquery/dist/jquery.min.js', 
          'Chalkable.Web/app/bower/autosize/dist/autosize.min.js', 
          'Chalkable.Web/app/bower/chosen/chosen.jquery.min.js', 
          'Chalkable.Web/app/bower/jcarousel/dist/jquery.jcarousel.min.js', 
          'Chalkable.Web/app/bower/jquery-ui/jquery-ui.min.js', 
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine-en.min.js', 
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine.min.js', 
          'Chalkable.Web/app/jquery/snippet/jquery.snippet.min.js', 
          'Chalkable.Web/app/bower/jquery-validation/dist/jquery.validate.min.js', 
          'Chalkable.Web/app/jquery/jquery.menu-aim.min.js', 
          'Chalkable.Web/app/jquery/jquery.quicksand.min.js', 
          'Chalkable.Web/app/jquery/validation/jquery.creditCardValidator.js', 
          'Chalkable.Web/app/jquery/jquery.maskedinput-1.3.1.min.js', 
          'Chalkable.Web/app/jquery/jquery.youtube.js', 
          'Chalkable.Web/app/chlk/index/prettify.js', 
          'Chalkable.Web/app/highcharts/highcharts.js'
          ],
        dest: 'Chalkable.Web/app/role-layout.min.js',
      },
      'devdocs-layout': {
        src: [
          'Chalkable.Web/app/bower/jquery/dist/jquery.min.js', 
          'Chalkable.Web/app/jquery/jquery.menu-aim.min.js', 
          'Chalkable.Web/app/jquery/jquery.maskedinput-1.3.1.min.js', 
          'Chalkable.Web/app/bower/jquery-ui/jquery-ui.min.js', 
          'Chalkable.Web/app/jquery/snippet/jquery.snippet.min.js', 
          'Chalkable.Web/app/jquery/jquery.cycle.min.js', 
          'Chalkable.Web/app/jquery/jquery.fancybox.min.js', 
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/languages/jquery.validationEngine-en.min.js', 
          'Chalkable.Web/app/bower/jQuery-Validation-Engine/js/jquery.validationEngine.min.js',
          'Chalkable.Web/app/chlk/shared.min.js',
          'Chalkable.Web/app/chlk/index/html5shiv.min.js',
          'Chalkable.Web/app/chlk/index/sign_in_forms.min.js'
        ],
        dest: 'Chalkable.Web/app/devdocs-layout.min.js',
      }
    },
    
    crypt:{
      files:[{
        dir: '.certs',                         // root dir of files to encrypt / decrypt
        include: 'ChalkableAzure.pem',        // pattern to include files
        encryptedExtension: '.encrypted'       // extension used for encrypted files
      }],
      options:{
        key: encryptionSecret
      }
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
      chakable_web_version: {
        src: ['Chalkable.Web/Tools/CompilerHelper.cs'],
        dest: ['Chalkable.Web/Tools/CompilerHelper.cs'],
        replacements: [{
          from: 'private-build',
          to: buildNumber
        }]
      },
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
          to: today
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
    },
    
    'azure-cdn-deploy': {
      artifacts: {
        options: {
          serviceOptions: azureStorageCredentials, // custom arguments to azure.createBlobService    
          containerName: 'artifacts', // container name in blob
          concurrentUploadThreads: 10, // number of concurrent uploads, choose best for your network condition
          folder: buildNumber,
          deleteExistingBlobs: false,
          zip: false,
          metadata: {
            cacheControl: 'public, max-age=31530000', // cache in browser
            cacheControlHeader: 'public, max-age=31530000' // cache in azure CDN. As this data does not change, we set it to 1 year
          },
          testRun: false // test run - means no blobs will be actually deleted or uploaded, see log messages for details
        },
        files: [{
          expand: true,
          src: 'Chalkable.Azure.cspkg',
          cwd: './Chalkable.Azure/bin/Release/app.publish/'
        },{
          expand: true,
          src: 'ServiceConfiguration.{Staging,QA,Support,Release,Sales,Staging2}.cscfg',
          cwd: './Chalkable.Azure/'
        }]
      },
      statics: {
        options: {
          serviceOptions: azureStorageCredentials, // custom arguments to azure.createBlobService    
          containerName: 'static-' + buildNumber, // container name in blob
          containerOptions: {publicAccessLevel: "blob"},
          concurrentUploadThreads: 10, // number of concurrent uploads, choose best for your network condition
          deleteExistingBlobs: false,
          zip: false,
          metadata: {
            cacheControl: 'public, max-age=31530000', // cache in browser
            cacheControlHeader: 'public, max-age=31530000' // cache in azure CDN. As this data does not change, we set it to 1 year
          },
          testRun: false // test run - means no blobs will be actually deleted or uploaded, see log messages for details
        },
        
        files: [{
          expand: true,
          cwd: './Chalkable.Web',
          src: [
            'app/*App.compiled.js',
            'app/*.min.js',
            'scripts/*.min.js',
            'app/chlk/index/modernizr-ck.js',
            
            'Content/**',
            '!Content/images2/alerts-icons/*',
            '!Content/images2/icons-*/*'
          ],
          
          filter: 'isFile',
          dot: true
        },        
        {
          expand: true,
          cwd: './Chalkable.Web/app/bower/chosen/',
          src: [
            '*.png'
          ],
          dest: 'Content',          
          filter: 'isFile'
        },        
        {
          expand: true,
          cwd: './Chalkable.Web/app/bower/jquery-ui/themes/smoothness/',
          src: [
            'images/*'
          ],
          dest: 'Content',
          
          filter: 'isFile'
        }]
      }
    },
    
    'azure-cs-deploy': {
      options: {
        credentials: {
          subscriptionId: '9aa0817a-76b5-4870-8499-64a09c714263',
          pemPath: './.certs/ChalkableAzure.pem'
        },
        
        serviceName: 'chalkable' + vcsBranch,
        deploymentSlot: 'Production',
        parameters: {
          force: false,
          mode: 'Auto',
          packageUri: 'https://chalkablestat.blob.core.windows.net/artifacts/' + buildNumber + '/Chalkable.Azure.cspkg',
          configurationPath: './Chalkable.Azure/ServiceConfiguration.' + vcsBranch + '.cscfg',
          label: 'TeamCity deploy ' + vcsBranch + ' ' + buildNumber + ' ' + today,
        }
      }
    },
    
    jsbuild3: {
        options: {
          config: 'Chalkable.Web/jsbuild.json',
          modules: []
        },
        all: {}
    },
    
    compass: {                  // Task
      'chalkable.web': {                   // Target
        options: {              // Target options
          config: 'Chalkable.Web/config.rb',
          basePath: 'Chalkable.Web',
          sassDir: 'assets/sass2',
          cssDir: 'Content',
          environment: 'production'
        }
      }
    }
  });

  grunt.loadTasks('./.local/tasks');

  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-raygun-deployment');
  grunt.loadNpmTasks('grunt-text-replace');
  grunt.loadNpmTasks('grunt-teamcity');
  grunt.loadNpmTasks('grunt-contrib-crypt');
  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-cssmin');
  grunt.loadNpmTasks('emp.ria-grunt-jsbuild3');
  grunt.loadNpmTasks('grunt-contrib-compass');
  
  // js concat/minify
  grunt.registerTask('version', ['replace:chakable_web_version']);
  grunt.registerTask('jsmin', ['uglify:chalkable.web', 'concat:index-layout', 'concat:role-layout', 'concat:devdocs-layout']);
  
  // general tasks
  grunt.registerTask('deploy-artifacts', ['azure-cdn-deploy']);  
  grunt.registerTask('deploy-to-azure', ['decrypt', 'azure-cs-deploy']);
  grunt.registerTask('raygun-create-deployment', ['replace:raygun_deployment_version', 'raygun_deployment']);
  
  // branch specific tasks
  var postBuildTasks = ['deploy-artifacts'];
  if (['staging', 'qa'].indexOf(vcsBranch) >= 0) {
    postBuildTasks.push('deploy-to-azure', 'raygun-create-deployment');
  }
  
  grunt.registerTask('post-checkout', ['compass', 'uglify:chalkable.web']);
  grunt.registerTask('pre-release', ['compass', 'cssmin', 'jsmin', 'jsbuild3']);
  grunt.registerTask('post-build', postBuildTasks);
  
  // Default task(s).
  grunt.registerTask('default', []);
};