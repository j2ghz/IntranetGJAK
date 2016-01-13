/// <binding BeforeBuild='default' Clean='clean' />
/// <reference path="wwwroot/ts/tsd.d.ts" />
/// <reference path="wwwroot/ts/_references.js" />

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
paths.sass = paths.webroot + "sass/**/*.scss";
paths.js = paths.webroot + "js/";
paths.jsOut = paths.webroot + "js/site.js";
paths.css = paths.webroot + "css/";
paths.cssOut = paths.webroot + "css/site.css";

gulp.task("clean:js", function(cb) {
    rimraf(paths.js, cb);
});

gulp.task("clean:css", function(cb) {
    rimraf(paths.css, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

//gulp.task("min:js",["typescript"], function () {
//    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
//        .pipe(concat(paths.concatJsDest))
//        .pipe(uglify())
//        .pipe(gulp.dest("."));
//});

//gulp.task("min:css", function() {
//    return gulp.src([paths.css, "!" + paths.minCss])
//        .pipe(concat(paths.concatCssDest))
//        .pipe(cssmin())
//        .pipe(gulp.dest("."));
//});

gulp.task("min", ["scripts", "sass"]);

gulp.task("default", ["min"]);

gulp.task("install", shell.task([
        "\"node_modules/.bin/bower\" install",
        "\"node_modules/.bin/gulp\""
])
);

gulp.task("scripts",["tsd","clean:js"], function () {
    var tsResult = gulp.src(paths.ts)
                       .pipe(sourcemaps.init()) // This means sourcemaps will be generated
                       .pipe(ts({
                           sortOutput: true,
                           noImplicitAny: true
                       }));

    return tsResult.js
                .pipe(sourcemaps.write()) // Now the sourcemaps are added to the .js file
                .pipe(gulp.dest(paths.js));
});

gulp.task("tsd:install", shell.task([
        "\"node_modules/.bin/tsd\" install"
    ])
);

gulp.task("tsd:rebundle", ["tsd:install"], shell.task([
        "\"node_modules/.bin/tsd\" rebundle"
    ])
);

gulp.task("tsd", ["tsd:rebundle"]);

gulp.task("sass", ["clean:css"], function() {
    return gulp.src(paths.sass)
        .pipe(sourcemaps.init())
        .pipe(sass().on("error", sass.logError))
        //.pipe(autoprefixer())
        .pipe(concat("site.css"))
        .pipe(cssmin())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(paths.css));
});