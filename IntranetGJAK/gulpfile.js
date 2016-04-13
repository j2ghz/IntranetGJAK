/// <binding BeforeBuild='default' Clean='clean' />
/// <reference path="wwwroot/ts/tsd.d.ts" />
/// <reference path="wwwroot/ts/_references.js" />

"use strict";

var gulp = require("gulp"),
	rimraf = require("rimraf"),
	concat = require("gulp-concat"),
	cssmin = require("gulp-cssmin"),
	sass = require("gulp-sass"),
	autoprefixer = require("gulp-autoprefixer"),
	sourcemaps = require("gulp-sourcemaps");

var paths = {
	webroot: "./wwwroot/"
};

paths.ts = paths.webroot + "ts/**/*.ts";
paths.sass = paths.webroot + "sass/**/*.scss";
paths.js = paths.webroot + "js/!js/index.js";
paths.jsOut = paths.webroot + "js/site.js";
paths.css = paths.webroot + "css/";
paths.cssOut = paths.webroot + "css/site.css";
paths.fonts = paths.webroot + "/lib/bootstrap-sass/assets/fonts/**/*";
paths.fontsOut = paths.webroot + "fonts/";

gulp.task("clean:css", function(cb) {
	rimraf(paths.css, cb);
});

gulp.task("min", ["sass", "fonts"]);

gulp.task("default", ["min"]);

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

gulp.task("fonts", function() {
	return gulp.src(paths.fonts)
		.pipe(gulp.dest(paths.fontsOut));
});
