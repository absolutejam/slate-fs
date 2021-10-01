// Template for webpack.config.js in Fable projects
// Find latest version in https://github.com/fable-compiler/webpack-config-template

// In most cases, you'll only need to edit the CONFIG object (after dependencies)
// See below if you need better fine-tuning of Webpack options

// Dependencies. Also required: core-js, fable-loader, fable-compiler, @babel/core,
// @babel/preset-env, babel-loader, css-loader, style-loader, file-loader, resolve-url-loader
var path = require('path');
var webpack = require('webpack');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');
var MiniCssExtractPlugin = require('mini-css-extract-plugin');

const root = resolve('./src/SlateExample');
var serverPort = process.env.SERVER_PROXY_PORT || 8089;
var serverHost = (process.env.DOTNET_RUNNING_IN_CONTAINER === true) ? "server" : "localhost";
var serverUri  = `http://${serverHost}:${serverPort}`;

var CONFIG = {
    // The tags to include the generated JS and CSS will be automatically injected in the HTML template
    // See https://github.com/jantimon/html-webpack-plugin
    indexHtmlTemplate: `${root}/index.html`,
    fsharpEntry: `${root}/src/Program.fs.js`,
    cssEntry: `${root}/css/tailwind-source.css`,
    outputDir: `${root}/out`,
    assetsDir: `${root}/public`,
    devServerPort: 9092,
    // When using webpack-dev-server, you may need to redirect some calls
    // to a external API server. See https://webpack.js.org/configuration/dev-server/#devserver-proxy
    devServerProxy: {
        // redirect requests that start with /api/ to the server on port 8085
        '/api/**': {
            target: serverUri,
            changeOrigin: true
        },
        // redirect websocket requests that start with /socket/ to the server on the port 8085
        '/socket/**': {
            target: serverUri,
            ws: true
        }
    },
    // Use babel-preset-env to generate JS compatible with most-used browsers.
    // More info at https://babeljs.io/docs/en/next/babel-preset-env.html
    babel: {
        presets: [
            ['@babel/preset-env', {
                modules: false,
                // This adds polyfills when needed. Requires core-js dependency.
                // See https://babeljs.io/docs/en/babel-preset-env#usebuiltins
                // Note that you still need to add custom polyfills if necessary (e.g. whatwg-fetch)
                useBuiltIns: 'usage',
                corejs: 3
            }]
        ],
    }
}

// If we're running the webpack-dev-server, assume we're in development mode
var isProduction = !process.argv.find(v => v.indexOf('webpack-dev-server') !== -1);

console.log("\n\n")
console.log('Bundling for ' + (isProduction ? 'production' : 'development') + '...');
console.log('Proxying to backend: ' + serverUri);
console.log("\n\n")

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: resolve(CONFIG.indexHtmlTemplate)
    })
];

module.exports = {
    // In development, split the JavaScript and CSS files in order to
    // have a faster HMR support. In production bundle styles together
    // with the code because the MiniCssExtractPlugin will extract the
    // CSS in a separate files.
    entry: isProduction ? {
        app: [resolve(CONFIG.fsharpEntry), resolve(CONFIG.cssEntry)]
    } : {
        app: [resolve(CONFIG.fsharpEntry)],
        style: [resolve(CONFIG.cssEntry)]
    },
    // Add a hash to the output file name in production
    // to prevent browser caching if code changes
    ...(CONFIG.outputDir) ?
        {
            output: {
                path: resolve(CONFIG.outputDir),
                filename: isProduction ? '[name].[hash].js' : '[name].js'
            },
        } : {},
    mode: isProduction ? 'production' : 'development',
    devtool: isProduction ? "source-map" : "eval-source-map",
    optimization: {
        // Split the code coming from npm packages into a different file.
        // 3rd party dependencies change less often, let the browser cache them.
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /node_modules/,
                    name: "vendors",
                    chunks: "all"
                }
            }
        },
    },
    // Besides the HtmlPlugin, we use the following plugins:
    // PRODUCTION
    //      - MiniCssExtractPlugin: Extracts CSS from bundle to a different file
    //          To minify CSS, see https://github.com/webpack-contrib/mini-css-extract-plugin#minimizing-for-production
    //      - CopyWebpackPlugin: Copies static assets to output directory
    // DEVELOPMENT
    //      - HotModuleReplacementPlugin: Enables hot reloading when code changes without refreshing
    plugins: isProduction ?
        commonPlugins.concat([
            new MiniCssExtractPlugin({
                filename: 'style.[hash].css'
            }),
            new CopyWebpackPlugin([{
                from: resolve(CONFIG.assetsDir)
            }]),
        ]) : commonPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
        ]),
    resolve: {
        // See https://github.com/fable-compiler/Fable/issues/1490
        symlinks: false,
        // modules: [
        //     path.join(path.resolve(__dirname, '..'), 'Slate/node_modules'),
        // ],
    },
    // Configuration for webpack-dev-server
    devServer: {
        publicPath: '/',
        contentBase: resolve(CONFIG.assetsDir),
        host: '0.0.0.0',
        port: CONFIG.devServerPort,
        proxy: CONFIG.devServerProxy,
        writeToDisk: true,
        hot: true,
        inline: true,
        historyApiFallback: true
    },
    // - fable-loader: transforms F# into JS
    // - babel-loader: transforms JS to old syntax (compatible with old browsers)
    // - file-loader: Moves files referenced in the code (fonts, images) into output folder
    module: {
        rules: [
            {
                test: /\.js$/,
                enforce: "pre",
                use: ["source-map-loader"],
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: CONFIG.babel
                },
            },
            {
                test: /\.css$/,
                use: [
                    isProduction ?  MiniCssExtractPlugin.loader : 'style-loader',
                    'css-loader',
                    {
                        loader: 'postcss-loader'
                    },
                ],
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
                use: ['file-loader']
            }
        ]
    }
};

function resolve(filePath) {
    return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}
