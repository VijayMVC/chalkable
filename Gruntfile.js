module.exports = function (grunt) {

  var encryptionSecret = process.env.PEM_ENCRYPTION_SECRET;
  var nugetApiKey = process.env.NUGET_API_KEY;
  var sysAdminPrivateToken = process.env.SYSADMIN_PRIVATE_TOKEN;

  var buildNumber = grunt.option("build.number");
  var vcsRevision = grunt.option("vcs.revision");
  var vcsBranch = grunt.option("vcs.branch");
  var nugetPre = grunt.option("nuget.pre") || '';
  if (buildNumber && (typeof buildNumber.split == 'function')) {
    var buildCounter = buildNumber.split('-').pop();
    var semVer = function () {
      var x = buildNumber.split('-');
      x[2] = 0;
      return x.join('.');
    }();
  }
  var today = new Date().toISOString().slice(0, 10);
  
  var azureStorageCredentials = ['chalkablestat', process.env.AZURE_STORAGE_ACCESS_KEY];

  var pkg = grunt.file.readJSON('package.json');
  pkg.version = buildNumber || pkg.version;

  var deploy_urls = {
    'staging': 'https://dev.chalkable.com',
    'qa': 'https://classroom.qa.chalkable.com'
  };

  // Project configuration.
  grunt.initConfig({
    pkg: pkg,
    
    teamcity: {
        options: {
          // Task-specific options go here.
        },
        all: {}
    },
    
    clean: {
      js: ["Chalkable.Web/app/*.js"],
      css: ["Chalkable.Web/Content/*.css"],      
    },
    
    uglify: {
      options: {
        preserveComments: 'some'
      }
    },
    
    cssmin: {
      options: {
        shorthandCompacting: false,
        roundingPrecision: -1
      }
    },
    
    concat: {
      options: {
        separator: ';\n\n',
        banner: '/*! <%= pkg.name %> - v<%= pkg.version %> - ' + 
                '<%= grunt.template.today("yyyy-mm-dd") %> */',
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
        }, {
          from: 'private-sysadmin-token',
          to: sysAdminPrivateToken
        }]
      },
      chakable_database_master_version: {
        src: ['Chalkable.Database.Master/Chalkable.Database.Master.sqlproj'],
        dest: ['Chalkable.Database.Master/Chalkable.Database.Master.sqlproj'],
        replacements: [{
          from: '1.0.0.0',
          to: '1.0.0.' + buildCounter
        }]
      },
      chakable_database_school_version: {
        src: ['Chalkable.Database.School/Chalkable.Database.School.sqlproj'],
        dest: ['Chalkable.Database.School/Chalkable.Database.School.sqlproj'],
        replacements: [{
          from: '1.0.0.0',
          to: '1.0.0.' + buildCounter
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
      },
      nuget_version: {
        src: ['Chalkable.API/Chalkable.API.nuspec'],
        dest: ['Chalkable.API/Chalkable.API.nuspec'],
        replacements: [{
          from: '$version$',
          to: semVer + nugetPre
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
      "artifacts-db": {
        options: {
          serviceOptions: azureStorageCredentials, // custom arguments to azure.createBlobService    
          containerName: 'artifacts-db', // container name in blob
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
          src: '*.dacpac',
          cwd: './Chalkable.Database.Master/bin/Release'
        },{
          expand: true,
          src: '*.dacpac',
          cwd: './Chalkable.Database.School/bin/Release'
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
    
    compass: {                  
      'chalkable.web': {               
        options: {             
          config: 'Chalkable.Web/config.rb',
          basePath: 'Chalkable.Web',
          sassDir: 'assets/sass2',
          cssDir: 'Content',
          environment: 'production'
        }
      }
    },
    
    useminPrepare: {
      options: {
        root: './Chalkable.Web',
        dest: './Chalkable.Web',
        patterns: {
          html: [
            [
              /<script.+src=['"]@Url\.StaticContent\(['"]([^"']+)["']/gm,
              'Update the HTML to reference our concat/min/revved script files'
            ],
            [
              /<link[^\>]+href=['"]@Url\.StaticContent\(['"]([^"']+)['"]/gm,
              'Update the HTML with the new css filenames'
            ]
          ]
        }
      },
      cshtml: {
        expand: true,
        cwd: './',
        src: ['./Chalkable.Web/Views/**/*.cshtml'],
        filter: 'isFile'
      }
    },
    
    usemin: {
      cshtml: {
        expand: true,
        cwd: './',
        src: ['./Chalkable.Web/Views/**/*.cshtml'],
        filter: 'isFile'
      },
      options: {
        blockReplacements: {
          css: function (block) {
            var media = block.media ? ' media="' + block.media + '"' : '';
            return '<link rel="stylesheet" href="@Url.StaticContent("' + block.dest + '")"' + media + '>';
          },
          js: function (block) {
            var defer = block.defer ? 'defer ' : '';
            var async = block.async ? 'async ' : '';
            return '<script ' + defer + async + 'src="@Url.StaticContent("' + block.dest + '")"><\/script>';
          } 
        }
      }
    },
    
    imagemin: {                          
      'chalkable.web': {                
        files: [{
          expand: true,                  
          cwd: 'Chalkable.Web/Content/images2/',                  
          src: ['**/*.{png,jpg,jpeg,gif}', '!icons-*/*', '*-icons/*'], 
          dest: 'Chalkable.Web/Content/images2/'                 
        }]
      }
    },
    
    nugetpack: {
        dist: {
            src: 'Chalkable.API/Chalkable.API.csproj',
            dest: 'Chalkable.API/',
            options: {
                build: true,
                symbols: true,
                properties: "Configuration=Release"
            }
        }        
    },
    
    nugetpush: {
        dist: {
            src: 'Chalkable.API/Chalkable.API.' + semVer + nugetPre + '.nupkg',
       
            options: {
              apiKey: nugetApiKey
            }
        }
    },

    'chlk-deploy-db': {
      options: {
        sysAdminToken: sysAdminPrivateToken,
        serverUrl: deploy_urls[vcsBranch]
      }
    },
      
    exec: {
        'automated-tests': {
            cwd: './Chalkable.AutomatedTests/',
            cmd: 'run-all.cmd'
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
  grunt.loadNpmTasks('grunt-usemin');
  grunt.loadNpmTasks('grunt-contrib-clean');
  grunt.loadNpmTasks('grunt-contrib-imagemin');
  grunt.loadNpmTasks('grunt-nuget');
  grunt.loadNpmTasks('grunt-exec');
  
  // simple build task 
  grunt.registerTask('usemin-build', [
    'useminPrepare',
    'concat:generated',
    'cssmin:generated',
    'uglify:generated',
    //'filerev',
    'usemin'
  ]);
  
  // js concat/minify
  grunt.registerTask('version', ['replace:chakable_web_version', 'replace:chakable_database_master_version', 'replace:chakable_database_school_version']);

  // general tasks
  grunt.registerTask('deploy-artifacts', ['azure-cdn-deploy']);  
  grunt.registerTask('deploy-to-azure', ['decrypt', 'azure-cs-deploy']);
  grunt.registerTask('raygun-create-deployment', ['replace:raygun_deployment_version', 'raygun_deployment']);
  grunt.registerTask('nuget-publish', ['replace:nuget_version', 'nugetpack', 'nugetpush']);
  
  // branch specific tasks
  var postBuildTasks = ['imagemin', 'deploy-artifacts'];
  if (['staging', 'qa'].indexOf(vcsBranch) >= 0) {
    postBuildTasks.push('deploy-to-azure', 'chlk-deploy-db', 'raygun-create-deployment');
  }
  
  if (['staging'].indexOf(vcsBranch) >= 0) {
    // SKIP: postBuildTasks.push('exec:automated-tests');
  }

  if (['qa'].indexOf(vcsBranch) >= 0) {
    postBuildTasks.push('nuget-publish');
  }
  
  grunt.registerTask('post-checkout', ['clean', 'compass']);
  grunt.registerTask('pre-release', ['clean', 'compass', 'usemin-build', 'jsbuild3']);
  grunt.registerTask('post-build', postBuildTasks);
  
  // Default task(s).
  grunt.registerTask('default', []);
};