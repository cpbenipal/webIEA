
module.exports = function(grunt) {
  
  require('load-grunt-tasks')(grunt);

  // project configuration
  grunt.initConfig({
    less: {
      development: {
        options: {
          compress: true,
          yuicompress: true,
          optimization: 2
        },
        files: {
          "dist/css/style.css": "dist/less/style.less" 
        }
      }
    },
    browserify: {
      options: {
        transform: [
          [ 'stringify', {
            extensions: [ '.bpmn' ]
          } ],
          [ 'babelify', {
            global: true
          } ]
        ]
      },
      watch: {
        options: {
          watch: true
        },
        files: {
          'dist/app.js': [ 'app/**/*.js' ]
        }
      },
      app: {
        files: {
          'dist/app.js': [ 'app/**/*.js' ]
        }
      }
    },
    copy: {
      diagram_js: {
        files: [ {
          src: require.resolve('diagram-js/assets/diagram-js.css'),
          dest: 'dist/css/diagram-js.css'
        },{
          src: require.resolve('diagram-js-minimap/assets/diagram-js-minimap.css'),
          dest: 'dist/css/diagram-js-minimap.css'
        } ]
      },
      app: {
        files: [
          {
            expand: true,
            cwd: 'app/',
            src: ['**/*', '!**/*.js'],
            dest: 'dist'
          }
        ]
      }
    },

    watch: {
      options: {
        livereload: true
      },
      styles: {
        files: ['dist/**/*.less'], 
        tasks: ['less'],
        options: {
          nospawn: true
        }
      },
      samples: {
        files: [ 'app/**/*.*' ],
        tasks: [ 'copy:app' ]
      },
    },

    connect: {
      livereload: {
        options: {
          port: 9013,
          livereload: true,
          hostname: 'localhost',
          open: true,
          base: [
            'dist'
          ]
        }
      }
    }
  });

  // tasks

  grunt.registerTask('build', [ 'browserify:app', 'copy' ]);

  grunt.registerTask('auto-build', [
    'copy',
    'browserify:watch',
    'connect:livereload',
    'less','watch'
  ]);

  grunt.registerTask('default', [ 'build' ]);
};
