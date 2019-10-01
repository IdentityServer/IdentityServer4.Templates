/*
	Gulp tasks for: AdminUI Docker and TailwindCSS
	Author: Sam and Karl
*/

var gulp = require('gulp');
var template = require('gulp-template');
var cssimport = require("gulp-cssimport");
var postcss = require('gulp-postcss');
var purgecss = require('gulp-purgecss');
var rename = require('gulp-rename');
var tailwindcss = require('tailwindcss');
var uglifycss = require('gulp-uglifycss');

// paths
var tailwind_dir = './tailwind/';
var tailwind_output_dir = './assets/styles/';

// custom purgecss extractor(s) https://github.com/FullHuman/purgecss
class TailwindExtractor {
	static extract(content) {
		return content.match(/[A-z0-9-:\/]+/g);
	}
}

// docker_env
function docker_env() {
    process.env.ApiUrl = process.env.ApiUrl || "http://localhost:5001";
    process.env.UiUrl = process.env.UiUrl || "http://localhost:5000";
    process.env.AuthorityUrl = process.env.AuthorityUrl || "http://ids:5003";
    process.env.AddUserPassword = process.env.AddUserPassword || false;
    return gulp.src('./env/env.js')
        .pipe(
            template({
                env: {
                    "ApiUrl": process.env.ApiUrl,
                    "UiUrl": process.env.UiUrl,
                    "AuthorityUrl": process.env.AuthorityUrl,
                    "AddUserPassword": process.env.AddUserPassword
                }
            })
        )
        .pipe(gulp.dest('./assets'));
}

// tailwind_build
function tailwind_build() {
    return gulp.src(tailwind_dir + 'app.css')
        .pipe(cssimport())
        .pipe(postcss([
            tailwindcss(tailwind_dir + 'tailwind.config.js'),
            require('postcss-nested'),
            require('autoprefixer')
        ]))
        .pipe(uglifycss({
            "maxLineLen": 312,
            "uglyComments": true
        }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(tailwind_output_dir));
}

// tailwind_purge
function tailwind_purge() {
    return gulp.src(tailwind_output_dir + 'app.min.css')
        .pipe(purgecss({
            content: ['./**/*.html', './**/*.js'],
            extractors: [{
                extractor: TailwindExtractor,
                extensions: ['html', 'ts']
            }],
            whitelistPatternsChildren: [/cms$/]
        }))
        .pipe(gulp.dest(tailwind_output_dir));
}

// tailwind_watcher
function tailwind_watcher() {
    gulp.watch([tailwind_dir + '**/*.*'], tailwind_build);
}

const build = gulp.series(tailwind_build, tailwind_purge);
const watch = gulp.series(tailwind_build, tailwind_watcher);

exports.docker_env = docker_env;
exports.build = build;
exports.watch = watch;