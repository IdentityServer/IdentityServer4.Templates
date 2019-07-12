/*

    IMPORTANT: MOVE ME TO DOCS PROJECT ROOT

    support: barnz@rocksolidknowledge.com

*/

const gulp = require('gulp');
const exec = require('child_process').exec;
const concat = require('gulp-concat');
const clean = require('gulp-clean');
const zip = require('gulp-zip');
const purgecss = require('gulp-purgecss');

// custom purgecss extractor
// https://github.com/FullHuman/purgecss
class TailwindExtractor {
    static extract(content) {
        return content.match(/[A-z0-9-:\/]+/g);
    }
}

gulp.task('purge_css', () => {
    return gulp.src(['./site/css/*.css', "!./site/css/prism-okaidia.min.css", "!./site/css/search.css"])
        .pipe(purgecss({
            content: ["./site/**/*.html"],
            extractors: [{
                extractor: TailwindExtractor,
                extensions: ['html', 'js']
            }]
        }))
        .pipe(gulp.dest('./site/css'))
})

gulp.task('mkdocs_build_forApp', (done) => {
    console.log("Building app config...");
    gulp.src(['./config/base.mkdocs.yml', './config/*.app.mkdocs.yml', './config/nav.mkdocs.yml']).pipe(concat('mkdocs.app.yml')).pipe(gulp.dest('./'));
    console.log("...done");

    console.log("\nExec: mkdocs build --clean -f mkdocs.app.yml");
    exec('mkdocs build --clean -f mkdocs.app.yml', function(err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);

        console.log("Zipping app docs...");
        gulp.src(['./site/**', '!./site/gulpfile.js', '!./site/package.json', '!./site/package-lock.json', '!./site/MANIFEST.in', '!./site/README', '!./site/sitemap.xml.gz']).pipe(zip('docs-app.zip')).pipe(gulp.dest('./'));
        console.log("...done");
        done(err);
    });
})

gulp.task('mkdocs_build_forWeb', (done) => {
    console.log("Building web config...");
    gulp.src(['./config/base.mkdocs.yml', './config/*.web.mkdocs.yml', './config/nav.mkdocs.yml']).pipe(concat('mkdocs.web.yml')).pipe(gulp.dest('./'));
    console.log("...done");

    console.log("\nExec: mkdocs build --clean -f mkdocs.web.yml");
    exec('mkdocs build --clean -f mkdocs.web.yml', function(err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);

        console.log("Zipping web docs...");
        gulp.src(['./site/**', '!./site/gulpfile.js', '!./site/package.json', '!./site/package-lock.json', '!./site/MANIFEST.in', '!./site/README']).pipe(zip('docs-web.zip')).pipe(gulp.dest('./'));
        console.log("...done");
        done(err);
    });
})

gulp.task('cleanup', (done) => {
    var cleanme = ['./mkdocs.app.yml', './mkdocs.web.yml', './site/'];

    for(var i = 0; i < cleanme.length; i++) {
        try {
            console.log("Removing " + cleanme[i]);
            gulp.src(cleanme[i], { read: false, allowEmpty: true }).pipe(clean());
            console.log("...success");
        } catch(e) {
            console.log("Error removing " + cleanme[i]);
            console.log(e);
        }
    }

    done();
})

gulp.task('build-app', gulp.series('mkdocs_build_forApp', 'purge_css'));
gulp.task('build-web', gulp.series('mkdocs_build_forWeb', 'purge_css'));

gulp.task('build', gulp.series('build-web', 'build-app', 'cleanup'));