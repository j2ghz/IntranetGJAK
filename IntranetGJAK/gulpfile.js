/// <binding BeforeBuild='default' Clean='clean' />
/// <reference path="wwwroot/ts/tsd.d.ts" />
"use strict";

var gulp = require("gulp"),
    shell = require("gulp-shell"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    sass = require("gulp-sass"),
    autoprefixer = require("gulp-autoprefixer"),
    sourcemaps = require("gulp-sourcemaps"),
    ts = require("gulp-typescript");

var paths = {
    webroot: "./wwwroot/"
};

paths.ts = paths.webroot + "ts/**/*.ts";
paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.webroot + "js/", cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.webroot + "css/", cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js",["typescript"], function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "sass"]);

gulp.task("default", ["min"]);

gulp.task("install", ["min"]);

gulp.task("typescript", ["tsd:rebundle"] ,function () {
    return gulp.src(paths.ts)
        .pipe(ts({
            noImplicitAny: true,
            out: "site.js"
        })
        .pipe(gulp.dest(paths.webroot + "js/")));
});

gulp.task("tsd:install", shell.task([
        "\"node_modules/.bin/tsd\" install"
])
);

gulp.task("tsd:rebundle",["tsd:install"], shell.task([
        "\"node_modules/.bin/tsd\" rebundle"
])
);

gulp.task("sass", function () {
    return gulp.src(paths.webroot +"sass/*.scss")
        .pipe(sass().on('error', sass.logError))
        .pipe(sourcemaps.init())
        .pipe(autoprefixer())
        .pipe(concat("stylesheet.css"))
        .pipe(sourcemaps.write("."))
        .pipe(cssmin())
        .pipe(gulp.dest(paths.webroot + "css/"));
});