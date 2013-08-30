/** @path lib/stacktrace.js */;/**
 * http://cloud.github.com/downloads/eriwen/javascript-stacktrace/stacktrace-min-0.4.js
 *
 */
function printStackTrace(a){var a=a||{guess:!0},b=a.e||null,a=!!a.guess,d=new printStackTrace.implementation,b=d.run(b);return a?d.guessAnonymousFunctions(b):b}printStackTrace.implementation=function(){};
printStackTrace.implementation.prototype={run:function(a,b){a=a||this.createException();b=b||this.mode(a);return"other"===b?this.other(arguments.callee):this[b](a)},createException:function(){try{this.undef()}catch(a){return a}},mode:function(a){return a.arguments&&a.stack?"chrome":a.stack&&a.sourceURL?"safari":"string"===typeof a.message&&"undefined"!==typeof window&&window.opera?!a.stacktrace||-1<a.message.indexOf("\n")&&a.message.split("\n").length>a.stacktrace.split("\n").length?"opera9":!a.stack?
    "opera10a":0>a.stacktrace.indexOf("called from line")?"opera10b":"opera11":a.stack?"firefox":"other"},instrumentFunction:function(a,b,d){var a=a||window,c=a[b];a[b]=function(){d.call(this,printStackTrace().slice(4));return a[b]._instrumented.apply(this,arguments)};a[b]._instrumented=c},deinstrumentFunction:function(a,b){a[b].constructor===Function&&(a[b]._instrumented&&a[b]._instrumented.constructor===Function)&&(a[b]=a[b]._instrumented)},chrome:function(a){a=(a.stack+"\n").replace(/^\S[^\(]+?[\n$]/gm,
    "").replace(/^\s+(at eval )?at\s+/gm,"").replace(/^([^\(]+?)([\n$])/gm,"{anonymous}()@$1$2").replace(/^Object.<anonymous>\s*\(([^\)]+)\)/gm,"{anonymous}()@$1").split("\n");a.pop();return a},safari:function(a){return a.stack.replace(/\[native code\]\n/m,"").replace(/^@/gm,"{anonymous}()@").split("\n")},firefox:function(a){return a.stack.replace(/(?:\n@:0)?\s+$/m,"").replace(/^[\(@]/gm,"{anonymous}()@").split("\n")},opera11:function(a){for(var b=/^.*line (\d+), column (\d+)(?: in (.+))? in (\S+):$/,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                  a=a.stacktrace.split("\n"),d=[],c=0,f=a.length;c<f;c+=2){var e=b.exec(a[c]);if(e){var g=e[4]+":"+e[1]+":"+e[2],e=e[3]||"global code",e=e.replace(/<anonymous function: (\S+)>/,"$1").replace(/<anonymous function>/,"{anonymous}");d.push(e+"@"+g+" -- "+a[c+1].replace(/^\s+/,""))}}return d},opera10b:function(a){for(var b=/^(.*)@(.+):(\d+)$/,a=a.stacktrace.split("\n"),d=[],c=0,f=a.length;c<f;c++){var e=b.exec(a[c]);e&&d.push((e[1]?e[1]+"()":"global code")+"@"+e[2]+":"+e[3])}return d},opera10a:function(a){for(var b=
    /Line (\d+).*script (?:in )?(\S+)(?:: In function (\S+))?$/i,a=a.stacktrace.split("\n"),d=[],c=0,f=a.length;c<f;c+=2){var e=b.exec(a[c]);e&&d.push((e[3]||"{anonymous}")+"()@"+e[2]+":"+e[1]+" -- "+a[c+1].replace(/^\s+/,""))}return d},opera9:function(a){for(var b=/Line (\d+).*script (?:in )?(\S+)/i,a=a.message.split("\n"),d=[],c=2,f=a.length;c<f;c+=2){var e=b.exec(a[c]);e&&d.push("{anonymous}()@"+e[2]+":"+e[1]+" -- "+a[c+1].replace(/^\s+/,""))}return d},other:function(a){for(var b=/function\s*([\w\-$]+)?\s*\(/i,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      d=[],c,f;a&&a.arguments&&10>d.length;)c=b.test(a.toString())?RegExp.$1||"{anonymous}":"{anonymous}",f=Array.prototype.slice.call(a.arguments||[]),d[d.length]=c+"("+this.stringifyArguments(f)+")",a=a.caller;return d},stringifyArguments:function(a){for(var b=[],d=Array.prototype.slice,c=0;c<a.length;++c){var f=a[c];void 0===f?b[c]="undefined":null===f?b[c]="null":f.constructor&&(f.constructor===Array?b[c]=3>f.length?"["+this.stringifyArguments(f)+"]":"["+this.stringifyArguments(d.call(f,0,1))+"..."+
    this.stringifyArguments(d.call(f,-1))+"]":f.constructor===Object?b[c]="#object":f.constructor===Function?b[c]="#function":f.constructor===String?b[c]='"'+f+'"':f.constructor===Number&&(b[c]=f))}return b.join(",")},sourceCache:{},ajax:function(a){var b=this.createXMLHTTPObject();if(b)try{return b.open("GET",a,!1),b.send(null),b.responseText}catch(d){}return""},createXMLHTTPObject:function(){for(var a,b=[function(){return new XMLHttpRequest},function(){return new ActiveXObject("Msxml2.XMLHTTP")},function(){return new ActiveXObject("Msxml3.XMLHTTP")},
    function(){return new ActiveXObject("Microsoft.XMLHTTP")}],d=0;d<b.length;d++)try{return a=b[d](),this.createXMLHTTPObject=b[d],a}catch(c){}},isSameDomain:function(a){return"undefined"!==typeof location&&-1!==a.indexOf(location.hostname)},getSource:function(a){a in this.sourceCache||(this.sourceCache[a]=this.ajax(a).split("\n"));return this.sourceCache[a]},guessAnonymousFunctions:function(a){for(var b=0;b<a.length;++b){var d=/^(.*?)(?::(\d+))(?::(\d+))?(?: -- .+)?$/,c=a[b],f=/\{anonymous\}\(.*\)@(.*)/.exec(c);
    if(f){var e=d.exec(f[1]);e&&(d=e[1],f=e[2],e=e[3]||0,d&&(this.isSameDomain(d)&&f)&&(d=this.guessAnonymousFunction(d,f,e),a[b]=c.replace("{anonymous}",d)))}}return a},guessAnonymousFunction:function(a,b){var d;try{d=this.findFunctionName(this.getSource(a),b)}catch(c){d="getSource failed with url: "+a+", exception: "+c.toString()}return d},findFunctionName:function(a,b){for(var d=/function\s+([^(]*?)\s*\(([^)]*)\)/,c=/['"]?([0-9A-Za-z_]+)['"]?\s*[:=]\s*function\b/,f=/['"]?([0-9A-Za-z_]+)['"]?\s*[:=]\s*(?:eval|new Function)\b/,
                                                                                                                                                                                                                                                                                                                                                                                               e="",g,j=Math.min(b,20),h,i=0;i<j;++i)if(g=a[b-i-1],h=g.indexOf("//"),0<=h&&(g=g.substr(0,h)),g)if(e=g+e,(g=c.exec(e))&&g[1]||(g=d.exec(e))&&g[1]||(g=f.exec(e))&&g[1])return g[1];return"(?)"}};;/** @path lib/jade.runtime.js */;(function(e){if("function"==typeof bootstrap)bootstrap("jade",e);else if("object"==typeof exports)module.exports=e();else if("function"==typeof define&&define.amd)define(e);else if("undefined"!=typeof ses){if(!ses.ok())return;ses.makeJade=e}else"undefined"!=typeof window?window.jade=e():global.jade=e()})(function(){var define,ses,bootstrap,module,exports;
return (function(e,t,n){function i(n,s){if(!t[n]){if(!e[n]){var o=typeof require=="function"&&require;if(!s&&o)return o(n,!0);if(r)return r(n,!0);throw new Error("Cannot find module '"+n+"'")}var u=t[n]={exports:{}};e[n][0].call(u.exports,function(t){var r=e[n][1][t];return i(r?r:t)},u,u.exports)}return t[n].exports}var r=typeof require=="function"&&require;for(var s=0;s<n.length;s++)i(n[s]);return i})({1:[function(require,module,exports){

/*!
 * Jade - runtime
 * Copyright(c) 2010 TJ Holowaychuk <tj@vision-media.ca>
 * MIT Licensed
 */

/**
 * Lame Array.isArray() polyfill for now.
 */

if (!Array.isArray) {
  Array.isArray = function(arr){
    return '[object Array]' == Object.prototype.toString.call(arr);
  };
}

/**
 * Lame Object.keys() polyfill for now.
 */

if (!Object.keys) {
  Object.keys = function(obj){
    var arr = [];
    for (var key in obj) {
      if (obj.hasOwnProperty(key)) {
        arr.push(key);
      }
    }
    return arr;
  }
}

/**
 * Merge two attribute objects giving precedence
 * to values in object `b`. Classes are special-cased
 * allowing for arrays and merging/joining appropriately
 * resulting in a string.
 *
 * @param {Object} a
 * @param {Object} b
 * @return {Object} a
 * @api private
 */

exports.merge = function merge(a, b) {
  var ac = a['class'];
  var bc = b['class'];

  if (ac || bc) {
    ac = ac || [];
    bc = bc || [];
    if (!Array.isArray(ac)) ac = [ac];
    if (!Array.isArray(bc)) bc = [bc];
    a['class'] = ac.concat(bc).filter(nulls);
  }

  for (var key in b) {
    if (key != 'class') {
      a[key] = b[key];
    }
  }

  return a;
};

/**
 * Filter null `val`s.
 *
 * @param {*} val
 * @return {Boolean}
 * @api private
 */

function nulls(val) {
  return val != null && val !== '';
}

function valueOf(obj) {
    return obj && 'function' === typeof obj.valueOf ? obj.valueOf() : obj;
}

/**
 * join array as classes.
 *
 * @param {*} val
 * @return {String}
 * @api private
 */

function joinClasses(val) {
  return Array.isArray(val) ? val.map(joinClasses).filter(nulls).join(' ') : valueOf(val);
}

/**
 * Render the given attributes object.
 *
 * @param {Object} obj
 * @param {Object} escaped
 * @return {String}
 * @api private
 */

exports.attrs = function attrs(obj, escaped){
  var buf = []
    , terse = obj.terse;

  delete obj.terse;
  var keys = Object.keys(obj)
    , len = keys.length;

  if (len) {
    buf.push('');
    for (var i = 0; i < len; ++i) {
      var key = keys[i]
        , val = obj[key];

      if ('boolean' == typeof val || null == val) {
        if (val) {
          terse
            ? buf.push(key)
            : buf.push(key + '="' + key + '"');
        }
      } else if (0 == key.indexOf('data') && 'string' != typeof val) {
        buf.push(key + "='" + JSON.stringify(valueOf(val)) + "'");
      } else if ('class' == key) {
        if (val = exports.escape(joinClasses(val))) {
          buf.push(key + '="' + val + '"');
        }
      } else if (escaped && escaped[key]) {
        buf.push(key + '="' + exports.escape(valueOf(val)) + '"');
      } else {
        buf.push(key + '="' + val + '"');
      }
    }
  }

  return buf.join(' ');
};

/**
 * Escape the given string of `html`.
 *
 * @param {String} html
 * @return {String}
 * @api private
 */

exports.escape = function escape(html){
  return String(html)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
};

/**
 * Re-throw the given `err` in context to the
 * the jade in `filename` at the given `lineno`.
 *
 * @param {Error} err
 * @param {String} filename
 * @param {String} lineno
 * @api private
 */

exports.rethrow = function rethrow(err, filename, lineno){
  if (!filename) throw err;
  if (typeof window != 'undefined') throw err;

  var context = 3
    , str = require('fs').readFileSync(filename, 'utf8')
    , lines = str.split('\n')
    , start = Math.max(lineno - context, 0)
    , end = Math.min(lines.length, lineno + context);

  // Error context
  var context = lines.slice(start, end).map(function(line, i){
    var curr = i + start + 1;
    return (curr == lineno ? '  > ' : '    ')
      + curr
      + '| '
      + line;
  }).join('\n');

  // Alter exception message
  err.path = filename;
  err.message = (filename || 'Jade') + ':' + lineno
    + '\n' + context + '\n\n' + err.message;
  throw err;
};

/**
 * Contains globals for jade scoping
 * @type {Object}
 */

exports.globals = {};

},{"fs":2}],2:[function(require,module,exports){
// nothing to see here... no file methods for the browser

},{}]},{},[1])(1)
});
;;(function(jQuery, jade) {
    var __ASSETS = {}, chlk = {};
    var _DEBUG;
    (function() {
        (ria = ria || {}).__CFG = [].slice.call(document.getElementsByTagName("script")).map(function(_) {
            return _.innerText || _.innerHTML;
        }).filter(function(text) {
            return text.match(/ria\.__CFG\s+=\s+\{/);
        }).map(function(text) {
            return JSON.parse(text.split("=").pop());
        }).pop();
    })();
    function ASSET(id) {
        return __ASSETS[id];
    }
    var ria = ria || {};
    ria.__API = ria.__API || {};
    (function() {
        "use strict";
        ria.__API.getPrototypeOf = function(v) {
            return Object.getPrototypeOf(v) || v.prototype || v.__proto__;
        };
        ria.__API.getConstructorOf = function(v) {
            return ria.__API.getPrototypeOf(v).constructor;
        };
        ria.__API.inheritFrom = function(superClass) {
            function InheritanceProxyClass() {}
            InheritanceProxyClass.prototype = superClass.prototype;
            return new InheritanceProxyClass();
        };
        ria.__API.extend = function(subClass, superClass) {
            subClass.prototype = ria.__API.inheritFrom(superClass);
            subClass.prototype.constructor = subClass;
            subClass.super_ = superClass.prototype;
        };
        ria.__API.extendWithDefault = function(first, second) {
            for (var prop in second) {
                if (!first.hasOwnProperty(prop)) first[prop] = second[prop];
            }
            return first;
        };
        ria.__API.getInstanceOf = function(ctor, name_) {
            var f = function InstanceOfProxy() {
                this.constructor = ctor;
            };
            if (ria.__CFG.prettyStackTraces) f = new Function("ctor", "return " + f.toString().replace("InstanceOfProxy", name_))(ctor);
            f.prototype = ctor.prototype;
            return new f();
        };
        ria.__API.getIdentifierOfType = function(type) {
            if (type === undefined) return "void";
            if (type === null) return "*";
            if (type === Function) return "Function";
            if (type === Number) return "Number";
            if (type === Boolean) return "Boolean";
            if (type === String) return "String";
            if (type === RegExp) return "RegExp";
            if (type === Date) return "Date";
            if (type === Array) return "Array";
            if (type === Object) return "Object";
            if (ria.__API.isArrayOfDescriptor(type) || ria.__API.isClassOfDescriptor(type) || ria.__API.isImplementerOfDescriptor(type)) return type.toString();
            if (type.__META) return type.__META.name;
            return type.name || "UnknownType";
        };
        ria.__API.getIdentifierOfValue = function(value) {
            if (value === undefined || value === null) return "void";
            if (typeof value === "number") return "Number";
            if (typeof value === "boolean") return "Boolean";
            if (typeof value === "string") return "String";
            if (typeof value === "regexp") return "RegExp";
            if (typeof value === "date") return "Date";
            if (typeof value === "function") return "Function";
            if (Array.isArray(value)) return "Array";
            if (ria.__API.getConstructorOf(value).__META) return ria.__API.getConstructorOf(value).__META.name;
            if (value instanceof Object) {
                var ctor = ria.__API.getConstructorOf(value);
                if (ctor) return ctor.name || "Constructor";
            }
            return "Object";
        };
        ria.__API.clone = function clone(obj) {
            switch (typeof obj) {
              case "number":
              case "string":
              case "boolean":
              case "regexp":
                return obj;

              default:
                if (Array.isArray(obj) || obj.length === +obj.length) return [].slice.call(obj);
                if ("function" == typeof obj.clone) return obj.clone();
                if (ria.__API.getConstructorOf(obj) !== Object) throw Error("Can not clone instance of " + ria.__API.getIdentifierOfValue(obj));
                var result = {};
                Object.keys(obj).forEach(function(_) {
                    result[_] = obj[_];
                });
                return result;
            }
        };
        ria.__API.defer = function defer(scope, method, args_, delay_) {
            setTimeout(function() {
                method.apply(scope, args_ || []);
            }, delay_ || 1);
        };
    })();
    (function() {
        "use strict";
        var pmcStages = {
            callInit_: [],
            OnCallInit: function(body, meta, scope, callSession) {
                this.callInit_.forEach(function(_) {
                    _(body, meta, scope, callSession);
                });
            },
            beforeCall_: [],
            OnBeforeCall: function(body, meta, scope, args, callSession) {
                this.beforeCall_.forEach(function(_) {
                    _(body, meta, scope, args, callSession);
                });
            },
            afterCall_: [],
            OnAfterCall: function(body, meta, scope, args, result, callSession) {
                this.afterCall_.forEach(function(_) {
                    result = _(body, meta, scope, args, result, callSession);
                });
                return result;
            },
            callFinally_: [],
            OnCallFinally: function(body, meta, scope, callSession) {
                this.callFinally_.forEach(function(_) {
                    _(body, meta, scope, callSession);
                });
            }
        };
        function PipelineMethodCall(body, meta, scope, args) {
            var callSession = {};
            pmcStages.OnCallInit(body, meta, scope, callSession);
            try {
                pmcStages.OnBeforeCall(body, meta, scope, args, callSession);
                var result = body.apply(scope, args);
                return pmcStages.OnAfterCall(body, meta, scope, args, result, callSession);
            } finally {
                pmcStages.OnCallFinally(body, meta, scope, callSession);
            }
        }
        ria.__API.addPipelineMethodCallStage = function(stage, worker) {
            switch (stage) {
              case "CallInit":
                pmcStages.callInit_.push(worker);
                break;

              case "BeforeCall":
                pmcStages.beforeCall_.push(worker);
                break;

              case "AfterCall":
                pmcStages.afterCall_.push(worker);
                break;

              case "CallFinally":
                pmcStages.callFinally_.push(worker);
                break;
            }
        };
        ria.__API.getPipelineMethodCallProxyFor = function(body, meta, scope) {
            var f_ = function PipelineMethodCallProxy() {
                return PipelineMethodCall(body, meta, scope, [].slice.call(arguments));
            };
            f_.__META = meta;
            _DEBUG && Object.freeze(f_);
            return f_;
        };
    })();
    (function() {
        function getStackTrace(e) {
            var callstack = [];
            var lines, i, len;
            if (e.stack) {
                lines = e.stack.split("\n");
                for (i = 0, len = lines.length; i < len; i++) {
                    if (lines[i].match(/^\s*[A-Za-z0-9\-_\$]+\(/)) {
                        callstack.push(lines[i]);
                    }
                }
                callstack.shift();
                return callstack;
            }
            if (window.opera && e.message) {
                lines = e.message.split("\n");
                for (i = 0, len = lines.length; i < len; i++) {
                    if (lines[i].match(/^\s*[A-Za-z0-9\-_\$]+\(/)) {
                        var entry = lines[i];
                        if (lines[i + 1]) {
                            entry += " at " + lines[i + 1];
                            i++;
                        }
                        callstack.push(entry);
                    }
                }
                callstack.shift();
                return callstack;
            }
            if (arguments.callee) {
                var currentFunction = arguments.callee.caller;
                while (currentFunction) {
                    var fn = currentFunction.toString();
                    var fname = fn.substring(fn.indexOf("function") + 8, fn.indexOf("")) || "anonymous";
                    callstack.push(fname);
                    currentFunction = currentFunction.caller;
                }
            }
            return callstack;
        }
        function getPrintStackTraceWrapper() {
            "use strict";
            return function(e) {
                return window.printStackTrace({
                    e: e,
                    guess: ria.__CFG.prettyStackTraces
                });
            };
        }
        ria.__API.getStackTrace = window.printStackTrace ? getPrintStackTraceWrapper() : getStackTrace;
    })();
    (function() {
        "use strict";
        function AnnotationDescriptor(name, argsTypes, argsNames) {
            this.name = name;
            this.argsNames = argsNames;
            this.argsTypes = argsTypes;
            this.ret = null;
            _DEBUG && Object.freeze(this);
        }
        ria.__API.AnnotationDescriptor = AnnotationDescriptor;
        function AnnotationInstance(args, meta) {
            for (var k in args) if (args.hasOwnProperty(k)) {
                this[k] = args[k];
            }
            this.__META = meta;
            _DEBUG && Object.freeze(this);
        }
        ria.__API.annotation = function(name, argsTypes_, argsNames_) {
            function AnnotationProxy() {
                var args = [].slice.call(arguments);
                var o = {};
                for (var index = 0; index < argsNames_.length; index++) {
                    o[argsNames_[index]] = args[index];
                }
                return new AnnotationInstance(o, AnnotationProxy.__META);
            }
            AnnotationProxy.__META = new AnnotationDescriptor(name, argsTypes_, argsNames_);
            var fn_ = AnnotationProxy;
            fn_ = ria.__CFG.enablePipelineMethodCall ? ria.__API.getPipelineMethodCallProxyFor(fn_, fn_.__META, null) : fn_;
            _DEBUG && Object.freeze(fn_);
            return fn_;
        };
        ria.__API.isAnnotation = function(value) {
            if (typeof value === "function") {
                return value.__META instanceof AnnotationDescriptor;
            }
            return value instanceof AnnotationInstance;
        };
    })();
    (function() {
        "use strict";
        function MethodDescriptor(name, ret, argsTypes, argsNames) {
            this.name = name;
            this.ret = ret;
            this.argsTypes = argsTypes;
            this.argsNames = argsNames;
            _DEBUG && Object.freeze(this);
        }
        MethodDescriptor.prototype.isProtected = function() {
            return /^.+_$/.test(this.name);
        };
        ria.__API.MethodDescriptor = MethodDescriptor;
        ria.__API.delegate = function(name, ret_, argsTypes_, argsNames_) {
            function DelegateProxy(fn) {
                return ria.__CFG.enablePipelineMethodCall ? ria.__API.getPipelineMethodCallProxyFor(fn, DelegateProxy.__META, null) : fn;
            }
            DelegateProxy.__META = new MethodDescriptor(name, ret_, argsTypes_, argsNames_);
            _DEBUG && Object.freeze(DelegateProxy);
            return DelegateProxy;
        };
        ria.__API.isDelegate = function(delegate) {
            return delegate.__META instanceof MethodDescriptor;
        };
    })();
    (function() {
        "use strict";
        function EnumDescriptor(enumClass, name) {
            this.enumClass = enumClass;
            this.name = name;
        }
        ria.__API.EnumDescriptor = EnumDescriptor;
        ria.__API.enum = function(enumClass, name) {
            enumClass.__META = new EnumDescriptor(enumClass, name);
        };
        ria.__API.isEnum = function(value) {
            return value && value.__META ? value.__META instanceof EnumDescriptor : false;
        };
    })();
    (function() {
        "use strict";
        function IdentifierDescriptor(identifierClass, name) {
            this.identifierClass = identifierClass;
            this.name = name;
            _DEBUG && Object.freeze(this);
        }
        ria.__API.IdentifierDescriptor = IdentifierDescriptor;
        ria.__API.identifier = function(identifierClass, name) {
            identifierClass.__META = new IdentifierDescriptor(identifierClass, name);
        };
        ria.__API.isIdentifier = function(value) {
            return value.__META instanceof IdentifierDescriptor;
        };
    })();
    (function() {
        "use strict";
        function InterfaceDescriptor(name, methods) {
            this.name = name;
            this.methods = {};
            methods.forEach(function(method) {
                this.methods[method[0]] = {
                    retType: method[1],
                    argsNames: method[3],
                    argsTypes: method[2]
                };
            }.bind(this));
        }
        ria.__API.InterfaceDescriptor = InterfaceDescriptor;
        var ifcRegister = {};
        ria.__API.getInterfaceByName = function(name) {
            return ifcRegister[name];
        };
        ria.__API.ifc = function(ifc, name, methods) {
            ifcRegister[name] = ifc;
            ifc.__META = new InterfaceDescriptor(name, methods);
            return ifc;
        };
        ria.__API.Interface = new function InterfaceBase() {}();
        ria.__API.isInterface = function(ifc) {
            return ifc.__META instanceof InterfaceDescriptor;
        };
    })();
    (function() {
        "use strict";
        function TypeOf(type) {}
        function ClassDescriptor(name, base, ifcs, anns, isAbstract) {
            this.name = name;
            this.base = base;
            this.ifcs = [].concat.call(base ? base.__META.ifcs : []).concat(ifcs);
            this.anns = anns;
            this.isAbstract = isAbstract;
            this.properties = base ? ria.__API.clone(base.__META.properties) : {};
            this.methods = base ? ria.__API.clone(base.__META.methods) : {};
            this.ctor = null;
            this.children = [];
        }
        ClassDescriptor.prototype.addProperty = function(name, ret, anns, getter, setter) {
            this.properties[name] = {
                retType: ret,
                annotations: anns,
                getter: getter,
                setter: setter
            };
        };
        ClassDescriptor.prototype.addMethod = function(impl, name, ret, argsTypes, argsNames, anns) {
            this.methods[name] = {
                impl: impl,
                retType: ret,
                argsNames: argsNames,
                argsTypes: argsTypes,
                annotations: anns
            };
        };
        ClassDescriptor.prototype.setCtor = function(impl, argsTypes, argsNames, anns) {
            this.ctor = {
                impl: impl,
                argsNames: argsNames,
                argsTypes: argsTypes,
                annotations: anns
            };
        };
        ClassDescriptor.prototype.addChild = function(clazz) {
            if (!ria.__API.isClassConstructor(clazz)) throw Error("Child should be a CLASS");
            if (clazz.__META.base.__META != this) throw Error("Child should extend me.");
            this.children.push(clazz);
        };
        ria.__API.ClassDescriptor = ClassDescriptor;
        var clazzRegister = {};
        ria.__API.getClassByName = function(name) {
            return clazzRegister[name];
        };
        ria.__API.clazz = function(clazz, name, base_, ifcs_, anns_, isAbstract_) {
            clazzRegister[name] = clazz;
            clazz.__META = new ClassDescriptor(name, base_, ifcs_, anns_, isAbstract_ || false);
            if (base_) {
                ria.__API.extend(clazz, base_);
                base_.__META.addChild(clazz);
            }
        };
        ria.__API.property = function(clazz, name, propType_, anns_, getter, setter) {
            getter.__META = new ria.__API.MethodDescriptor("", propType_, [], []);
            if (setter) {
                setter.__META = new ria.__API.MethodDescriptor("", undefined, [ propType_ ], [ "value" ]);
            }
            clazz.__META.addProperty(name, propType_, anns_, getter, setter);
        };
        ria.__API.method = function(clazz, impl, name, ret_, argsTypes_, argsNames_, anns_) {
            clazz.__META.addMethod(impl, name, ret_, argsTypes_, argsNames_, anns_);
            impl.__META = new ria.__API.MethodDescriptor(name, ret_, argsTypes_, argsNames_);
            _DEBUG && Object.freeze(impl);
        };
        ria.__API.ctor = function(clazz, impl, argsTypes_, argsNames_, anns_) {
            clazz.__META.setCtor(impl, argsTypes_, argsNames_, anns_);
            impl.__META = new ria.__API.MethodDescriptor("$", undefined, argsTypes_, argsNames_);
            _DEBUG && Object.freeze(impl);
        };
        function ProtectedMethodProxy() {
            throw Error("Can NOT call protected methods");
        }
        ria.__API.init = function(instance, clazz, ctor, args) {
            if (clazz.__META.isAbstract) throw Error("Can NOT instantiate asbtract class " + clazz.__META.name);
            if (!(instance instanceof clazz)) instance = ria.__API.getInstanceOf(clazz, clazz.__META.name.split(".").pop());
            var publicInstance = instance;
            if (_DEBUG) {
                instance = ria.__API.getInstanceOf(clazz, clazz.__META.name.split(".").pop());
                publicInstance.__PROTECTED = instance;
            }
            for (var k in instance) {
                var name_ = k;
                var f_ = instance[name_];
                if (typeof f_ === "function" && f_ !== ctor && k !== "constructor") {
                    instance[name_] = f_.bind(instance);
                    if (ria.__CFG.enablePipelineMethodCall && f_.__META) {
                        var fn = ria.__API.getPipelineMethodCallProxyFor(f_, f_.__META, instance);
                        if (_DEBUG) {
                            Object.defineProperty(instance, name_, {
                                writable: false,
                                configurable: false,
                                value: fn
                            });
                            if (f_.__META.isProtected()) fn = ProtectedMethodProxy;
                        }
                        publicInstance[name_] = fn;
                        _DEBUG && Object.defineProperty(publicInstance, name_, {
                            writable: false,
                            configurable: false,
                            value: fn
                        });
                    }
                }
            }
            if (_DEBUG) {
                instance.$ = undefined;
                publicInstance.$ = undefined;
            }
            if (ria.__CFG.enablePipelineMethodCall && ctor.__META) {
                ctor = ria.__API.getPipelineMethodCallProxyFor(ctor, ctor.__META, instance);
            }
            if (_DEBUG) for (var name in clazz.__META.properties) {
                if (clazz.__META.properties.hasOwnProperty(name)) {
                    instance[name] = null;
                }
            }
            ctor.apply(instance, args);
            _DEBUG && Object.seal(instance);
            _DEBUG && Object.freeze(publicInstance);
            return publicInstance;
        };
        ria.__API.compile = function(clazz) {
            _DEBUG && Object.freeze(clazz);
        };
        ria.__API.isClassConstructor = function(type) {
            return type.__META instanceof ClassDescriptor;
        };
        ria.__API.Class = function() {
            function ClassBase() {
                ria.__API.init(this, ClassBase, ClassBase.prototype.$, arguments);
            }
            ria.__API.clazz(ClassBase, "Class", null, [], []);
            ClassBase.prototype.$ = function() {
                this.__hashCode = Math.random().toString(36);
                _DEBUG && Object.defineProperty(this, "hashCode", {
                    writable: false,
                    configurable: false
                });
            };
            ria.__API.ctor(ClassBase, ClassBase.prototype.$, [], [], []);
            ClassBase.prototype.getClass = function() {
                return ria.__API.getConstructorOf(this);
            };
            ria.__API.method(ClassBase, ClassBase.prototype.getClass, "getClass", Function, [], [], []);
            ClassBase.prototype.getHashCode = function() {
                return this.__hashCode;
            };
            ria.__API.method(ClassBase, ClassBase.prototype.getHashCode, "getHashCode", String, [], [], []);
            ClassBase.prototype.equals = function(other) {
                return this.getHashCode() === other.getHashCode();
            };
            ria.__API.method(ClassBase, ClassBase.prototype.equals, "equals", Boolean, [ ClassBase ], [ "other" ], []);
            ria.__API.compile(ClassBase);
            return ClassBase;
        }();
    })();
    (function() {
        function ArrayOfDescriptor(clazz) {
            this.clazz = clazz;
        }
        ArrayOfDescriptor.isArrayOfDescriptor = function(ds) {
            return ds instanceof ArrayOfDescriptor;
        };
        ArrayOfDescriptor.prototype = {
            toString: function() {
                return "Array<" + ria.__API.getIdentifierOfType(this.clazz) + ">";
            },
            valueOf: function() {
                return this.clazz;
            }
        };
        Object.freeze(ArrayOfDescriptor);
        ria.__API.ArrayOfDescriptor = ArrayOfDescriptor;
        function ArrayOf(clazz) {
            if (clazz == undefined) throw Error("Expected class or type, but gor undefined");
            return new ArrayOfDescriptor(clazz);
        }
        ria.__API.ArrayOf = ArrayOf;
        ria.__API.isArrayOfDescriptor = ArrayOfDescriptor.isArrayOfDescriptor;
    })();
    (function() {
        function ClassOfDescriptor(clazz) {
            this.clazz = clazz;
        }
        ClassOfDescriptor.isClassOfDescriptor = function(ds) {
            return ds instanceof ClassOfDescriptor;
        };
        ClassOfDescriptor.prototype = {
            toString: function() {
                return "ClassOf<" + ria.__API.getIdentifierOfType(this.clazz) + ">";
            },
            valueOf: function() {
                return this.clazz;
            }
        };
        Object.freeze(ClassOfDescriptor);
        ria.__API.ClassOfDescriptor = ClassOfDescriptor;
        function ClassOf(clazz) {
            if (clazz == undefined) throw Error("Expected class in ClassOf, but got undefined");
            if (!ria.__API.isClassConstructor(clazz) && clazz !== ria.__SYNTAX.Modifiers.SELF) throw Error("Expected class in ClassOf, but got " + ria.__API.getIdentifierOfType(clazz));
            return new ClassOfDescriptor(clazz);
        }
        ria.__API.ClassOf = ClassOf;
        ria.__API.isClassOfDescriptor = ClassOfDescriptor.isClassOfDescriptor;
    })();
    (function() {
        ria.__API.Exception = function() {
            "use strict";
            function ExceptionBase() {
                ria.__API.init(this, ExceptionBase, ExceptionBase.prototype.$, arguments);
            }
            ria.__API.clazz(ExceptionBase, "Exception", null, [], []);
            ExceptionBase.prototype.$ = function(msg, inner_) {
                this.msg = msg;
                this.stack = ria.__API.getStackTrace(Error(msg));
                this.inner_ = inner_;
            };
            ria.__API.ctor(ExceptionBase, ExceptionBase.prototype.$, [ String, [ Error, ExceptionBase ] ], [ "msg", "inner_" ], []);
            ExceptionBase.prototype.toString = function() {
                var msg = this.stack.join("\n  ").replace("Error:", ria.__API.getIdentifierOfValue(this) + ":");
                if (this.inner_) {
                    msg += "\nCaused by: ";
                    if (this.inner_ instanceof Error) {
                        msg += ria.__API.getStackTrace(this.inner_).join("\n  ");
                    } else {
                        msg += this.inner_.toString();
                    }
                }
                return msg;
            };
            ria.__API.method(ExceptionBase, ExceptionBase.prototype.toString, "toString", String, [], [], []);
            ExceptionBase.prototype.getMessage = function() {
                return this.msg;
            };
            ria.__API.method(ExceptionBase, ExceptionBase.prototype.getMessage, "getMessage", String, [], [], []);
            ExceptionBase.prototype.getStack = function() {
                return this.stack;
            };
            ria.__API.method(ExceptionBase, ExceptionBase.prototype.getStack, "getStack", Array, [], [], []);
            ria.__API.compile(ExceptionBase);
            return ExceptionBase;
        }();
    })();
    (function() {
        function ImplementerOfDescriptor(clazz) {
            this.ifc = clazz;
        }
        ImplementerOfDescriptor.isImplementerOfDescriptor = function(ds) {
            return ds instanceof ImplementerOfDescriptor;
        };
        ImplementerOfDescriptor.prototype = {
            toString: function() {
                return "ImplementerOf<" + ria.__API.getIdentifierOfType(this.ifc) + ">";
            },
            valueOf: function() {
                return this.ifc;
            }
        };
        Object.freeze(ImplementerOfDescriptor);
        ria.__API.ImplementerOfDescriptor = ImplementerOfDescriptor;
        function ImplementerOf(ifc) {
            if (ifc == undefined) throw Error("Expected interface in ImplementerOf, but got undefined");
            if (!ria.__API.isInterface(ifc)) throw Error("Expected interface in ImplementerOf, but got " + ria.__API.getIdentifierOfType(ifc));
            return new ImplementerOfDescriptor(ifc);
        }
        ria.__API.ImplementerOf = ImplementerOf;
        ria.__API.isImplementerOfDescriptor = ImplementerOfDescriptor.isImplementerOfDescriptor;
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        ria.__SYNTAX.validateAnnotationDecl = function(def) {
            if (def.annotations.length) throw Error("Annotations are not supported in annotations");
            if (def.flags.isAbstract || def.flags.isOverride || def.flags.isFinal) throw Error("Modifiers are not supported in annotations");
            if (def.retType !== null) throw Error("Return type is not supported in annotations");
            def.argsTypes.forEach(function(type) {
                if (type == ria.__SYNTAX.Modifiers.SELF) throw Error("Argument type can't be SELF in annotations");
            });
        };
        ria.__SYNTAX.compileAnnotation = function(name, def) {
            return ria.__API.annotation(name, def.argsTypes.map(function(_) {
                return _.value;
            }), def.argsNames);
        };
        ria.__SYNTAX.ANNOTATION = function() {
            var def = ria.__SYNTAX.parseMember(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
            ria.__SYNTAX.validateAnnotationDecl(def);
            var name = ria.__SYNTAX.getFullName(def.name);
            var annotation = ria.__SYNTAX.compileAnnotation(name, def);
            ria.__SYNTAX.isProtected(name) || ria.__SYNTAX.define(name, annotation);
            return annotation;
        };
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    ria.__CFG.AssertWithExceptions = !!ria.__CFG.AssertWithExceptions;
    (function() {
        "use strict";
        function Assert(condition, msg_) {
            if (!!condition) return;
            if (ria.__CFG.AssertWithExceptions) {
                throw Error('Assert failed with msg "' + msg_ + '"');
            }
            if (confirm('Assert failed with msg "' + msg_ + '". Debug?')) debugger;
        }
        ria.__SYNTAX.Assert = Assert;
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        function getDefaultGetter(property, name) {
            return {
                value: new Function("return " + function getPropertyProxy() {
                    return this.name;
                }.toString().replace("name", property).replace("getProperty", name))()
            };
        }
        function getDefaultSetter(property, name) {
            return {
                value: new Function("return " + function setPropertyProxy(value) {
                    this.name = value;
                }.toString().replace("name", property).replace("setProperty", name))()
            };
        }
        function getDefaultCtor(name) {
            return {
                value: new Function("return " + function ConstructorProxy(value) {
                    BASE();
                }.toString().replace("Constructor", name))()
            };
        }
        function findParentMethod(def, name) {
            if (!def.base) return null;
            var base = ria.__SYNTAX.Registry.find(def.base.value.__META.name);
            var baseMethod = null;
            while (base) {
                baseMethod = base.methods.filter(function(method) {
                    return method.name == name;
                }).pop();
                if (baseMethod) break;
                base = base.base && ria.__SYNTAX.Registry.find(base.base.value.__META.name);
            }
            return baseMethod;
        }
        function findParentProperty(def, name) {
            var base = def.base && def.base.__SYNTAX_META;
            var baseProperty = null;
            while (base) {
                baseProperty = base.properties.filter(function(property) {
                    return property.name == name;
                }).pop();
                if (baseProperty) break;
                base = base.base && base.base.__SYNTAX_META;
            }
            return baseProperty;
        }
        function findParentPropertyFromMeta(def, name) {
            var base = def.base && def.base.__META;
            var baseProperty;
            while (base) {
                baseProperty = base.properties[name];
                if (baseProperty) break;
                base = base.base && base.base.__META;
            }
            return baseProperty;
        }
        function findParentPropertyByGetterOrSetter(def, name) {
            var base = def.base && def.base.__SYNTAX_META;
            var baseProperty, res = {};
            while (base) {
                base.properties.forEach(function(property) {
                    if (property.getSetterName() == name) {
                        baseProperty = property;
                        res.isGetter = true;
                    }
                    if (property.getGetterName() == name) {
                        baseProperty = property;
                        res.isSetter = true;
                    }
                });
                if (baseProperty) break;
                base = base.base && base.base.__SYNTAX_META;
            }
            res.property = baseProperty;
            return res;
        }
        function isSameFlags(def1, def2) {
            for (var flag in def1.flags) {
                if (flag == "isReadonly") continue;
                if (def1.flags.hasOwnProperty(flag) && def1.flags[flag] != def2.flags[flag]) return false;
            }
            return true;
        }
        function validateGetterSetterOverride(method, def, baseSearchResult, processedMethods) {
            if (!method.flags.isOverride) throw Error("Method" + method.name + " have to be marked as OVERRIDE in " + def.name + " class");
            var property = baseSearchResult.property;
            if (property.flags.isFinal) throw Error("There is no ability to override setter or getter of final property " + property.name + " in " + def.name + " class");
            var getter, setter;
            var setterInMethods = def.methods.filter(function(_1) {
                return _1.name == property.getSetterName();
            }).pop();
            setterInMethods && processedMethods.push(setterInMethods.name);
            if (setterInMethods && !setterInMethods.flags.isOverride) throw Error("Method" + setterInMethods.name + " have to be marked as OVERRIDE in " + def.name + " class");
            var getterInMethods = def.methods.filter(function(_1) {
                return _1.name == property.getGetterName();
            }).pop();
            getterInMethods && processedMethods.push(getterInMethods.name);
            if (getterInMethods && !getterInMethods.flags.isOverride) throw Error("Method" + getterInMethods.name + " have to be marked as OVERRIDE in " + def.name + " class");
            if (property.flags.isReadonly && setterInMethods) throw Error("There is no ability to add setter to READONLY property " + property.name + " in " + def.name + " class");
            if (setterInMethods && getterInMethods && !isSameFlags(setterInMethods, getterInMethods)) throw Error("Setter" + setterInMethods.name + " ang getter" + getterInMethods.name + " have to have to have the same flags in " + def.name + " class");
            var newProperty = new ria.__SYNTAX.PropertyDescriptor(property.name, property.type, property.annotations, method.flags);
            newProperty.getterDef = getterInMethods;
            newProperty.setterDef = setterInMethods;
            def.properties.push(newProperty);
        }
        function compileGetterSetterOverride(method, def, baseSearchResult, processedMethods, ClassProxy) {
            var property = baseSearchResult.property;
            var getter, setter;
            var setterInMethods = def.methods.filter(function(_1) {
                return _1.name == property.getSetterName();
            }).pop();
            setterInMethods && processedMethods.push(setterInMethods.name);
            var getterInMethods = def.methods.filter(function(_1) {
                return _1.name == property.getGetterName();
            }).pop();
            getterInMethods && processedMethods.push(getterInMethods.name);
            var propertyFromMeta = findParentPropertyFromMeta(def, property.name);
            getter = getterInMethods ? getterInMethods.body.value : function() {
                return BASE();
            };
            ClassProxy.prototype[getter.name] = getter;
            getter.__BASE_BODY = propertyFromMeta.getter;
            getter.__SELF = ClassProxy;
            if (!property.flags.isReadonly) {
                setter = setterInMethods ? setterInMethods.body.value : function(value) {
                    BASE(value);
                };
                ClassProxy.prototype[setter.name] = setter;
                setter.__BASE_BODY = propertyFromMeta.setter;
                setter.__SELF = ClassProxy;
            }
            ria.__API.property(ClassProxy, property.name, property.type, property.annotations, getter, setter);
            var newProperty = new ria.__SYNTAX.PropertyDescriptor(property.name, property.type, property.annotations, method.flags);
            def.properties.push(newProperty);
        }
        function validateMethodDeclaration(def, method) {
            var parentMethod = findParentMethod(def, method.name);
            if (method.flags.isOverride && !parentMethod) {
                throw Error("There is no " + method.name + " method in base classes of " + def.name + " class");
            }
            if (method.flags.isAbstract && parentMethod) {
                throw Error(method.name + " can't be abstract, because there is method with the same name in one of the base classes");
            }
            if (parentMethod && parentMethod.flags.isFinal) {
                throw Error("Final method " + method.name + " can't be overridden in " + def.name + " class");
            }
        }
        function compileMethodDeclaration(def, method, ClassProxy) {
            var parentMethod = findParentMethod(def, method.name);
            if (method.flags.isOverride) {
                method.body.value.__BASE_BODY = parentMethod.body.value;
            }
            if (method.retType instanceof ria.__SYNTAX.Tokenizer.SelfToken) {
                method.retType = new ria.__SYNTAX.Tokenizer.RefToken(ClassProxy);
            }
            if (method.retType && method.retType.value instanceof ria.__API.ArrayOfDescriptor && method.retType.value.clazz == ria.__SYNTAX.Modifiers.SELF) {
                method.retType = new ria.__SYNTAX.Tokenizer.RefToken(ria.__API.ArrayOf(ClassProxy));
            }
            if (method.retType && method.retType.value instanceof ria.__API.ClassOfDescriptor && method.retType.value.clazz == ria.__SYNTAX.Modifiers.SELF) {
                method.retType = new ria.__SYNTAX.Tokenizer.RefToken(ria.__API.ClassOf(ClassProxy));
            }
            if (method.retType && method.retType.value instanceof ria.__API.ClassOfDescriptor && method.retType.value.clazz == ria.__SYNTAX.Modifiers.SELF) {
                method.retType = new ria.__SYNTAX.Tokenizer.RefToken(ria.__API.ClassOf(ClassProxy));
            }
            method.argsTypes.forEach(function(t, index) {
                if (method.argsTypes[index] instanceof ria.__SYNTAX.Tokenizer.SelfToken) method.argsTypes[index] = new ria.__SYNTAX.Tokenizer.RefToken(ClassProxy);
                if (method.argsTypes[index] && method.argsTypes[index].value instanceof ria.__API.ArrayOfDescriptor && method.argsTypes[index].value.clazz == ria.__SYNTAX.Modifiers.SELF) {
                    method.argsTypes[index] = new ria.__SYNTAX.Tokenizer.RefToken(ria.__API.ArrayOf(ClassProxy));
                }
                if (method.argsTypes[index] && method.argsTypes[index].value instanceof ria.__API.ClassOfDescriptor && method.argsTypes[index].value.clazz == ria.__SYNTAX.Modifiers.SELF) {
                    method.argsTypes[index] = new ria.__SYNTAX.Tokenizer.RefToken(ria.__API.ClassOf(ClassProxy));
                }
                if (method.argsTypes[index] && method.argsTypes[index].value instanceof ria.__API.ClassOfDescriptor && method.argsTypes[index].value.clazz == ria.__SYNTAX.Modifiers.SELF) {
                    method.argsTypes[index] = new ria.__SYNTAX.Tokenizer.RefToken(ria.__API.ClassOf(ClassProxy));
                }
            });
            var impl = ClassProxy.prototype[method.name] = method.body.value;
            impl.__SELF = ClassProxy;
            ria.__API.method(ClassProxy, impl, method.name, method.retType ? method.retType.value : null, method.argsTypes.map(function(_) {
                return _.value;
            }), method.argsNames, method.annotations.map(function(_) {
                return _.value;
            }));
        }
        function validatePropertyDeclaration(property, def, processedMethods) {
            var getterName = property.getGetterName();
            var setterName = property.getSetterName();
            var getterDef = def.methods.filter(function(_1) {
                return _1.name == getterName;
            }).pop();
            if (getterDef && !isSameFlags(property, getterDef)) throw Error("The flags of getter " + getterDef.name + " should be the same with property flags");
            var setterDef = def.methods.filter(function(_1) {
                return _1.name == setterName;
            }).pop();
            if (setterDef && property.flags.isReadonly) throw Error("There is no ability to add setter to READONLY property " + property.name + " in " + def.name + " class");
            if (setterDef && !isSameFlags(property, setterDef)) throw Error("The flags of setter " + setterDef.name + " should be the same with property flags");
            processedMethods.push(getterName);
            processedMethods.push(setterName);
            property.getterDef = getterDef;
            property.setterDef = setterDef;
        }
        function compilePropertyDeclaration(property, ClassProxy, processedMethods) {
            var getterName = property.getGetterName();
            var setterName = property.getSetterName();
            var getterDef = property.getterDef;
            var setterDef = property.setterDef;
            processedMethods.push(getterName);
            processedMethods.push(setterName);
            if (property.type instanceof ria.__SYNTAX.Tokenizer.SelfToken) {
                property.type = new ria.__SYNTAX.Tokenizer.RefToken(ClassProxy);
            }
            var getter = getterDef ? getterDef.body : getDefaultGetter(property.name, getterName);
            ClassProxy.prototype[getterName] = getter.value;
            ClassProxy.prototype[getterName].__SELF = ClassProxy;
            var setter = null;
            if (!property.flags.isReadonly) {
                setter = setterDef ? setterDef.body : getDefaultSetter(property.name, setterName);
                ClassProxy.prototype[setterName] = setter.value;
                ClassProxy.prototype[setterName].__SELF = ClassProxy;
            }
            ria.__API.property(ClassProxy, property.name, property.type.value, property.annotations.map(function(_) {
                return _.value;
            }), getter.value, setter ? setter.value : null);
        }
        function compileCtorDeclaration(def, ClassProxy, processedMethods) {
            var ctorDef = def.methods.filter(function(_1) {
                return _1.name == "$";
            }).pop();
            var ctor = ctorDef ? ctorDef.body.value : getDefaultCtor(def.name).value;
            var argsTypes = ctorDef ? ctorDef.argsTypes : [];
            var argsNames = ctorDef ? ctorDef.argsNames : [];
            var anns = ctorDef ? ctorDef.annotations.map(function(item) {
                return item.value;
            }) : [];
            ClassProxy.prototype.$ = ctor;
            ClassProxy.prototype.$.__BASE_BODY = def.base ? def.base.value.__META.ctor.impl : undefined;
            ClassProxy.prototype.$.__SELF = ClassProxy;
            ria.__API.ctor(ClassProxy, ClassProxy.prototype.$, argsTypes.map(function(_) {
                return _.value;
            }), argsNames, anns);
            processedMethods.push("$");
        }
        function validateBaseClassMethodDeclaration(def, baseMethod) {
            var childMethod = def.methods.filter(function(method) {
                return method.name == baseMethod.name;
            }).pop();
            if (baseMethod.flags.isFinal) {
                if (childMethod) throw Error("There is no ability to override final method " + childMethod.name + " in " + def.name + " class");
            } else if (baseMethod.flags.isAbstract) {
                if (!childMethod) throw Error("The abstract method " + baseMethod.name + " have to be overridden in " + def.name + " class");
                if (!childMethod.flags.isOverride) throw Error("The overridden method " + childMethod.name + " have to be marked as OVERRIDE in " + def.name + " class");
            } else {
                if (childMethod && !childMethod.flags.isOverride) throw Error("The overridden method " + childMethod.name + " have to be marked as OVERRIDE in " + def.name + " class");
            }
        }
        function validateBaseClassPropertyDeclaration(baseProperty, childGetter, childSetter, def) {
            if (baseProperty.flags.isFinal) {
                if (childGetter || childSetter) throw Error("There is no ability to override getter or setter of final property " + baseProperty.name + " in " + def.name + " class");
            } else if (baseProperty.flags.isAbstract) {
                if (!childGetter || !childSetter) throw Error("The setter and getter of abstract property " + baseProperty.name + " have to be overridden in " + def.name + " class");
                if (!childGetter.flags.isOverride || !childSetter.flags.isOverride) throw Error("The overridden setter and getter of property" + baseProperty.name + " have to be marked as OVERRIDE in " + def.name + " class");
            } else {
                if (childGetter && !childGetter.flags.isOverride || childSetter && !childSetter.flags.isOverride) throw Error("The overridden getter or setter of property " + baseProperty.name + " have to be marked as OVERRIDE in " + def.name + " class");
            }
        }
        ria.__SYNTAX.validateClassDecl = function(def, baseClass) {
            def.base = def.base === null ? new ria.__SYNTAX.Tokenizer.RefToken(baseClass) : def.base;
            if (!ria.__SYNTAX.isDescendantOf(def.base.value, baseClass)) throw Error("Base class must be descendant of " + baseClass.__META.name);
            if (def.flags.isOverride) throw Error("Modifier OVERRIDE is not supported in classes");
            if (def.flags.isReadonly) throw Error("Modifier READONLY is not supported in classes");
            if (def.flags.isAbstract && def.flags.isFinal) throw Error("Class can not be ABSTRACT and FINAL simultaneously");
            var baseSyntaxMeta = ria.__SYNTAX.Registry.find(def.base.value.__META.name);
            if (baseSyntaxMeta.flags.isFinal) throw Error("Can NOT extend final class " + def.base.value.__META.name);
            baseSyntaxMeta.properties.forEach(function(baseProperty) {
                var childGetter = def.methods.filter(function(method) {
                    return method.name == baseProperty.getGetterName();
                }).pop(), childSetter = def.methods.filter(function(method) {
                    return method.name == baseProperty.getSetterName();
                }).pop();
                validateBaseClassPropertyDeclaration(baseProperty, childGetter, childSetter, def);
            });
            baseSyntaxMeta.methods.forEach(function(baseMethod) {
                if (baseMethod.name == "$") return;
                validateBaseClassMethodDeclaration(def, baseMethod);
            });
            var processedMethods = [];
            def.properties.forEach(function(property) {
                if (findParentProperty(def, property.name)) throw Error("There is defined property " + property.name + " in one of the base classes");
                validatePropertyDeclaration(property, def, processedMethods);
            });
            processedMethods.push("$");
            def.methods.forEach(function(method) {
                if (processedMethods.indexOf(method.name) >= 0) return;
                var baseSearchResult = findParentPropertyByGetterOrSetter(def, method.name);
                if (baseSearchResult.property) {
                    validateGetterSetterOverride(method, def, baseSearchResult, processedMethods);
                    return;
                }
                validateMethodDeclaration(def, method);
            });
        };
        ria.__SYNTAX.compileClass = function(name, def) {
            var processedMethods = [];
            var $$Def = def.methods.filter(function(_1) {
                return _1.name == "$$";
            }).pop();
            var $$ = $$Def ? $$Def.body.value : ria.__API.init;
            processedMethods.push("$$");
            var ClassProxy = function ClassProxy() {
                var _old_SELF = ria.__SYNTAX.Modifiers.SELF;
                try {
                    ria.__SYNTAX.Modifiers.SELF = ClassProxy;
                    return $$.call(undefined, this, ClassProxy, ClassProxy.prototype.$, arguments);
                } catch (e) {
                    throw new ria.__API.Exception("Error instantiating class " + name, e);
                } finally {
                    ria.__SYNTAX.Modifiers.SELF = _old_SELF;
                }
            };
            ria.__API.clazz(ClassProxy, name, def.base.value, def.ifcs.values, def.annotations.map(function(_) {
                return _.value;
            }), def.flags.isAbstract);
            def.properties.forEach(function(property) {
                compilePropertyDeclaration(property, ClassProxy, processedMethods);
            });
            compileCtorDeclaration(def, ClassProxy, processedMethods);
            def.methods.forEach(function(method) {
                if (processedMethods.indexOf(method.name) >= 0) return;
                compileMethodDeclaration(def, method, ClassProxy);
            });
            ria.__API.compile(ClassProxy);
            ria.__SYNTAX.Registry.registry(name, def);
            return ClassProxy;
        };
        ria.__SYNTAX.CLASS = function() {
            var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
            ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
            var name = ria.__SYNTAX.getFullName(def.name);
            var clazz = ria.__SYNTAX.compileClass(name, def);
            ria.__SYNTAX.isProtected(name) || ria.__SYNTAX.define(name, clazz);
            return clazz;
        };
        function BaseIsUndefined() {
            throw Error("BASE is supported only on method with OVERRIDE");
        }
        if (ria.__CFG.enablePipelineMethodCall) {
            ria.__API.addPipelineMethodCallStage("CallInit", function(body, meta, scope, callSession) {
                callSession.__OLD_SELF = ria.__SYNTAX.Modifiers.SELF;
                ria.__SYNTAX.Modifiers.SELF = body.__SELF;
                callSession.__OLD_BASE = window.BASE;
                var base = body.__BASE_BODY;
                window.BASE = base ? ria.__API.getPipelineMethodCallProxyFor(base, base.__META, scope) : BaseIsUndefined;
            });
            ria.__API.addPipelineMethodCallStage("CallFinally", function(body, meta, scope, callSession) {
                ria.__SYNTAX.Modifiers.SELF = callSession.__OLD_SELF;
                delete callSession.__OLD_SELF;
                window.BASE = callSession.__OLD_BASE;
                delete callSession.__OLD_BASE;
            });
        }
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        ria.__SYNTAX.validateDelegateDecl = function(def) {
            if (def.annotations.length) throw Error("Annotations are not supported in delegates");
            if (def.flags.isAbstract || def.flags.isOverride || def.flags.isFinal) throw Error("Modifiers are not supported in delegates");
            def.argsTypes.forEach(function(type) {
                if (type instanceof ria.__SYNTAX.Tokenizer.SelfToken) throw Error("Argument type can't be SELF in delegates");
            });
            if (def.retType instanceof ria.__SYNTAX.Tokenizer.SelfToken) throw Error("Return type can't be SELF in delegates");
        };
        ria.__SYNTAX.compileDelegate = function(name, def) {
            return ria.__API.delegate(name, def.retType.value, def.argsTypes.map(function(_) {
                return _.value;
            }), def.argsNames);
        };
        function DELEGATE() {
            var def = ria.__SYNTAX.parseMember(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
            ria.__SYNTAX.validateDelegateDecl(def);
            var name = ria.__SYNTAX.getFullName(def.name);
            var delegate = ria.__SYNTAX.compileDelegate(name, def);
            ria.__SYNTAX.isProtected(name) || ria.__SYNTAX.define(name, delegate);
            return delegate;
        }
        ria.__SYNTAX.DELEGATE = DELEGATE;
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        ria.__SYNTAX.validateEnumDecl = function(name, val) {
            if (typeof name !== "string") throw Error("String is expected as enum name");
            for (var prop in val) if (val.hasOwnProperty(prop)) {
                var value_ = val[prop];
                switch (typeof value_) {
                  case "number":
                  case "string":
                  case "boolean":
                    break;

                  default:
                    throw Error("Enum value should Number, String or Boolean, got " + typeof value_);
                }
            }
        };
        ria.__SYNTAX.compileEnum = function(name, val) {
            var values = {};
            function Enum(raw) {
                return values[raw];
            }
            ria.__API.enum(Enum, name);
            function EnumImpl(raw) {
                this.valueOf = function() {
                    return raw;
                };
                this.toString = function() {
                    return name + "#" + raw;
                };
            }
            ria.__API.extend(EnumImpl, Enum);
            for (var prop in val) if (val.hasOwnProperty(prop)) {
                var value_ = val[prop];
                values[value_] = Enum[prop] = new EnumImpl(value_);
            }
            Object.freeze(Enum);
            Object.freeze(values);
            return Enum;
        };
        ria.__SYNTAX.ENUM = function(n, val) {
            ria.__SYNTAX.validateEnumDecl(n, val);
            var name = ria.__SYNTAX.getFullName(n);
            var enumeration = ria.__SYNTAX.compileEnum(name, val);
            ria.__SYNTAX.isProtected(name) || ria.__SYNTAX.define(name, enumeration);
            return enumeration;
        };
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        ria.__SYNTAX.validateException = function(def) {
            ria.__SYNTAX.validateClassDecl(def, ria.__API.Exception);
            if (def.annotations.length) throw Error("Annotations are not supported in delegates");
            if (def.flags.isAbstract || def.flags.isOverride || def.flags.isFinal) throw Error("Modifiers are not supported in exceptions");
            if (!ria.__SYNTAX.isDescendantOf(def.base.value, ria.__API.Exception)) throw Error("Errors can extend only from other exceptions");
        };
        ria.__SYNTAX.EXCEPTION = function() {
            var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
            ria.__SYNTAX.validateException(def);
            var name = ria.__SYNTAX.getFullName(def.name);
            var exception = ria.__SYNTAX.compileClass(name, def);
            ria.__SYNTAX.isProtected(name) || ria.__SYNTAX.define(name, exception);
            return exception;
        };
    })();
    ria = ria || {};
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        ria.__SYNTAX.compileIdentifier = function(name) {
            var values = {};
            function IdentifierValue(value) {
                ria.__SYNTAX.checkArg("value", [ String, Number, Boolean ], value);
                return values.hasOwnProperty(value) ? values[value] : values[value] = new IdentifierValueImpl(value);
            }
            ria.__API.identifier(IdentifierValue, name);
            function IdentifierValueImpl(value) {
                this.valueOf = function() {
                    return value;
                };
                this.toString = function toString() {
                    return "[" + name + "#" + value + "]";
                };
                Object.freeze(this);
            }
            ria.__API.extend(IdentifierValueImpl, IdentifierValue);
            Object.freeze(IdentifierValue);
            Object.freeze(IdentifierValueImpl);
            return IdentifierValue;
        };
        ria.__SYNTAX.IDENTIFIER = function(n) {
            var name = ria.__SYNTAX.getFullName(n);
            var identifier = ria.__SYNTAX.compileIdentifier(name);
            ria.__SYNTAX.isProtected(name) || ria.__SYNTAX.define(name, identifier);
            return identifier;
        };
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        ria.__SYNTAX.validateInterfaceDecl = function(def) {
            if (def.flags.isFinal) throw Error("Interface can NOT be marked with FINAL");
            if (def.flags.isAbstract) throw Error("Interface can NOT be marked with ABSTRACT");
            if (def.flags.isOverride) throw Error("Interface can NOT be marked with OVERRIDE");
            if (def.flags.isReadonly) throw Error("Interface can NOT be marked with READONLY");
            if (def.annotations.length != 0) throw Error("Annotation are not supported on interfaces");
            def.methods.map(function(method) {
                if (method.flags.isAbstract || method.flags.isOverride || method.flags.isReadonly || method.flags.isFinal) throw Error("Interface method can NOT be marked with ABSTRACT, OVERRIDE, READONLY or FINAL");
            });
            def.properties.forEach(function(property) {
                if (property.flags.isAbstract || property.flags.isOverride || property.flags.isFinal) throw Error("Interface property can NOT be marked with ABSTRACT, OVERRIDE or FINAL");
            });
        };
        ria.__SYNTAX.compileInterface = function(name, def) {
            function InterfaceProxy() {
                var members = ria.__SYNTAX.parseMembers([].slice.call(arguments));
                var flags = {
                    isFinal: true
                };
                var properties = members.filter(function(_1) {
                    return _1 instanceof ria.__SYNTAX.PropertyDescriptor;
                });
                var methods = members.filter(function(_1) {
                    return _1 instanceof ria.__SYNTAX.MethodDescriptor;
                });
                var def = new ria.__SYNTAX.ClassDescriptor("$AnonymousClass", ria.__API.Class, [ InterfaceProxy ], flags, [], properties, methods);
                var impl = ria.__SYNTAX.buildClass("$AnonymousClass", def);
                return impl();
            }
            var methods = def.methods.map(function(method) {
                var types = method.argsTypes.map(function(_) {
                    return _ instanceof ria.__SYNTAX.Tokenizer.SelfToken ? InterfaceProxy : _.value;
                });
                return [ method.name, method.retType instanceof ria.__SYNTAX.Tokenizer.SelfToken ? InterfaceProxy : method.retType.value, types, method.argsNames ];
            });
            def.properties.forEach(function(property) {
                methods.push([ property.getGetterName(), property.type instanceof ria.__SYNTAX.Tokenizer.SelfToken ? InterfaceProxy : property.type.value, [], [] ]);
                if (property.flags.isReadonly) return;
                methods.push([ property.getSetterName(), undefined, [ property.type instanceof ria.__SYNTAX.Tokenizer.SelfToken ? InterfaceProxy : property.type.value ], [ "value" ] ]);
            });
            ria.__API.ifc(InterfaceProxy, name, methods);
            Object.freeze(InterfaceProxy);
            ria.__SYNTAX.Registry.registry(name, InterfaceProxy);
            return InterfaceProxy;
        };
        function INTERFACE() {
            var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
            ria.__SYNTAX.validateInterfaceDecl(def);
            var name = ria.__SYNTAX.getFullName(def.name);
            var ifc = ria.__SYNTAX.compileInterface(name, def);
            ria.__SYNTAX.isProtected(name) || ria.__SYNTAX.define(name, ifc);
            return ifc;
        }
        ria.__SYNTAX.INTERFACE = INTERFACE;
        if (ria.__CFG.enablePipelineMethodCall && ria.__CFG.checkedMode) {
            ria.__API.addPipelineMethodCallStage("BeforeCall", function(body, meta, scope, args, result, callSession) {});
            ria.__API.addPipelineMethodCallStage("AfterCall", function(body, meta, scope, args, result, callSession) {
                if (meta.ret && ria.__API.isInterface(meta.ret)) {
                    var fn = function AnonymousClass() {};
                }
                return result;
            });
        }
    })();
    ria.__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        function buildNs(ns, name) {
            return ns ? ns + "." + name : name;
        }
        function setPath(path, value) {
            var p = path.split(/\./);
            var root = window;
            var name = p.pop();
            while (p.length) {
                var n = p.shift();
                if (!root.hasOwnProperty(n)) Object.defineProperty(root, n, {
                    writable: false,
                    configurable: false,
                    value: {}
                });
                root = root[n];
            }
            if (!root.hasOwnProperty(name)) Object.defineProperty(root, name, {
                writable: false,
                configurable: false,
                value: value
            });
        }
        ria.__SYNTAX.getFullName = function(name) {
            return buildNs(CurrentNamespace, name);
        };
        ria.__SYNTAX.define = function(name, def) {
            setPath(name, def);
        };
        var CurrentNamespace = null;
        ria.__SYNTAX.NS = function(name, callback) {
            var old = CurrentNamespace;
            setPath(CurrentNamespace = name, {});
            callback();
            CurrentNamespace = old;
        };
    })();
    (ria = ria || {}).__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        ria.__SYNTAX.isProtected = function(name) {
            return /^.+_$/.test(name);
        };
        ria.__SYNTAX.parseModifiers = function(tkz) {
            ria.__SYNTAX.checkArg("tkz", [ ria.__SYNTAX.Tokenizer ], tkz);
            var flags = {
                isAbstract: false,
                isFinal: false,
                isOverride: false,
                isReadonly: false
            };
            while (!tkz.eot() && tkz.check(ria.__SYNTAX.Tokenizer.ModifierToken)) {
                switch (tkz.next().value) {
                  case ria.__SYNTAX.Modifiers.ABSTRACT:
                    flags.isAbstract = true;
                    break;

                  case ria.__SYNTAX.Modifiers.FINAL:
                    flags.isFinal = true;
                    break;

                  case ria.__SYNTAX.Modifiers.OVERRIDE:
                    flags.isOverride = true;
                    break;

                  case ria.__SYNTAX.Modifiers.READONLY:
                    flags.isReadonly = true;
                    break;
                }
            }
            return flags;
        };
        ria.__SYNTAX.parseAnnotations = function(tkz) {
            ria.__SYNTAX.checkArg("tkz", [ ria.__SYNTAX.Tokenizer ], tkz);
            var annotations = [];
            while (!tkz.eot() && tkz.check(ria.__SYNTAX.Tokenizer.ArrayToken)) {
                var a = tkz.next();
                if (a.values.length != 1) throw Error('Annotation expected, eg [SomeAnnotation] or [SomeAnnotationWithParams("some values here")], or check if annotation is loaded');
                annotations.push(a.values[0]);
            }
            return annotations;
        };
        function MethodDescriptor(name, argsNames, argsTypes, retType, flags, body, annotations) {
            this.name = name;
            this.argsNames = argsNames;
            this.argsTypes = argsTypes;
            this.retType = retType;
            this.body = body;
            this.annotations = annotations;
            this.flags = flags;
        }
        ria.__SYNTAX.MethodDescriptor = MethodDescriptor;
        function capitalize(str) {
            return str.replace(/\w/, function(_1) {
                return _1.toUpperCase();
            });
        }
        function PropertyDescriptor(name, type, annotations, flags) {
            this.name = name;
            this.type = type;
            this.annotations = annotations;
            this.flags = flags;
        }
        PropertyDescriptor.prototype.getGetterName = function() {
            return (this.type.value === Boolean ? "is" : "get") + capitalize(this.name);
        };
        PropertyDescriptor.prototype.getSetterName = function() {
            return "set" + capitalize(this.name);
        };
        ria.__SYNTAX.PropertyDescriptor = PropertyDescriptor;
        ria.__SYNTAX.parseMember = function(tkz) {
            ria.__SYNTAX.checkArg("tkz", [ ria.__SYNTAX.Tokenizer ], tkz);
            var annotations = ria.__SYNTAX.parseAnnotations(tkz);
            var argsHints = [];
            if (tkz.check(ria.__SYNTAX.Tokenizer.DoubleArrayToken)) argsHints = tkz.next().values;
            var flags = ria.__SYNTAX.parseModifiers(tkz);
            var retType = null;
            if (tkz.check(ria.__SYNTAX.Tokenizer.RefToken) || tkz.check(ria.__SYNTAX.Tokenizer.FunctionCallToken) || tkz.check(ria.__SYNTAX.Tokenizer.VoidToken) || tkz.check(ria.__SYNTAX.Tokenizer.SelfToken)) retType = tkz.next();
            if (tkz.check(ria.__SYNTAX.Tokenizer.StringToken)) return new PropertyDescriptor(tkz.next().value, retType, annotations, flags);
            tkz.ensure(ria.__SYNTAX.Tokenizer.FunctionToken);
            var body = tkz.next();
            return new MethodDescriptor(body.getName(), body.getParameters(), argsHints, retType, flags, body, annotations);
        };
        ria.__SYNTAX.parseMembers = function(tkz) {
            ria.__SYNTAX.checkArg("tkz", [ ria.__SYNTAX.Tokenizer ], tkz);
            var members = [];
            while (!tkz.eot()) members.push(ria.__SYNTAX.parseMember(tkz));
            return members;
        };
        function ClassDescriptor(name, base, ifcs, flags, annotations, properties, methods) {
            this.name = name;
            this.base = base;
            this.ifcs = ifcs;
            this.flags = flags;
            this.annotations = annotations;
            this.properties = properties;
            this.methods = methods;
        }
        ria.__SYNTAX.ClassDescriptor = ClassDescriptor;
        ria.__SYNTAX.parseClassDef = function(tkz) {
            ria.__SYNTAX.checkArg("tkz", [ ria.__SYNTAX.Tokenizer ], tkz);
            var annotations = ria.__SYNTAX.parseAnnotations(tkz);
            var flags = ria.__SYNTAX.parseModifiers(tkz);
            tkz.ensure(ria.__SYNTAX.Tokenizer.StringToken);
            var name = tkz.next().value;
            var base = null;
            if (tkz.check(ria.__SYNTAX.Tokenizer.ExtendsToken)) base = tkz.next();
            var ifcs = new ria.__SYNTAX.Tokenizer.ImplementsToken([]);
            if (tkz.check(ria.__SYNTAX.Tokenizer.ImplementsToken)) ifcs = tkz.next();
            tkz.ensure(ria.__SYNTAX.Tokenizer.ArrayToken);
            var members = ria.__SYNTAX.parseMembers(tkz.next().getTokenizer());
            if (!tkz.eot()) throw Error("Expected end of class declaration");
            var properties = members.filter(function(_1) {
                return _1 instanceof PropertyDescriptor;
            });
            var methods = members.filter(function(_1) {
                return _1 instanceof MethodDescriptor;
            });
            return new ClassDescriptor(name, base, ifcs, flags, annotations, properties, methods);
        };
        ria.__SYNTAX.isInstanceOf = function(object, constructor) {
            var o = object;
            while (o.__proto__ != null) {
                if (o.__proto__ === constructor.prototype) return true;
                o = o.__proto__;
            }
            return false;
        };
        ria.__SYNTAX.isDescendantOf = function(clazz, constructor) {
            var o = clazz;
            while (o.__META != null) {
                if (o === constructor) break;
                o = o.__META.base;
            }
            return o === constructor;
        };
    })();
    (ria = ria || {}).__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        var registry = {};
        ria.__SYNTAX.Registry = {};
        ria.__SYNTAX.Registry.find = function(name) {
            if ("string" !== typeof name) throw Error("String is only acceptable type for name");
            return registry.hasOwnProperty(name) ? registry[name] : null;
        };
        ria.__SYNTAX.Registry.registry = function(name, value) {
            if ("string" !== typeof name) throw Error("String is only acceptable type for name");
            registry[name] = value;
        };
    })();
    (ria = ria || {}).__SYNTAX = ria.__SYNTAX || {};
    (ria = ria || {}).__CFG = ria.__CFG || {};
    (function() {
        "use strict";
        var IS_OPTIONAL = /^.+_$/;
        function checkDelegate(value, type) {
            if ("function" !== typeof value) return false;
            var method = value.__META;
            if (method) {
                var delegate = type.__META;
                try {
                    method.argsNames.forEach(function(name, index) {
                        if (!IS_OPTIONAL.test(name)) {
                            if (delegate.argsNames[index] == undefined) {
                                throw new ria.__API.Exception("Lambda required arguments " + name + " that delegate does not supply");
                            }
                        }
                        if (!checkTypeHint(method.argsTypes[index], delegate.argsTypes[index])) {
                            throw new ria.__API.Exception("Lambda accepts " + ria.__API.getIdentifierOfType(method.argsTypes[index]) + " for argument " + name + ", but delegate supplies " + ria.__API.getIdentifierOfType(delegate.argsTypes[index]));
                        }
                    });
                } catch (e) {
                    throw new ria.__API.Exception("Delegate validation error", e);
                }
            }
            return true;
        }
        function checkTypeHint(value, type) {
            if (value === undefined) return false;
            if (value === null || type === Object) return true;
            switch (typeof value) {
              case "number":
                return type === Number;

              case "string":
                return type === String;

              case "boolean":
                return type === Boolean;

              default:
                if (value === Boolean || value === String || value === Number || value === Function || value === RegExp) {
                    return value == type;
                }
                if (ria.__API.isDelegate(type)) return checkDelegate(value, type);
                if (type === Function) return "function" === typeof value;
                if (type === ria.__API.Interface) {
                    return value === ria.__API.Interface || ria.__API.isInterface(value);
                }
                if (ria.__API.isInterface(type)) {
                    if (ria.__API.isInterface(value)) return value === type;
                    if (!(value instanceof ria.__API.Class)) return false;
                    return ria.__API.getConstructorOf(value).__META.ifcs.indexOf(type.valueOf()) >= 0;
                }
                if (ria.__API.isArrayOfDescriptor(type)) {
                    if (ria.__API.isArrayOfDescriptor(value)) return type.valueOf() == value.valueOf();
                    if (!Array.isArray(value)) return false;
                    for (var i = 0; i < value.length; i++) {
                        if (!checkTypeHint(value[i], type.valueOf())) return false;
                    }
                    return true;
                }
                if (ria.__API.isClassOfDescriptor(type)) {
                    if (ria.__API.isClassOfDescriptor(value)) return type.valueOf() == value.valueOf();
                    var v = value;
                    while (v) {
                        if (v === type.valueOf()) return true;
                        v = v.__META.base;
                    }
                    return false;
                }
                if (ria.__API.isImplementerOfDescriptor(type)) {
                    if (ria.__API.isImplementerOfDescriptor(value)) return type.valueOf() == value.valueOf();
                    if (!ria.__API.isClassConstructor(value)) return false;
                    return value.__META.ifcs.indexOf(type.valueOf()) >= 0;
                }
                return value === type || value instanceof type;
            }
        }
        ria.__SYNTAX.checkArg = function(name, type, value) {
            var isOptional = IS_OPTIONAL.test(name);
            if (isOptional && value === undefined) return;
            if (!isOptional && value === undefined) throw Error("Argument " + name + " is required");
            if (!Array.isArray(type)) type = [ type ];
            var error;
            var t = type.slice();
            while (t.length > 0) {
                var t_ = t.pop();
                try {
                    if (checkTypeHint(value, t_)) return;
                } catch (e) {
                    error = e;
                }
            }
            throw new ria.__API.Exception("Argument " + name + " expected to be " + type.map(ria.__API.getIdentifierOfType).join(" or ") + " but received " + ria.__API.getIdentifierOfValue(value), error);
        };
        ria.__SYNTAX.checkArgs = function(names, types, values) {
            if (values.length > names.length) throw Error("Too many arguments passed");
            for (var index = 0; index < names.length; index++) {
                ria.__SYNTAX.checkArg(names[index], types.length > index ? types[index] : Object, values[index]);
            }
        };
        ria.__SYNTAX.checkReturn = function(type, value) {
            if (type === null) return;
            if (type === undefined && value !== undefined) {
                throw Error("No return expected but got " + ria.__API.getIdentifierOfValue(value));
            }
            if (type !== undefined && !checkTypeHint(value, type)) throw Error("Expected return of " + ria.__API.getIdentifierOfType(type) + " but got " + ria.__API.getIdentifierOfValue(value));
        };
        if (ria.__CFG.enablePipelineMethodCall && ria.__CFG.checkedMode) {
            ria.__API.addPipelineMethodCallStage("BeforeCall", function(body, meta, scope, args) {
                try {
                    ria.__SYNTAX.checkArgs(meta.argsNames, meta.argsTypes, args);
                } catch (e) {
                    throw new ria.__API.Exception("Bad argument for " + meta.name, e);
                }
            });
            ria.__API.addPipelineMethodCallStage("AfterCall", function(body, meta, scope, args, result) {
                try {
                    ria.__SYNTAX.checkReturn(meta.ret, result);
                } catch (e) {
                    throw new ria.__API.Exception("Bad return of " + meta.name, e);
                }
                return result;
            });
        }
    })();
    (ria = ria || {}).__SYNTAX = ria.__SYNTAX || {};
    (function() {
        "use strict";
        function isBuildInType(type) {
            return type === Function || type === String || type === Boolean || type === Number || type === RegExp || type === Object || type === Array || type === Date;
        }
        function isCustomType(type) {
            return ria.__API.isClassConstructor(type) || ria.__API.isInterface(type) || ria.__API.isEnum(type) || ria.__API.isIdentifier(type) || ria.__API.isDelegate(type);
        }
        function isImportedType(type) {
            if (!type) throw Error("value expected");
            return false;
        }
        function isType(type) {
            return isBuildInType(type) || isCustomType(type) || isImportedType(type);
        }
        function parseName(fn) {
            return fn.name || (fn.toString().substring(9).match(/[a-z0-9_$]+/i) || [])[0] || "";
        }
        function getParameters(fn) {
            var body = fn.toString().substring(8);
            var start = body.indexOf("(");
            var params = body.substring(start + 1, body.indexOf(")", start));
            return params.length > 0 ? params.replace(/\s+/g, "").split(",") : [];
        }
        var Modifiers = function() {
            function Modifiers() {
                throw Error();
            }
            ria.__API.enum(Modifiers, "Modifiers");
            function ModifiersImpl(raw) {
                this.valueOf = function() {
                    return raw;
                };
            }
            ria.__API.extend(ModifiersImpl, Modifiers);
            Modifiers.OVERRIDE = new ModifiersImpl("OVERRIDE");
            Modifiers.ABSTRACT = new ModifiersImpl("ABSTRACT");
            Modifiers.VOID = new ModifiersImpl("VOID");
            Modifiers.SELF = new ModifiersImpl("SELF");
            Modifiers.FINAL = new ModifiersImpl("FINAL");
            Modifiers.READONLY = new ModifiersImpl("READONLY");
            return Modifiers;
        }();
        ria.__SYNTAX.Modifiers = Modifiers;
        function FunctionToken(value) {
            this.value = value;
        }
        FunctionToken.prototype.getName = function() {
            return parseName(this.value);
        };
        FunctionToken.prototype.getParameters = function() {
            return getParameters(this.value);
        };
        function FunctionCallToken(token) {
            throw Error("This token type can not be detected at RtDebug");
        }
        function StringToken(str) {
            this.value = str;
            this.raw = str;
        }
        function RefToken(ref) {
            this.value = ref;
            this.raw = ref;
        }
        function ModifierToken(mod) {
            this.value = mod;
        }
        function ArrayToken(value, raw) {
            this.values = value;
            this.raw = raw;
        }
        ArrayToken.prototype.getTokenizer = function() {
            return new Tokenizer(this.raw);
        };
        function DoubleArrayToken(value, raw) {
            this.values = value;
            this.raw = raw;
        }
        function VoidToken() {}
        function SelfToken() {}
        function ExtendsToken(base) {
            this.value = base;
            this.raw = base;
        }
        function ImplementsToken(ifcs) {
            this.raw = this.values = [].slice.call(ifcs);
        }
        function ExtendsDescriptor(base) {
            this.base = base;
        }
        ria.__SYNTAX.ExtendsDescriptor = ExtendsDescriptor;
        ria.__SYNTAX.EXTENDS = function(base) {
            if (base === undefined) throw Error("Class expected, but got undefined. Check if it is defined already");
            if (!(base.__META instanceof ria.__API.ClassDescriptor)) throw Error("Class expected, but got " + ria.__API.getIdentifierOfType(base));
            return new ExtendsDescriptor(base);
        };
        function ImplementsDescriptor(ifcs) {
            this.ifcs = ifcs;
        }
        ria.__SYNTAX.ImplementsDescriptor = ImplementsDescriptor;
        ria.__SYNTAX.IMPLEMENTS = function() {
            var ifcs = [].slice.call(arguments);
            if (ifcs.length < 1) throw Error("Interfaces expected, but got nothing");
            for (var index = 0; index < ifcs.length; index++) {
                var ifc = ifcs[index];
                if (ifc === undefined) throw Error("Interface expected, but got undefined. Check if it is defined already");
                if (!(ifc.__META instanceof ria.__API.InterfaceDescriptor)) throw Error("Interface expected, but got " + ria.__API.getIdentifierOfType(ifc));
            }
            return new ImplementsDescriptor(ifcs);
        };
        function Tokenizer(data) {
            this.token = this.token.bind(this);
            this.data = [].slice.call(data).map(this.token);
        }
        Tokenizer.prototype.token = function(token) {
            if (token instanceof Modifiers) {
                if (token == Modifiers.VOID) return new VoidToken();
                if (token == Modifiers.SELF) return new SelfToken();
                return new ModifierToken(token);
            }
            if (token instanceof ExtendsDescriptor) return new ExtendsToken(token.base);
            if (token instanceof ImplementsDescriptor) return new ImplementsToken(token.ifcs);
            if (Array.isArray(token) && token.length == 1 && Array.isArray(token[0])) return new DoubleArrayToken(token[0].map(this.token), token);
            if (Array.isArray(token)) return new ArrayToken(token.map(this.token), token);
            if (typeof token === "function" && !isType(token)) return new FunctionToken(token);
            if (typeof token === "string") return new StringToken(token);
            if (typeof token === "function") return new RefToken(token);
            if (typeof token === "object") return new RefToken(token);
            throw Error("Unexpected token, type: " + typeof token);
        };
        Tokenizer.prototype.check = function(type) {
            return this.data[0] instanceof type;
        };
        Tokenizer.prototype.next = function() {
            return this.data.shift();
        };
        Tokenizer.prototype.ensure = function(type) {
            if (!this.check(type)) throw Error("Expected " + type.name);
        };
        Tokenizer.prototype.eot = function() {
            return this.data.length < 1;
        };
        Tokenizer.FunctionToken = FunctionToken;
        Tokenizer.FunctionCallToken = FunctionCallToken;
        Tokenizer.StringToken = StringToken;
        Tokenizer.RefToken = RefToken;
        Tokenizer.ModifierToken = ModifierToken;
        Tokenizer.ArrayToken = ArrayToken;
        Tokenizer.DoubleArrayToken = DoubleArrayToken;
        Tokenizer.VoidToken = VoidToken;
        Tokenizer.SelfToken = SelfToken;
        Tokenizer.ExtendsToken = ExtendsToken;
        Tokenizer.ImplementsToken = ImplementsToken;
        ria.__SYNTAX.Tokenizer = Tokenizer;
    })();
    (function() {
        "use strict";
        var ClassMeta = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([ "Class", [ function $() {}, Function, function getClass() {}, String, function getHashCode() {}, [ [ ria.__API.Class ] ], Boolean, function equals(other) {} ] ]));
        ria.__SYNTAX.Registry.registry("Class", ClassMeta);
        var ExceptionMeta = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([ "Exception", [ [ [ String, Object ] ], function $(msg, inner_) {}, String, function toString() {}, String, function getMessage() {}, Array, function getStack() {} ] ]));
        ria.__SYNTAX.Registry.registry("Exception", ExceptionMeta);
    })();
    __ASSETS._mfwd7z7vjagaatt9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                id: "home-logo"
            },
            escaped: {}
        }, "district", "list");
        buf.push('<div id="sidebar-controls"><div class="search-wrapper"><div id="search-bar"></div></div><div class="wrapper"><div class="buttons-row">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Districts");
            },
            attributes: {
                title: "View Districts",
                "class": "one-third-button" + " " + "people"
            },
            escaped: {
                title: true
            }
        }, "district", "list");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Apps");
            },
            attributes: {
                title: "View Applications",
                "class": "one-third-button" + " " + "apps"
            },
            escaped: {
                title: true
            }
        }, "apps", "list");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Settings");
            },
            attributes: {
                title: "View System Types",
                "class": "one-third-button" + " " + "settings"
            },
            escaped: {
                title: true
            }
        }, "settings", "dashboard");
        buf.push('</div><div class="buttons-row">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Purchase Order");
            },
            attributes: {
                title: "View Purchase Order",
                "class": "one-third-button" + " " + "statistic"
            },
            escaped: {
                title: true
            }
        }, "funds", "list");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Signups");
            },
            attributes: {
                title: "View Signups",
                "class": "one-third-button"
            },
            escaped: {
                title: true
            }
        }, "signup", "list");
        buf.push('</div></div></div><div id="invite-button-container"></div>');
        return buf.join("");
    };
    __ASSETS._tsj822rosin0o1or = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        currentUserName = self.getCurrentPerson().getDisplayName();
        jade.globals.Logout_mixin.call({
            buf: buf
        }, "account", "logout", currentUserName);
        return buf.join("");
    };
    (function() {
        (ria = ria || {}).reflection = ria.reflection || {};
        (function() {
            "use strict";
            ria.reflection.Reflector = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.reflection." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.ABSTRACT, "Reflector", [ ria.__SYNTAX.Modifiers.ABSTRACT, Array, function getAnnotations() {}, Boolean, function isAnnotatedWith(ann) {
                return this.getAnnotations().some(function(_) {
                    return _.__META == ann.__META;
                });
            }, Array, function findAnnotation(ann) {
                return this.getAnnotations().filter(function(_) {
                    return _.__META == ann.__META;
                });
            } ]);
        })();
    })();
    "ria.reflection.Reflector";
    (function() {
        (ria = ria || {}).reflection = ria.reflection || {};
        (function() {
            "use strict";
            ria.reflection.ReflectionCtor = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.reflection." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.FINAL, "ReflectionCtor", ria.__SYNTAX.EXTENDS(ria.reflection.Reflector), [ ria.__SYNTAX.Modifiers.READONLY, ria.__API.ClassOf(ria.__API.Class), "clazz", ria.__SYNTAX.Modifiers.READONLY, String, "name", [ [ ria.__API.ClassOf(ria.__API.Class) ] ], function $(clazz) {
                this.clazz = clazz;
                this.method = clazz.__META.ctor;
                this.name = "ctor";
            }, String, function getName() {
                return this.clazz.__META.name + "#" + this.name;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Array, function getAnnotations() {
                return this.method.annotations;
            }, ria.__API.ArrayOf(String), function getArguments() {
                return this.method.argsNames;
            }, ria.__API.ArrayOf(String), function getRequiredArguments() {
                return this.getArguments().filter(function(_) {
                    return !/^.+_$/.test(_);
                });
            }, Array, function getArgumentsTypes() {
                return this.method.argsTypes;
            } ]);
        })();
    })();
    "ria.reflection.Reflector";
    (function() {
        (ria = ria || {}).reflection = ria.reflection || {};
        (function() {
            "use strict";
            ria.reflection.ReflectionMethod = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.reflection." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.FINAL, "ReflectionMethod", ria.__SYNTAX.EXTENDS(ria.reflection.Reflector), [ ria.__SYNTAX.Modifiers.READONLY, ria.__API.ClassOf(ria.__API.Class), "clazz", ria.__SYNTAX.Modifiers.READONLY, String, "name", [ [ ria.__API.ClassOf(ria.__API.Class), String ] ], function $(clazz, name) {
                this.clazz = clazz;
                this.method = clazz.__META.methods[name];
                this.name = name;
            }, String, function getName() {
                return this.clazz.__META.name + "#" + this.name;
            }, String, function getShortName() {
                return this.name;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Array, function getAnnotations() {
                return this.method.annotations;
            }, Object, function getReturnType() {
                return this.method.retType;
            }, ria.__API.ArrayOf(String), function getArguments() {
                return this.method.argsNames;
            }, ria.__API.ArrayOf(String), function getRequiredArguments() {
                return this.getArguments().filter(function(_) {
                    return !/^.+_$/.test(_);
                });
            }, Array, function getArgumentsTypes() {
                return this.method.argsTypes;
            }, function invokeOn(instance, args_) {
                null;
                null;
                _DEBUG && (instance = instance.__PROTECTED || instance);
                return this.method.impl.apply(instance, args_ || []);
            } ]);
        })();
    })();
    "ria.reflection.Reflector";
    (function() {
        (ria = ria || {}).reflection = ria.reflection || {};
        (function() {
            "use strict";
            ria.reflection.ReflectionProperty = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.reflection." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.FINAL, "ReflectionProperty", ria.__SYNTAX.EXTENDS(ria.reflection.Reflector), [ ria.__SYNTAX.Modifiers.READONLY, ria.__API.ClassOf(ria.__API.Class), "clazz", ria.__SYNTAX.Modifiers.READONLY, String, "name", [ [ ria.__API.ClassOf(ria.__API.Class), String ] ], function $(clazz, name) {
                this.clazz = clazz;
                this.property = clazz.__META.properties[name];
                this.name = name;
            }, String, function getName() {
                return this.clazz.__META.name + "#" + this.name;
            }, String, function getShortName() {
                return this.name;
            }, Boolean, function isReadonly() {
                return this.property.setter == undefined;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Array, function getAnnotations() {
                return this.property.annotations;
            }, Object, function getType() {
                return this.property.retType;
            }, function invokeGetterOn(instance) {
                null;
                _DEBUG && (instance = instance.__PROTECTED || instance);
                return this.property.getter.call(instance);
            }, ria.__SYNTAX.Modifiers.VOID, function invokeSetterOn(instance, value) {
                null;
                null;
                _DEBUG && (instance = instance.__PROTECTED || instance);
                this.property.setter.call(instance, value);
            } ]);
        })();
    })();
    "ria.reflection.Reflector";
    "ria.reflection.ReflectionMethod";
    (function() {
        (ria = ria || {}).reflection = ria.reflection || {};
        (function() {
            "use strict";
            ria.reflection.ReflectionInterface = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.reflection." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.FINAL, "ReflectionInterface", ria.__SYNTAX.EXTENDS(ria.reflection.Reflector), [ ria.__SYNTAX.Modifiers.READONLY, ria.__API.Interface, "ifc", [ [ ria.__API.Interface ] ], function $(ifc) {
                this.ifc = ifc;
            }, String, function getName() {
                return this.ifc.__META.name;
            }, Array, function getMethodsNames() {
                return Object.keys(this.ifc.__META.methods);
            }, Object, function getMethodInfo(name) {
                return this.ifc.__META.methods[name] || null;
            }, function getMethodReturnType(name) {
                return this.ifc.__META.methods[name].retType;
            }, ria.__API.ArrayOf(String), function getMethodArguments(name) {
                return this.ifc.__META.methods[name].argsNames;
            }, ria.__API.ArrayOf(Object), function getMethodArgumentsTypes(name) {
                return this.ifc.__META.methods[name].argsTypes;
            }, Boolean, function hasMethod(name) {
                return this.ifc.__META.methods.hasOwnProperty(name);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Array, function getAnnotations() {
                return [];
            } ]);
        })();
    })();
    "ria.reflection.Reflector";
    "ria.reflection.ReflectionCtor";
    "ria.reflection.ReflectionMethod";
    "ria.reflection.ReflectionProperty";
    "ria.reflection.ReflectionInterface";
    (function() {
        (ria = ria || {}).reflection = ria.reflection || {};
        (function() {
            "use strict";
            var cache = {};
            ria.reflection.ReflectionClass = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.reflection." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.FINAL, "ReflectionClass", ria.__SYNTAX.EXTENDS(ria.reflection.Reflector), [ function $$(instance, Clazz, ctor, args) {
                var clazz = args[0];
                if (clazz instanceof ria.__API.Class) clazz = clazz.getClass();
                if (clazz instanceof ria.__API.ClassDescriptor) clazz = clazz.ctor;
                if (!ria.__API.isClassConstructor(clazz)) throw new ria.reflection.Exception("ReflectionFactory works only on CLASS");
                var name = clazz.__META.name;
                if (cache.hasOwnProperty(name)) return cache[name];
                return cache[name] = new ria.__API.init(instance, Clazz, ctor, args);
            }, ria.__SYNTAX.Modifiers.READONLY, ria.__API.ClassOf(ria.__API.Class), "clazz", [ [ ria.__API.ClassOf(ria.__API.Class) ] ], function $(clazz) {
                this.clazz = clazz;
            }, String, function getName() {
                return this.clazz.__META.name;
            }, String, function getShortName() {
                return this.clazz.__META.name.split(".").pop();
            }, Boolean, function isAbstract() {
                return this.clazz.__META.isAbstract;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Array, function getAnnotations() {
                return this.clazz.__META.anns;
            }, ria.__API.ClassOf(ria.__API.Class), function getBaseClass() {
                return this.clazz.__META.base || null;
            }, ria.__SYNTAX.Modifiers.SELF, function getBaseClassReflector() {
                var base = this.getBaseClass();
                return base ? new ria.__SYNTAX.Modifiers.SELF(base) : null;
            }, ria.__API.ArrayOf(ria.__API.Interface), function getInterfaces() {
                return this.clazz.__META.ifcs.slice();
            }, ria.__API.ArrayOf(ria.reflection.ReflectionInterface), function getInterfacesReflector() {
                return this.getInterfaces().map(function(_) {
                    return new ria.reflection.ReflectionInterface(_);
                }.bind(this));
            }, ria.__API.ArrayOf(String), function getMethodsNames() {
                return Object.keys(this.clazz.__META.methods);
            }, [ [ String ] ], ria.reflection.ReflectionMethod, function getMethodReflector(name) {
                var method = this.clazz.__META.methods[name];
                return method ? new ria.reflection.ReflectionMethod(this.clazz, name) : null;
            }, ria.__API.ArrayOf(ria.reflection.ReflectionMethod), function getMethodsReflector() {
                return this.getMethodsNames().map(function(_) {
                    return this.getMethodReflector(_);
                }.bind(this));
            }, ria.__API.ArrayOf(String), function getPropertiesNames() {
                return Object.keys(this.clazz.__META.properties);
            }, [ [ String ] ], ria.reflection.ReflectionProperty, function getPropertyReflector(name) {
                var property = this.clazz.__META.properties[name];
                return property ? new ria.reflection.ReflectionProperty(this.clazz, name) : null;
            }, ria.__API.ArrayOf(ria.reflection.ReflectionProperty), function getPropertiesReflector() {
                return this.getPropertiesNames().map(function(_) {
                    return this.getPropertyReflector(_);
                }.bind(this));
            }, ria.__API.ArrayOf(ria.__API.ClassOf(ria.__API.Class)), function getChildren() {
                return this.clazz.__META.children.slice();
            }, ria.__API.ArrayOf(ria.__SYNTAX.Modifiers.SELF), function getChildrenReflector() {
                return this.getChildren().map(function(_) {
                    return new ria.__SYNTAX.Modifiers.SELF(_);
                }.bind(this));
            }, ria.reflection.ReflectionCtor, function getCtorReflector() {
                return new ria.reflection.ReflectionCtor(this.clazz);
            }, ria.__API.ArrayOf(ria.__API.ClassOf(ria.__API.Class)), function getParents() {
                var parents = [];
                var root = this.getBaseClass();
                while (root != null) {
                    parents.push(root);
                    root = root.__META.base;
                }
                return parents;
            }, ria.__API.ArrayOf(ria.__SYNTAX.Modifiers.SELF), function getParentsReflector() {
                return this.getParents().map(function(_) {
                    return new ria.__SYNTAX.Modifiers.SELF(_);
                }.bind(this));
            }, Boolean, function extendsClass(parent) {
                return this.clazz == parent || this.getParents().some(function(_) {
                    return _ == parent;
                });
            }, Boolean, function implementsIfc(ifc) {
                if (!ria.__API.isInterface(ifc)) throw ria.reflection.Exception("Interface expected, but got " + ria.__API.getIdentifierOfType(ifc));
                return this.getInterfaces().some(function(_) {
                    return _ === ifc;
                });
            }, [ [ String ] ], Boolean, function hasProperty(name) {
                return this.clazz.__META.properties.hasOwnProperty(name);
            }, [ [ String ] ], Boolean, function hasMethod(name) {
                return this.clazz.__META.methods.hasOwnProperty(name);
            }, [ [ Array ] ], ria.__API.Class, function instantiate(args_) {
                return ria.__API.init(null, this.clazz, this.clazz.__META.ctor.impl, args_ ? args_ : []);
            } ]);
        })();
    })();
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.MvcException = function ExceptionCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateException(def);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("MvcException", [ [ [ String, Object ] ], function $(message, inner_) {
                BASE(message, inner_);
            } ]);
        })();
    })();
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            "use strict";
            ria.async.ICancelable = ria.__API.ifc(function() {}, "ria.async.ICancelable", [ [ "cancel", null, [], [] ] ]);
        })();
    })();
    "ria.async.ICancelable";
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            "use strict";
            function DefaultDataHanlder(data) {
                return data;
            }
            function DefaultErrorHandler(e) {
                throw e;
            }
            function FutureBreaker() {}
            ria.async.BREAK = new FutureBreaker();
            Object.defineProperty(ria.async, "BREAK", {
                writable: false,
                configurable: false,
                enumerable: true
            });
            ria.async.FutureDataDelegate = ria.__API.delegate("ria.async.FutureDataDelegate", Object, [ Object ], [ "data" ]);
            ria.async.FutureProgressDelegate = ria.__API.delegate("ria.async.FutureProgressDelegate", Object, [ Object ], [ "data" ]);
            ria.async.FutureErrorDelegate = ria.__API.delegate("ria.async.FutureErrorDelegate", Object, [ Object ], [ "error" ]);
            ria.async.FutureCompleteDelegate = ria.__API.delegate("ria.async.FutureCompleteDelegate", null, [], []);
            ria.async.Future = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.async." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Future", ria.__SYNTAX.IMPLEMENTS(ria.async.ICancelable), [ [ [ ria.async.ICancelable ] ], function $(canceler_) {
                this._canceler = canceler_;
                this._next = null;
                this._onData = null;
                this._onProgress = null;
                this._onError = null;
                this._onComplete = null;
            }, ria.__SYNTAX.Modifiers.VOID, function cancel() {
                this._canceler && this._canceler.cancel();
            }, [ [ ria.async.FutureDataDelegate, Object ] ], ria.__SYNTAX.Modifiers.SELF, function then(handler, scope_) {
                this._onData = handler.bind(scope_);
                return this.attach(new ria.__SYNTAX.Modifiers.SELF());
            }, [ [ ria.async.FutureProgressDelegate, Object ] ], ria.__SYNTAX.Modifiers.SELF, function handleProgress(handler, scope_) {
                this._onProgress = handler.bind(scope_);
                return this.attach(new ria.__SYNTAX.Modifiers.SELF());
            }, [ [ ria.async.FutureErrorDelegate, Object ] ], ria.__SYNTAX.Modifiers.SELF, function catchError(handler, scope_) {
                this._onError = handler.bind(scope_);
                return this.attach(new ria.__SYNTAX.Modifiers.SELF());
            }, [ [ ria.__API.ClassOf(ria.__API.Exception), ria.async.FutureErrorDelegate, Object ] ], ria.__SYNTAX.Modifiers.SELF, function catchException(exception, handler, scope_) {
                var me = this;
                this._onError = function(error) {
                    if (error instanceof exception) return handler.call(scope_, error);
                    throw error;
                };
                return this.attach(new ria.__SYNTAX.Modifiers.SELF());
            }, [ [ ria.async.FutureCompleteDelegate, Object ] ], ria.__SYNTAX.Modifiers.SELF, function complete(handler, scope_) {
                this._onComplete = handler.bind(scope_);
                return this.attach(new ria.__SYNTAX.Modifiers.SELF());
            }, [ [ String, Object ] ], function doCallNext_(method, arg_) {
                if (!this._next) return;
                var next_protected = this._next.__PROTECTED || this._next;
                var args = [];
                arg_ !== undefined && args.push(arg_);
                return next_protected[method].apply(next_protected, args);
            }, ria.__SYNTAX.Modifiers.VOID, function updateProgress_(data) {
                ria.__API.defer(this, function() {
                    try {
                        this._onProgress && this._onProgress(data);
                    } finally {
                        this.doCallNext_("updateProgress_", data);
                    }
                });
            }, ria.__SYNTAX.Modifiers.VOID, function complete_(data) {
                ria.__API.defer(this, function() {
                    try {
                        var result = (this._onData || DefaultDataHanlder).call(this, data);
                        if (result === ria.async.BREAK) {
                            this.doCallNext_("completeBreak_");
                        } else if (result instanceof ria.async.Future) {
                            this.attach(result);
                        } else {
                            this.doCallNext_("complete_", result === undefined ? null : result);
                        }
                    } catch (e) {
                        this.doCallNext_("completeError_", e);
                    } finally {
                        this._onComplete && this._onComplete();
                    }
                });
            }, ria.__SYNTAX.Modifiers.VOID, function completeError_(error) {
                if (!this._next) throw error;
                ria.__API.defer(this, function() {
                    try {
                        var result = (this._onError || DefaultErrorHandler).call(this, error);
                        this.doCallNext_("complete_", result === undefined ? null : result);
                    } catch (e) {
                        this.doCallNext_("completeError_", e);
                    } finally {
                        this._onComplete && this._onComplete();
                    }
                });
            }, ria.__SYNTAX.Modifiers.VOID, function completeBreak_() {
                ria.__API.defer(this, function() {
                    try {
                        this._onComplete && this._onComplete();
                    } finally {
                        this.doCallNext_("completeBreak_");
                    }
                });
            }, [ [ ria.async.ICancelable ] ], ria.__SYNTAX.Modifiers.VOID, function setCanceler_(canceler) {
                this._canceler = canceler;
            }, [ [ ria.__SYNTAX.Modifiers.SELF ] ], ria.__SYNTAX.Modifiers.SELF, function attach(future) {
                var old_next = this._next;
                this._next = future;
                this.doCallNext_("setCanceler_", this);
                return this.attachEnd_(old_next || null);
            }, [ [ ria.__SYNTAX.Modifiers.SELF ] ], ria.__SYNTAX.Modifiers.SELF, function attachEnd_(future) {
                return this._next ? this.doCallNext_("attachEnd_", future) : future ? this._next = future : this;
            } ]);
            ria.async.DeferredAction = ria.async.DeferredData = function(data_, delay_) {
                var future = new ria.async.Future();
                ria.__API.defer(null, (future.__PROTECTED || future).complete_, [ data_ || null ], delay_ | 0);
                return future;
            };
        })();
    })();
    "ria.async.Future";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.ActivityGroup = ria.__API.annotation("ria.mvc.ActivityGroup", [], [ "name" ]);
            ria.mvc.ActivityClosedEvent = ria.__API.delegate("ria.mvc.ActivityClosedEvent", null, [ Object ], [ "activity" ]);
            ria.mvc.ActivityRefreshedEvent = ria.__API.delegate("ria.mvc.ActivityRefreshedEvent", null, [ Object, Object, String ], [ "activity", "model", "msg_" ]);
            ria.mvc.IActivity = ria.__API.ifc(function() {}, "ria.mvc.IActivity", [ [ "show", null, [], [] ], [ "pause", null, [], [] ], [ "stop", null, [], [] ], [ "isForeground", Boolean, [], [] ], [ "isStarted", Boolean, [], [] ], [ "close", null, [], [] ], [ "addCloseCallback", null, [ ria.mvc.ActivityClosedEvent ], [ "callback" ] ], [ "refresh", null, [ Object ], [ "model" ] ], [ "refreshD", null, [ ria.async.Future ], [ "model" ] ], [ "partialRefreshD", null, [ ria.async.Future ], [ "model" ] ], [ "addRefreshCallback", null, [ ria.mvc.ActivityRefreshedEvent ], [ "callback" ] ] ]);
        })();
    })();
    "ria.mvc.IActivity";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.IView = ria.__API.ifc(function() {}, "ria.mvc.IView", [ [ "push", null, [ ria.mvc.IActivity ], [ "activity" ] ], [ "pushD", null, [ ria.mvc.IActivity, ria.async.Future ], [ "activity", "data" ] ], [ "shade", null, [ ria.mvc.IActivity ], [ "activity" ] ], [ "shadeD", null, [ ria.mvc.IActivity, ria.async.Future ], [ "activity", "data" ] ], [ "pop", ria.mvc.IActivity, [], [] ], [ "updateD", null, [ ria.__API.ImplementerOf(ria.mvc.IActivity), ria.async.Future ], [ "activityClass", "data" ] ], [ "getCurrent", ria.mvc.IActivity, [], [] ], [ "reset", null, [], [] ], [ "onActivityRefreshed", null, [ ria.mvc.ActivityRefreshedEvent ], [ "callback" ] ] ]);
        })();
    })();
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.ISession = ria.__API.ifc(function() {}, "ria.mvc.ISession", [ [ "get", Object, [ String, Object ], [ "key", "def_" ] ], [ "set", null, [ String, Object, Boolean ], [ "key", "value", "isPersistent_" ] ], [ "remove", null, [ String ], [ "key" ] ] ]);
        })();
    })();
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.State = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("State", [ String, "controller", String, "action", Array, "params", Boolean, "public", Boolean, "dispatched", function $() {
                this.dispatched = false;
                this.public = false;
            } ]);
        })();
    })();
    "ria.mvc.IView";
    "ria.mvc.ISession";
    "ria.mvc.State";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.IContext = ria.__API.ifc(function() {}, "ria.mvc.IContext", [ [ "getState", ria.mvc.State, [], [] ], [ "getDefaultView", ria.mvc.IView, [], [] ], [ "getSession", ria.mvc.ISession, [], [] ], [ "getService", ria.__API.Class, [ ria.__API.ClassOf(ria.__API.Class) ], [ "clazz" ] ], [ "stateUpdated", null, [], [] ] ]);
        })();
    })();
    "ria.mvc.IContext";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.IContextable = ria.__API.ifc(function() {}, "ria.mvc.IContextable", []);
        })();
    })();
    "ria.mvc.State";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.IStateSerializer = ria.__API.ifc(function() {}, "ria.mvc.IStateSerializer", [ [ "serialize", String, [ ria.mvc.State ], [ "state" ] ], [ "deserialize", ria.mvc.State, [ String ], [ "value" ] ] ]);
        })();
    })();
    "ria.mvc.State";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.IDispatchPlugin = ria.__API.ifc(function() {}, "ria.mvc.IDispatchPlugin", [ [ "dispatchStartup", null, [], [] ], [ "preDispatch", null, [ ria.mvc.State ], [ "state" ] ], [ "postDispatch", null, [ ria.mvc.State ], [ "state" ] ], [ "dispatchShutdown", null, [], [] ] ]);
        })();
    })();
    (function() {
        (ria = ria || {}).serialize = ria.serialize || {};
        (function() {
            ria.serialize.Exception = function ExceptionCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateException(def);
                var name = "ria.serialize." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Exception", [ function $(msg, inner_) {
                BASE(msg, inner_);
            } ]);
        })();
    })();
    (function() {
        (ria = ria || {}).serialize = ria.serialize || {};
        (function() {
            "use strict";
            ria.serialize.SerializeProperty = ria.__API.annotation("ria.serialize.SerializeProperty", [], [ "name" ]);
        })();
    })();
    (function() {
        (ria = ria || {}).serialize = ria.serialize || {};
        (function() {
            "use strict";
            ria.serialize.IDeserializable = ria.__API.ifc(function() {}, "ria.serialize.IDeserializable", [ [ "deserialize", null, [], [ "raw" ] ] ]);
        })();
    })();
    (function() {
        (ria = ria || {}).serialize = ria.serialize || {};
        (function() {
            "use strict";
            ria.serialize.ISerializable = ria.__API.ifc(function() {}, "ria.serialize.ISerializable", [ [ "serialize", Object, [], [] ] ]);
        })();
    })();
    (function() {
        (ria = ria || {}).serialize = ria.serialize || {};
        (function() {
            ria.serialize.ISerializer = ria.__API.ifc(function() {}, "ria.serialize.ISerializer", [ [ "serialize", Object, [], [ "object" ] ], [ "deserialize", Object, [], [ "raw", "clazz" ] ] ]);
        })();
    })();
    "ria.reflection.ReflectionClass";
    "ria.serialize.Exception";
    "ria.serialize.SerializeProperty";
    "ria.serialize.IDeserializable";
    "ria.serialize.ISerializable";
    "ria.serialize.ISerializer";
    (function() {
        (ria = ria || {}).serialize = ria.serialize || {};
        (function() {
            "use strict";
            function isValue(_) {
                return _ !== null && _ !== undefined;
            }
            ria.serialize.JsonSerializer = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.serialize." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("JsonSerializer", ria.__SYNTAX.IMPLEMENTS(ria.serialize.ISerializer), [ Object, function serialize(object) {
                throw Error("Not implemented");
            }, Object, function deserialize(raw, clazz) {
                var value;
                if (clazz === Object) return raw || {};
                if (clazz === Array && (Array.isArray(raw) || raw == null)) return raw || [];
                if (clazz === Boolean) {
                    if (raw === "false") raw = false;
                    return clazz(raw || "");
                }
                if (clazz === Number || clazz === String) {
                    if (clazz === Number && raw === "") return null;
                    return clazz(raw || "");
                }
                if (ria.__API.isIdentifier(clazz)) return raw !== undefined ? clazz(raw) : null;
                if (ria.__API.isEnum(clazz)) {
                    if (raw === null || raw === undefined) return null;
                    value = clazz(raw);
                    if (value == undefined) throw new ria.serialize.Exception('Unknown value "' + JSON.stringify(raw) + '" of enum ' + clazz.__IDENTIFIER__);
                    return value;
                }
                var deserialize = this.deserialize;
                if (ria.__API.isArrayOfDescriptor(clazz)) {
                    if (raw === null || raw === undefined) return [];
                    if (!Array.isArray(raw)) throw new ria.serialize.Exception("Value expected to be array, but got: " + JSON.stringify(raw));
                    var type = clazz.valueOf();
                    return raw.filter(isValue).map(function(_, i) {
                        try {
                            return deserialize(_, type);
                        } catch (e) {
                            throw new ria.serialize.Exception("Error deserializing " + clazz + " value with index " + i, e);
                        }
                    });
                }
                if (ria.__API.isClassConstructor(clazz)) {
                    if (raw === null || raw === undefined) return null;
                    var ref = new ria.reflection.ReflectionClass(clazz);
                    value = ref.instantiate();
                    if (ref.implementsIfc(ria.serialize.IDeserializable)) {
                        try {
                            ref.getMethodReflector("deserialize").invokeOn(value, [ raw ]);
                        } catch (e) {
                            throw new ria.serialize.Exception("Error in deserialize method of class " + ref.getName(), e);
                        }
                        return value;
                    }
                    ref.getPropertiesReflector().forEach(function(property) {
                        var name = property.getShortName();
                        if (property.isAnnotatedWith(ria.serialize.SerializeProperty)) name = property.findAnnotation(ria.serialize.SerializeProperty).pop().name;
                        try {
                            var tmp = null;
                            var r = raw;
                            var path = name.split(".").filter(isValue);
                            while (isValue(r) && path.length) r = r[path.shift()];
                            if (isValue(r)) tmp = deserialize(r, property.getType());
                            property.invokeSetterOn(value, tmp);
                        } catch (e) {
                            throw new ria.serialize.Exception('Error deserializing property "' + property.getName() + ", value: " + JSON.stringify(r), e);
                        }
                    });
                    return value;
                }
                throw new ria.serialize.Exception('Unsupported type "' + ria.__API.getIdentifierOfType(clazz) + '"');
            } ]);
        })();
    })();
    "ria.mvc.IContext";
    "ria.mvc.IContextable";
    "ria.async.Future";
    "ria.serialize.JsonSerializer";
    "ria.reflection.ReflectionClass";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            var jsonSerializer = new ria.serialize.JsonSerializer();
            ria.mvc.ControllerUri = ria.__API.annotation("ria.mvc.ControllerUri", [], [ "value" ]);
            ria.mvc.AccessFor = ria.__API.annotation("ria.mvc.AccessFor", [], [ "roles" ]);
            ria.mvc.Inject = ria.__API.annotation("ria.mvc.Inject", [], []);
            function toCamelCase(str) {
                return str.replace(/(\-[a-z])/g, function($1) {
                    return $1.substring(1).toUpperCase();
                });
            }
            ria.mvc.Controller = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.ABSTRACT, "Controller", ria.__SYNTAX.IMPLEMENTS(ria.mvc.IContextable), [ ria.mvc.IContext, "context", ria.mvc.State, "state", ria.mvc.IView, "view", function $() {
                this.context = null;
                this.state = null;
                this.view = null;
            }, ria.async.Future, function onAppStart() {
                return ria.async.DeferredAction();
            }, ria.async.Future, function onAppInit() {
                return ria.async.DeferredAction();
            }, ria.__SYNTAX.Modifiers.VOID, function onInitialize() {
                this.view = this.context.getDefaultView();
                this.state = null;
            }, [ [ String, String, Array ] ], ria.__SYNTAX.Modifiers.VOID, function redirect_(controller, action, args) {
                this.state.setController(controller);
                this.state.setParams(args);
                this.state.setAction(action);
                this.state.setDispatched(false);
                this.state.setPublic(true);
                this.context.stateUpdated();
            }, [ [ String, String, Array ] ], ria.__SYNTAX.Modifiers.VOID, function forward_(controller, action, args) {
                this.state.setController(controller);
                this.state.setParams(args);
                this.state.setAction(action);
                this.state.setDispatched(false);
                this.state.setPublic(false);
                this.context.stateUpdated();
            }, ria.__SYNTAX.Modifiers.VOID, function preDispatchAction_() {}, ria.__SYNTAX.Modifiers.VOID, function postDispatchAction_() {}, ria.reflection.ReflectionMethod, function resolveRoleAction_(state) {
                var ref = new ria.reflection.ReflectionClass(this.getClass());
                var action = toCamelCase(state.getAction()) + "Action";
                var method = ref.getMethodReflector(action);
                if (!method) throw new ria.mvc.MvcException("Controller " + ref.getName() + " has no method " + action + " for action " + state.getAction());
                return method;
            }, [ [ ria.mvc.State ] ], ria.__SYNTAX.Modifiers.VOID, function callAction_(state) {
                var params = state.getParams();
                var method = this.resolveRoleAction_(state);
                this.validateActionCall_(method, params);
                try {
                    method.invokeOn(this, this.deserializeParams_(params, method));
                } catch (e) {
                    throw new ria.mvc.MvcException("Exception in action " + method.getName(), e);
                }
            }, [ [ ria.reflection.ReflectionMethod, Array ] ], ria.__SYNTAX.Modifiers.VOID, function validateActionCall_(actionRef, params) {
                var c = params.length;
                var min = actionRef.getRequiredArguments().length;
                if (min > c) throw new ria.mvc.MvcException("Method " + actionRef.getName() + " requires at least " + min + " arguments.");
                var max = actionRef.getArguments().length;
                if (max < c) throw new ria.mvc.MvcException("Method " + actionRef.getName() + " requires at most " + max + " arguments.");
            }, [ [ Array, ria.reflection.ReflectionMethod ] ], Array, function deserializeParams_(params, actionRef) {
                var types = actionRef.getArgumentsTypes(), names = actionRef.getArguments();
                try {
                    return params.map(function(_, index) {
                        try {
                            return _ === null || _ === undefined ? _ : jsonSerializer.deserialize(_, types[index]);
                        } catch (e) {
                            throw new ria.mvc.MvcException("Error deserializing action param " + names[index], e);
                        }
                    });
                } catch (e) {
                    throw new ria.mvc.MvcException("Error deserializing action params", e);
                }
            }, [ [ ria.mvc.State ] ], ria.__SYNTAX.Modifiers.VOID, function dispatch(state) {
                this.state = state;
                this.preDispatchAction_();
                if (!state.isDispatched()) return;
                if (state.isPublic()) {
                    this.view.reset();
                }
                this.callAction_(state);
                if (!state.isDispatched()) return;
                this.postDispatchAction_();
            }, [ [ String, String, Array ] ], function Redirect(controller, action_, arg_) {
                return this.redirect_(controller, action_ || null, arg_ || null);
            }, [ [ String, String, Array ] ], function Forward(controller, action_, arg_) {
                return this.forward_(controller, action_ || null, arg_ || null);
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity), ria.async.Future ] ], function PushView(clazz, data) {
                var instance = new clazz();
                this.view.pushD(instance, data);
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity), ria.async.Future ] ], function ShadeView(clazz, data) {
                var instance = new clazz();
                this.view.shadeD(instance, data);
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity), ria.async.Future, String ] ], function UpdateView(clazz, data, msg_) {
                this.view.updateD(clazz, data, msg_ || "");
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity) ] ], function StartLoading(clazz) {
                this.view.startLoading(clazz);
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity) ] ], function StopLoading(clazz) {
                this.view.stopLoading(clazz);
            } ]);
        })();
    })();
    "ria.mvc.IContextable";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.Control = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.ABSTRACT, "Control", ria.__SYNTAX.IMPLEMENTS(ria.mvc.IContextable), [ ria.mvc.IContext, "context", ria.async.Future, function onAppStart() {
                return ria.async.DeferredAction();
            }, function init() {
                this.onCreate_();
            }, ria.__SYNTAX.Modifiers.ABSTRACT, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {}, ria.__SYNTAX.Modifiers.VOID, function onDispose_() {} ]);
        })();
    })();
    "ria.async.Future";
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            "use strict";
            ria.async.Completer = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.async." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Completer", ria.__SYNTAX.IMPLEMENTS(ria.async.ICancelable), [ ria.__SYNTAX.Modifiers.READONLY, ria.async.Future, "future", ria.__SYNTAX.Modifiers.READONLY, Boolean, "completed", [ [ ria.async.ICancelable ] ], function $(canceler_) {
                this.future = new ria.async.Future(canceler_);
                this.completed = false;
            }, [ [ String, Object ] ], function doCallFuture_(method, arg_) {
                if (this.completed) return;
                var future_protected = this.future.__PROTECTED || this.future;
                var args = [];
                arg_ !== undefined && args.push(arg_);
                return future_protected[method].apply(future_protected, args);
            }, ria.__SYNTAX.Modifiers.VOID, function progress(data) {
                this.doCallFuture_("updateProgress_", data);
            }, ria.__SYNTAX.Modifiers.VOID, function complete(data) {
                this.doCallFuture_("complete_", data);
                this.completed = true;
            }, ria.__SYNTAX.Modifiers.VOID, function completeError(error) {
                this.doCallFuture_("completeError_", error);
                this.completed = true;
            }, ria.__SYNTAX.Modifiers.VOID, function cancel() {
                this.doCallFuture_("completeBreak_");
                this.completed = true;
            } ]);
        })();
    })();
    "ria.async.Completer";
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            "use strict";
            ria.async.wait = function(future) {
                var futures = Array.isArray(future) ? future : [].slice.call(arguments), completer = new ria.async.Completer(), counter = 0, size = futures.length + 1, results = [], complete = false;
                futures.unshift(ria.async.DeferredAction());
                futures.forEach(function(_, index) {
                    null;
                    _.then(function(data) {
                        if (complete) return;
                        counter++;
                        results[index] = data;
                        if (counter == size) {
                            complete = true;
                            completer.complete(results.slice(1));
                        } else {
                            completer.progress(counter);
                        }
                    }).catchError(function(e) {
                        if (complete) return;
                        complete = true;
                        completer.completeError(e);
                    });
                });
                return completer.getFuture();
            };
        })();
    })();
    "ria.mvc.MvcException";
    "ria.mvc.IContext";
    "ria.mvc.State";
    "ria.mvc.IStateSerializer";
    "ria.mvc.IDispatchPlugin";
    "ria.mvc.Controller";
    "ria.mvc.Control";
    "ria.async.Future";
    "ria.async.wait";
    "ria.reflection.ReflectionClass";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            function capitalize(str) {
                return str.toLowerCase().replace(/\w/, function(x) {
                    return x.toUpperCase();
                });
            }
            function toDashed(str) {
                return str.replace(/([A-Z])/g, function($1) {
                    return "-" + $1.toLowerCase();
                });
            }
            function controllerNameToUri(name) {
                return toDashed(name.replace("Controller", "").toLowerCase());
            }
            function toCamelCase(str) {
                return str.replace(/(\-[a-z])/g, function($1) {
                    return $1.substring(1).toUpperCase();
                });
            }
            ria.mvc.Dispatcher = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Dispatcher", [ String, "defaultControllerId", String, "defaultControllerAction", ria.mvc.State, "state", ria.__SYNTAX.Modifiers.READONLY, Boolean, "dispatching", function $() {
                this.defaultControllerId = "index";
                this.defaultControllerAction = "index";
                this.plugins = [];
                this.dispatching = false;
                this.controllers = {};
                this.cache = {};
                this.controls = [];
            }, [ [ ria.mvc.IDispatchPlugin ] ], ria.__SYNTAX.Modifiers.VOID, function addPlugin(plugin) {
                this.plugins.push(plugin);
            }, [ [ ria.mvc.IDispatchPlugin ] ], ria.__SYNTAX.Modifiers.VOID, function removePlugin(plugin) {
                var index = this.plugins.indexOf(plugin);
                index >= 0 && this.plugins.splice(index, 1);
            }, [ [ ria.reflection.ReflectionClass ] ], ria.async.Future, function loadControllers_(baseRef) {
                var onAppStartFutures = [];
                baseRef.getChildrenReflector().forEach(function(controllerRef) {
                    var name = controllerRef.getShortName();
                    if (name.match(/.*Controller$/)) {
                        if (controllerRef.isAnnotatedWith(ria.mvc.ControllerUri)) name = controllerRef.findAnnotation(ria.mvc.ControllerUri).shift().value; else name = controllerNameToUri(name);
                        try {
                            onAppStartFutures.push(controllerRef.instantiate().onAppStart());
                            this.controllers[name] = controllerRef;
                        } catch (e) {
                            throw new ria.mvc.MvcException("Error intializing controller " + controllerRef.getName(), e);
                        }
                    }
                    this.loadControllers_(controllerRef);
                }.bind(this));
                return ria.async.wait(onAppStartFutures);
            }, ria.async.Future, function loadControllers() {
                return this.loadControllers_(new ria.reflection.ReflectionClass(ria.mvc.Controller));
            }, [ [ ria.mvc.IContext ] ], ria.async.Future, function initControllers(context) {
                var onAppInitFutures = [];
                for (var name in this.controllers) if (this.controllers.hasOwnProperty(name)) {
                    onAppInitFutures.push(this.prepareInstance_(this.controllers[name], context).onAppInit());
                }
                return ria.async.wait(onAppInitFutures);
            }, [ [ ria.reflection.ReflectionClass ] ], ria.async.Future, function loadControl_(baseRef) {
                var onAppStartFutures = [];
                baseRef.getChildrenReflector().forEach(function(controlRef) {
                    var name = controlRef.getShortName();
                    if (name.match(/.*Control$/)) {
                        try {
                            this.controls.push(controlRef);
                            onAppStartFutures.push(controlRef.instantiate().onAppStart());
                        } catch (e) {
                            throw new ria.mvc.MvcException("Error intializing control " + controlRef.getName(), e);
                        }
                    }
                    this.loadControl_(controlRef);
                }.bind(this));
                return ria.async.wait(onAppStartFutures);
            }, ria.async.Future, function loadControls() {
                return this.loadControl_(new ria.reflection.ReflectionClass(ria.mvc.Control));
            }, [ [ ria.mvc.IContext ] ], ria.__SYNTAX.Modifiers.VOID, function initControls(context) {
                var getC = this.prepareInstance_;
                this.controls.forEach(function(_) {
                    getC(_, context).init();
                });
            }, [ [ ria.__API.ClassOf(ria.__API.Class), ria.mvc.IContext ] ], ria.__API.Class, function getCached_(type, context) {
                var ref = new ria.reflection.ReflectionClass(type);
                var name = ref.getName();
                if (this.cache.hasOwnProperty(name)) {
                    return this.cache[name];
                }
                var instanse = this.cache[name] = ref.instantiate();
                if (ref.implementsIfc(ria.mvc.IContextable)) {
                    ref.getPropertyReflector("context").invokeSetterOn(instanse, context);
                }
                return instanse;
            }, [ [ ria.reflection.ReflectionClass, ria.mvc.IContext ] ], ria.__API.Class, function prepareInstance_(ref, context) {
                var instanse = ref.instantiate();
                ref.getPropertiesReflector().forEach(function(_) {
                    if (_.isReadonly()) return;
                    if (!_.isAnnotatedWith(ria.mvc.Inject)) return;
                    _.invokeSetterOn(instanse, this.getCached_(_.getType(), context));
                }.bind(this));
                if (ref.implementsIfc(ria.mvc.IContextable)) {
                    ref.getPropertyReflector("context").invokeSetterOn(instanse, context);
                }
                return instanse;
            }, [ [ ria.__API.ClassOf(ria.__API.Class), ria.mvc.IContext ] ], ria.__API.Class, function createService(clazz, context) {
                return this.getCached_(clazz, context);
            }, [ [ ria.mvc.State, ria.mvc.IContext ] ], ria.__SYNTAX.Modifiers.VOID, function dispatch(state, context) {
                var index;
                try {
                    this.dispatching = true;
                    try {
                        for (index = this.plugins.length; index > 0; index--) this.plugins[index - 1].dispatchStartup();
                        state.setController(state.getController() || this.defaultControllerId);
                        state.setAction(state.getAction() || this.defaultControllerAction);
                        this.setState(state);
                        do {
                            state.setDispatched(true);
                            for (index = this.plugins.length; index > 0; index--) this.plugins[index - 1].preDispatch(state);
                            if (!state.isDispatched()) continue;
                            if (!this.controllers.hasOwnProperty(state.getController())) {
                                throw new ria.mvc.MvcException('Controller with id "' + state.getController() + '" not found');
                            }
                            var instanse = this.prepareInstance_(this.controllers[state.getController()], context);
                            instanse.onInitialize();
                            instanse.dispatch(state);
                            if (!state.isDispatched()) continue;
                            for (index = this.plugins.length; index > 0; index--) this.plugins[index - 1].postDispatch(state);
                        } while (!state.isDispatched());
                    } catch (e) {
                        throw new ria.mvc.MvcException("Dispatch failed.", e);
                    }
                    try {
                        for (index = this.plugins.length; index > 0; index--) this.plugins[index - 1].dispatchShutdown();
                    } catch (e) {
                        throw new ria.mvc.MvcException("Dispatch failed.", e);
                    }
                } finally {
                    this.dispatching = false;
                }
            } ]);
        })();
    })();
    "ria.mvc.IContext";
    "ria.mvc.Dispatcher";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            ria.mvc.ServiceCreateDelegate = ria.__API.delegate("ria.mvc.ServiceCreateDelegate", ria.__API.Class, [ ria.__API.ClassOf(ria.__API.Class), ria.mvc.IContext ], [ "clazz", "context" ]);
            ria.mvc.BaseContext = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BaseContext", ria.__SYNTAX.IMPLEMENTS(ria.mvc.IContext), [ ria.mvc.ServiceCreateDelegate, "serviceCreateDelegate", ria.mvc.IView, "defaultView", ria.mvc.ISession, "session", ria.mvc.Dispatcher, "dispatcher", ria.__SYNTAX.Modifiers.VOID, function stateUpdated() {
                if (!this.dispatcher.isDispatching()) this.dispatcher.dispatch(this.dispatcher.getState(), this);
            }, ria.mvc.State, function getState() {
                return this.dispatcher.getState();
            }, [ [ ria.__API.ClassOf(ria.__API.Class) ] ], ria.__API.Class, function getService(clazz) {
                if (!this.serviceCreateDelegate) throw ria.__API.Exception("No service creator is provided");
                return this.serviceCreateDelegate(clazz, this);
            } ]);
        })();
    })();
    "ria.mvc.ISession";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            ria.mvc.Session = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Session", ria.__SYNTAX.IMPLEMENTS(ria.mvc.ISession), [ function $() {
                this.items = {};
            }, [ [ String, Object ] ], Object, function get(key, def_) {
                return this.items.hasOwnProperty(key) ? this.items[key] : def_;
            }, [ [ String, Object, Boolean ] ], ria.__SYNTAX.Modifiers.VOID, function set(key, value, isPersistent_) {
                this.items[key] = value;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.VOID, function remove(key) {
                delete this.items[key];
            } ]);
        })();
    })();
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            ria.async.Observer = ria.__API.delegate("ria.async.Observer", Boolean, [ Object ], [ "data_" ]);
            ria.async.IObservable = ria.__API.ifc(function() {}, "ria.async.IObservable", [ [ "on", null, [ Function, Object ], [ "handler", "scope_" ] ], [ "off", null, [ Function ], [ "handler" ] ] ]);
        })();
    })();
    "ria.async.IObservable";
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            ria.async.Observable = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.async." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Observable", ria.__SYNTAX.IMPLEMENTS(ria.async.IObservable), [ [ [ Function ] ], function $(T) {
                this.T = T;
                this._handlers = [];
            }, [ [ Function, Object ] ], ria.async.IObservable, function on(handler, scope_) {
                null;
                this.off(handler);
                this._handlers.push([ handler, scope_ ]);
                return this;
            }, [ [ Function ] ], ria.async.IObservable, function off(handler) {
                null;
                this._handlers = this._handlers.filter(function(_) {
                    return handler[0] !== _;
                });
                return this;
            }, [ [ Array, Boolean ] ], ria.__SYNTAX.Modifiers.VOID, function notify_(data, once) {
                var me = this;
                this._handlers.forEach(function(_) {
                    ria.__API.defer(me, function(handler, scope) {
                        var result = true;
                        try {
                            result = handler.apply(scope, data);
                        } catch (e) {
                            throw new ria.__API.Exception("Unhandled error occurred while notifying observer", e);
                        } finally {
                            if (!once && result !== false) me._handlers.push(_);
                        }
                    }, _);
                });
                this._handlers = [];
            }, [ [ Array, Boolean ] ], ria.__SYNTAX.Modifiers.VOID, function notify(data_) {
                this.notify_(data_ || [], false);
            }, [ [ Array ] ], ria.__SYNTAX.Modifiers.VOID, function notifyAndClear(data_) {
                this.notify_(data_ || [], true);
            }, Number, function count() {
                return this._handlers.length;
            }, ria.__SYNTAX.Modifiers.VOID, function clear() {
                this._handlers = [];
            } ]);
        })();
    })();
    "ria.mvc.MvcException";
    "ria.mvc.IView";
    "ria.async.Observable";
    "ria.reflection.ReflectionClass";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.View = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("View", ria.__SYNTAX.IMPLEMENTS(ria.mvc.IView), [ function $() {
                BASE();
                this._stack = [];
                this._refreshEvents = new ria.async.Observable(ria.mvc.ActivityRefreshedEvent);
            }, [ [ ria.mvc.IActivity, ria.mvc.IActivity ] ], Boolean, function isSameActivityGroup_(a1, a2) {
                var ref1 = new ria.reflection.ReflectionClass(a1.getClass());
                var ref2 = new ria.reflection.ReflectionClass(a2.getClass());
                var v1 = ref1.findAnnotation(ria.mvc.ActivityGroup)[0];
                var v2 = ref2.findAnnotation(ria.mvc.ActivityGroup)[0];
                return v1 != null && v2 != null && v1.name == v2.name;
            }, [ [ ria.mvc.IActivity ] ], ria.__SYNTAX.Modifiers.VOID, function push_(activity) {
                activity.addCloseCallback(this.onActivityClosed_);
                activity.addRefreshCallback(this.onActivityRefreshed_);
                activity.show();
                this._stack.unshift(activity);
            }, [ [ ria.mvc.IActivity ] ], ria.__SYNTAX.Modifiers.VOID, function onActivityClosed_(activity) {
                if (!this._stack.filter(function(_) {
                    return _ && _.equals(activity);
                }).length) return;
                while (this.getCurrent() != null) {
                    if (this.getCurrent().equals(activity)) {
                        this.pop();
                        break;
                    } else {
                        this.pop_();
                    }
                }
            }, ria.mvc.IActivity, function pop_() {
                return this._stack.shift() || null;
            }, [ [ ria.mvc.IActivity, ria.async.Future ] ], ria.__SYNTAX.Modifiers.VOID, function pushD(activity, data) {
                this.push(activity);
                activity.refreshD(data);
            }, [ [ ria.mvc.IActivity ] ], ria.__SYNTAX.Modifiers.VOID, function push(activity) {
                this.reset();
                this.push_(activity);
            }, [ [ ria.mvc.IActivity, ria.async.Future ] ], ria.__SYNTAX.Modifiers.VOID, function shadeD(activity, data) {
                this.shade(activity);
                activity.refreshD(data);
            }, [ [ ria.mvc.IActivity ] ], ria.__SYNTAX.Modifiers.VOID, function shade(activity) {
                var top = this.getCurrent();
                if (top) {
                    top.pause();
                    if (this.isSameActivityGroup_(top, activity)) this.pop_().stop();
                }
                this.push_(activity);
            }, ria.mvc.IActivity, function pop() {
                var pop = this.pop_();
                if (!pop) return null;
                pop.stop();
                var top = this.getCurrent();
                if (top) {
                    top.show();
                }
                return pop;
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity), ria.async.Future, String ] ], ria.__SYNTAX.Modifiers.VOID, function updateD(activityClass, data, msg_) {
                this._stack.forEach(function(_) {
                    if (_ instanceof activityClass) _.partialRefreshD(data, msg_);
                });
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity) ] ], ria.__SYNTAX.Modifiers.VOID, function startLoading(activityClass) {
                this._stack.forEach(function(_) {
                    if (_ instanceof activityClass) _.startLoading();
                });
            }, [ [ ria.__API.ImplementerOf(ria.mvc.IActivity) ] ], ria.__SYNTAX.Modifiers.VOID, function stopLoading(activityClass) {
                this._stack.forEach(function(_) {
                    if (_ instanceof activityClass) _.stopLoading();
                });
            }, ria.mvc.IActivity, function getCurrent() {
                return this._stack[0] || null;
            }, ria.__SYNTAX.Modifiers.VOID, function reset() {
                while (this.getCurrent() !== null) this.pop_().stop();
            }, ria.__API.ArrayOf(ria.mvc.IActivity), function getStack_() {
                return this._stack.slice();
            }, [ [ ria.mvc.IActivity, Object, String ] ], ria.__SYNTAX.Modifiers.VOID, function onActivityRefreshed_(activity, model, msg_) {
                this._refreshEvents.notifyAndClear([ activity, model, msg_ ]);
            }, [ [ ria.mvc.ActivityRefreshedEvent ] ], ria.__SYNTAX.Modifiers.VOID, function onActivityRefreshed(callback) {
                this._refreshEvents.on(callback);
            } ]);
        })();
    })();
    "ria.mvc.IStateSerializer";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            ria.mvc.StateSerializer = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StateSerializer", ria.__SYNTAX.IMPLEMENTS(ria.mvc.IStateSerializer), [ [ [ String ] ], function $(separator) {
                this.separator = separator;
            }, [ [ ria.mvc.State ] ], String, function serialize(state) {
                var params = state.getParams().slice().filter(function(_) {
                    return _ !== null && _ !== undefined;
                });
                params.unshift(state.getController(), state.getAction());
                return params.map(function(_) {
                    return encodeURIComponent(_);
                }).join(this.separator);
            }, [ [ String ] ], ria.mvc.State, function deserialize(value) {
                var params = value.split(this.separator).map(function(_) {
                    return decodeURIComponent(_);
                });
                var state = new ria.mvc.State();
                state.setController(params.shift() || null);
                state.setAction(params.shift() || null);
                state.setParams(params);
                return state;
            } ]);
        })();
    })();
    "ria.reflection.ReflectionClass";
    "ria.mvc.MvcException";
    "ria.mvc.IContextable";
    "ria.mvc.ISession";
    "ria.mvc.IStateSerializer";
    "ria.mvc.IView";
    "ria.mvc.BaseContext";
    "ria.mvc.Dispatcher";
    "ria.mvc.Session";
    "ria.mvc.View";
    "ria.mvc.StateSerializer";
    "ria.mvc.Controller";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            var History = window.History;
            ria.mvc.Application = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Application", [ ria.mvc.IStateSerializer, "serializer", ria.mvc.IContext, "context", function $() {
                this.serializer = this.initSerializer_();
                this._dispatcher = this.initDispatcher_();
                this.context = this.initContext_();
            }, ria.mvc.IStateSerializer, function initSerializer_() {
                return new ria.mvc.StateSerializer("/");
            }, ria.mvc.Dispatcher, function initDispatcher_() {
                return new ria.mvc.Dispatcher();
            }, ria.mvc.ISession, function initSession_() {
                return new ria.mvc.Session();
            }, ria.mvc.IView, function initView_() {
                return new ria.mvc.View();
            }, function initContext_() {
                var context = new ria.mvc.BaseContext();
                context.setDispatcher(this._dispatcher);
                context.setSession(this.initSession_());
                context.setDefaultView(this.initView_());
                context.setServiceCreateDelegate(this._dispatcher.createService);
                return context;
            }, [ [ HashChangeEvent ] ], ria.__SYNTAX.Modifiers.VOID, function onHashChanged_(event) {
                this.dispatch(event.newUrl);
            }, ria.__SYNTAX.Modifiers.SELF, function session(obj) {
                var session = this.context.getSession();
                for (var key in obj) if (obj.hasOwnProperty(key)) {
                    session.set(key, obj[key], false);
                }
                return this;
            }, ria.__SYNTAX.Modifiers.VOID, function run() {
                var me = this;
                ria.async.DeferredAction().then(function() {
                    return me.onInitialize_();
                }).then(function() {
                    return me._dispatcher.loadControls();
                }).then(function() {
                    return me._dispatcher.loadControllers();
                }).then(function() {
                    me._dispatcher.initControls(me.context);
                    return null;
                }).then(function() {
                    return me.onStart_();
                }).then(function() {
                    return me._dispatcher.initControllers(me.context);
                }).then(function() {
                    me.onResume_();
                    return null;
                }).then(function() {
                    me.dispatch();
                    return null;
                }).catchError(function(e) {
                    throw new ria.mvc.MvcException("Failed to start application", e);
                });
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.VOID, function dispatch(route_) {
                var state = this.serializer.deserialize(route_ || window.location.hash.substr(1));
                state.setPublic(true);
                this._dispatcher.dispatch(state, this.context);
            }, ria.async.Future, function onInitialize_() {
                window.addEventListener("hashchange", this.onHashChanged_, false);
                return ria.async.DeferredAction();
            }, ria.async.Future, function onStart_() {
                return ria.async.DeferredAction();
            }, ria.__SYNTAX.Modifiers.VOID, function onResume_() {}, ria.__SYNTAX.Modifiers.VOID, function onPause_() {}, ria.__SYNTAX.Modifiers.VOID, function onStop_() {
                return ria.async.DeferredAction();
            }, ria.__SYNTAX.Modifiers.VOID, function onDispose_() {} ]);
        })();
    })();
    (function() {
        (ria = ria || {}).dom = ria.dom || {};
        (function() {
            "use strict";
            var global = "undefined" !== typeof window ? window.document : null, docElem = global.documentElement, __find = function(s, n) {
                return n.querySelectorAll(s);
            }, __is = docElem ? docElem.webkitMatchesSelector || docElem.mozMatchesSelector || docElem.oMatchesSelector || docElem.msMatchesSelector : function() {
                return false;
            };
            function checkEventHandlerResult(event, result) {
                if (result === false) {
                    event.stopPropagation && event.stopPropagation();
                    event.cancelBubble && event.cancelBubble();
                    event.preventDefault && event.preventDefault();
                }
            }
            function nulls(_) {
                return _ != null;
            }
            ria.dom.Event = Event;
            ria.dom.DomIterator = ria.__API.delegate("ria.dom.DomIterator", Object, [], [ "node" ]);
            ria.dom.DomEventHandler = ria.__API.delegate("ria.dom.DomEventHandler", Boolean, [ Object, ria.dom.Event ], [ "node", "event" ]);
            ria.dom.Keys =             function wrapper() {
                var values = {};
                function Keys(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(Keys, "ria.dom.Keys");
                function KeysImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[ria.dom.Keys#" + value + "]";
                    };
                }
                ria.__API.extend(KeysImpl, Keys);
                values[37] = Keys.LEFT = new KeysImpl(37);
                values[38] = Keys.UP = new KeysImpl(38);
                values[39] = Keys.RIGHT = new KeysImpl(39);
                values[40] = Keys.DOWN = new KeysImpl(40);
                values[13] = Keys.ENTER = new KeysImpl(13);
                values[8] = Keys.BACKSPACE = new KeysImpl(8);
                values[46] = Keys.DELETE = new KeysImpl(46);
                values[27] = Keys.ESC = new KeysImpl(27);
                values[32] = Keys.SPACE = new KeysImpl(32);
                return Keys;
            }();
            var GID = new Date().getTime();
            ria.dom.NewGID = function() {
                return "gid-" + (GID++).toString(36);
            };
            ria.dom.Dom = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.dom." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Dom", [ function $$(instance, clazz, ctor, args) {
                if (DomImpl == ria.__SYNTAX.Modifiers.SELF) throw Error("");
                instance = DomImpl.apply(undefined, args);
                ria.__SYNTAX && ria.__SYNTAX.checkReturn(ria.dom.Dom, instance);
                return instance;
            }, function $(dom_) {
                null;
                this._dom = [ global ];
                if ("string" === typeof dom_) {
                    this._dom = this.find(dom_).valueOf();
                } else if (Array.isArray(dom_)) {
                    this._dom = dom_;
                } else if (dom_ instanceof Node) {
                    this._dom = [ dom_ ];
                } else if (dom_ instanceof NodeList) {
                    this._dom = ria.__API.clone(dom_);
                } else if (dom_ instanceof ria.__SYNTAX.Modifiers.SELF) {
                    this._dom = dom_.valueOf();
                }
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function find(selector) {
                return new ria.dom.Dom(__find(selector, this._dom[0]));
            }, ria.__SYNTAX.Modifiers.SELF, function on(event, selector, handler_) {
                null;
                var events = event.split(" ").filter(nulls);
                if (!handler_) {
                    handler_ = selector;
                    selector = undefined;
                }
                var hid = handler_.__domEventHandlerId = handler_.__domEventHandlerId || Math.random().toString(36).slice(2);
                this._dom.forEach(function(element) {
                    events.forEach(function(evt) {
                        element.__domEvents = element.__domEvents || {};
                        if (element.__domEvents[evt + hid]) return;
                        var h = element.__domEvents[evt + hid] = function(e) {
                            var target = new ria.dom.Dom(e.target);
                            if (selector === undefined) return checkEventHandlerResult(e, handler_(target, e));
                            if (target.is(selector)) return checkEventHandlerResult(e, handler_(target, e));
                            var selectorTarget = new ria.dom.Dom(element).find(selector).filter(function(_) {
                                return _.contains(e.target);
                            }).valueOf().pop();
                            if (selectorTarget) return checkEventHandlerResult(e, handler_(new ria.dom.Dom(selectorTarget), e));
                        };
                        element.addEventListener(evt, h, "change select focus blur".search(evt) >= 0);
                    });
                });
                return this;
            }, ria.__SYNTAX.Modifiers.SELF, function off(event, selector, handler_) {
                null;
                var events = event.split(" ").filter(nulls);
                if (!handler_) {
                    handler_ = selector;
                    selector = undefined;
                }
                var hid = handler_.__domEventHandlerId;
                if (!hid) return;
                this._dom.forEach(function(element) {
                    events.forEach(function(evt) {
                        if (!element.__domEvents) return;
                        var h;
                        if (h = element.__domEvents[evt + hid]) element.removeEventListener(evt, h, "change select focus blur".search(evt) >= 0);
                    });
                });
                return this;
            }, ria.__SYNTAX.Modifiers.SELF, function appendTo(dom) {
                null;
                if (typeof dom == "string") dom = new ria.dom.Dom(dom);
                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                null;
                this._dom.forEach(function(item) {
                    dest.appendChild(item);
                });
                return this;
            }, ria.__SYNTAX.Modifiers.SELF, function prependTo(dom) {
                null;
                if (typeof dom == "string") dom = new ria.dom.Dom(dom);
                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                null;
                var first = dest.firstChild;
                if (!first) return this.appendTo(dest);
                this._dom.forEach(function(item) {
                    dest.insertBefore(item, first);
                });
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function fromHTML(html) {
                this._dom = [];
                var div = document.createElement("div");
                div.innerHTML = html;
                var count = div.childElementCount;
                for (var i = 0; i < count; i++) {
                    var node = div.removeChild(div.childNodes[0]);
                    node && this._dom.push(node);
                }
                return this;
            }, ria.__SYNTAX.Modifiers.SELF, function empty() {
                this._dom.forEach(function(element) {
                    element.innerHTML = "";
                });
                return this;
            }, [ [ ria.__SYNTAX.Modifiers.SELF ] ], ria.__SYNTAX.Modifiers.SELF, function remove(node) {
                this.forEach(function(element) {
                    element.removeChild(node);
                });
                return this;
            }, [ [ ria.__SYNTAX.Modifiers.SELF ] ], ria.__SYNTAX.Modifiers.SELF, function removeSelf() {
                this.forEach(function(element) {
                    element.parentNode.removeChild(element);
                });
                return this;
            }, [ [ ria.__SYNTAX.Modifiers.SELF ] ], Boolean, function areEquals(el) {
                var val1 = this.valueOf(), val2 = el.valueOf(), len = val1.length;
                if (len != val2.valueOf().length) return false;
                for (var i = 0; i < len; i++) {
                    if (val1[i] != val2[i]) return false;
                }
                return true;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function descendants(selector__) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function parent(selector_) {
                if (selector_) {
                    var parents = new ria.dom.Dom(selector_);
                    if (parents.count() == 0) return null;
                    if (parents.count() == 1) if (parents.contains(this)) {
                        return parents;
                    } else {
                        return null;
                    }
                    parents.forEach(function(parent) {
                        if (parent.contains(this)) return parent;
                    });
                }
            }, Object, function offset() {
                if (!this._dom[0]) return null;
                var box = this._dom[0].getBoundingClientRect();
                var body = document.body;
                var docElem = document.documentElement;
                var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop;
                var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft;
                var clientTop = docElem.clientTop || body.clientTop || 0;
                var clientLeft = docElem.clientLeft || body.clientLeft || 0;
                var top = box.top + scrollTop - clientTop;
                var left = box.left + scrollLeft - clientLeft;
                return {
                    top: Math.round(top),
                    left: Math.round(left)
                };
            }, Number, function height() {
                return this._dom[0] ? this._dom[0].getBoundingClientRect().height : null;
            }, Number, function width() {
                return this._dom[0] ? this._dom[0].getBoundingClientRect().width : null;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function next(selector_) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function previous(selector_) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function first(selector_) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function last(selector_) {}, [ [ String ] ], Boolean, function is(selector) {
                return this._dom.some(function(el) {
                    return __is.call(el, selector);
                });
            }, [ [ Object ] ], Boolean, function contains(node) {
                null;
                var nodes = [];
                if (node instanceof Node) {
                    nodes = [ node ];
                } else if (Array.isArray(node)) {
                    nodes = node;
                } else if (node instanceof ria.__SYNTAX.Modifiers.SELF) {
                    nodes = node.valueOf();
                }
                return this._dom.some(function(el) {
                    return nodes.some(function(_) {
                        return el.contains(_);
                    });
                });
            }, Boolean, function exists() {
                return !!this._dom[0];
            }, Object, function getValue() {
                return this.valueOf()[0].value;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.SELF, function setValue(value) {
                this.valueOf()[0].value = value;
                return this;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.SELF, function setFormValues(values) {
                for (var valueName in values) {
                    if (values.hasOwnProperty(valueName)) {
                        this.find('[name="' + valueName + '"]').setValue(values[valueName]);
                    }
                }
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function triggerEvent(event) {
                var node = this.valueOf()[0];
                if (document.createEvent) {
                    var evt = document.createEvent("Event");
                    evt.initEvent(event, true, false);
                    node.dispatchEvent(evt);
                } else if (document.createEventObject) {
                    node.fireEvent("on" + event);
                } else if (typeof node.onsubmit == "function") {
                    node.onsubmit();
                }
                return this;
            }, Object, function getAllAttrs() {}, [ [ String ] ], Object, function getAttr(name) {
                var node = this._dom[0];
                return node ? node.getAttribute(name) : null;
            }, Object, function getValue() {
                return this._dom.value;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.SELF, function setAllAttrs(obj) {
                for (var k in obj) if (obj.hasOwnProperty(k)) this.setAttr(k, obj[k]);
                return this;
            }, [ [ String, Object ] ], ria.__SYNTAX.Modifiers.SELF, function setAttr(name, value) {
                var node = this._dom[0];
                node ? node.setAttribute(name, value) : null;
                return this;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.SELF, function setValue(value) {
                this._dom.value = value;
                return this;
            }, Object, function getAllData() {}, [ [ String ] ], Object, function getData(name) {
                return this.getAttr("data-" + name);
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.SELF, function setAllData(obj) {}, [ [ String, Object ] ], ria.__SYNTAX.Modifiers.SELF, function setData(name, value) {}, [ [ String ] ], Boolean, function hasClass(clazz) {
                return (" " + this.getAttr("class") + " ").replace(/\s+/g, " ").indexOf(" " + clazz + " ") >= 0;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function addClass(clazz) {
                return this.toggleClass(clazz, true);
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function removeClass(clazz) {
                return this.toggleClass(clazz, false);
            }, [ [ String, Boolean ] ], ria.__SYNTAX.Modifiers.SELF, function toggleClass(clazz, toggleOn_) {
                var hasClass = this.hasClass(clazz);
                toggleOn_ = toggleOn_ === undefined ? !hasClass : toggleOn_;
                if (toggleOn_ && !hasClass) {
                    this.setAttr("class", this.getAttr("class") + " " + clazz);
                    return this;
                }
                if (!toggleOn_ && hasClass) {
                    this.setAttr("class", this.getAttr("class").split(/\s+/).filter(function(_) {
                        return _ != clazz;
                    }).join(" "));
                    return this;
                }
                return this;
            }, [ [ String ] ], Object, function getCss(property) {}, [ [ String, Object ] ], ria.__SYNTAX.Modifiers.SELF, function setCss(property, value) {
                for (var i = 0; i < this._dom.length; i++) {
                    this._dom[i].style[property] = value + "px";
                }
                return this;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.SELF, function updateCss(props) {}, [ [ ria.dom.DomIterator ] ], ria.__SYNTAX.Modifiers.SELF, function forEach(iterator) {
                this._dom.forEach(function(_) {
                    iterator(ria.__SYNTAX.Modifiers.SELF(_));
                });
            }, [ [ ria.dom.DomIterator ] ], ria.__SYNTAX.Modifiers.SELF, function filter(iterator) {
                this._dom = this._dom.filter(function(_) {
                    return iterator(ria.__SYNTAX.Modifiers.SELF(_));
                });
                return this;
            }, Number, function count() {
                return this._dom.length;
            }, ria.__API.ArrayOf(Node), function valueOf() {
                return this._dom.slice();
            } ]);
            ria.dom.SimpleDom = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.dom." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SimpleDom", ria.__SYNTAX.EXTENDS(ria.dom.Dom), [ function $(dom_) {
                dom_ ? BASE(dom_) : BASE();
            } ]);
            var DomImpl = ria.dom.SimpleDom;
            ria.dom.setDomImpl = function(impl) {
                DomImpl = impl;
            };
        })();
    })();
    "ria.dom.Dom";
    (function() {
        (ria = ria || {}).dom = ria.dom || {};
        (function() {
            "use strict";
            if ("undefined" === typeof jQuery) throw Error("jQuery is not defined.");
            var global = "undefined" !== typeof window ? window.document : null;
            ria.dom.Event = Object;
            ria.dom.jQueryDom = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.dom." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("jQueryDom", ria.__SYNTAX.EXTENDS(ria.dom.Dom), [ function $(dom_) {
                null;
                this._dom = jQuery(global);
                if ("string" === typeof dom_) {
                    this._dom = jQuery(dom_);
                } else if (Array.isArray(dom_)) {
                    this._dom = jQuery(dom_);
                } else if (dom_ instanceof Node) {
                    this._dom = jQuery(dom_);
                } else if (dom_ instanceof NodeList) {
                    this._dom = jQuery(dom_);
                } else if (dom_ instanceof ria.__SYNTAX.Modifiers.SELF) {
                    this._dom = jQuery(dom_.valueOf());
                } else if (dom_ instanceof jQuery) {
                    this._dom = dom_;
                }
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Boolean, function exists() {
                return !!this._dom.valueOf()[0];
            }, [ [ String ] ], Boolean, function is(selector) {
                return this._dom.is(selector);
            }, [ [ Object, Date ] ], ria.__SYNTAX.Modifiers.SELF, function datepicker(options, value_) {
                this._dom.datepicker(options);
                value_ && this._dom.datepicker("setDate", value_);
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function find(selector) {
                return new ria.dom.Dom(jQuery(selector, this._dom));
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function on(event, selector, handler_) {
                var old_dom = this._dom;
                this._dom = this.valueOf();
                BASE(event, selector, handler_);
                this._dom = old_dom;
                return this;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function off(event, selector, handler_) {
                var old_dom = this._dom;
                this._dom = this.valueOf();
                BASE(event, selector, handler_);
                this._dom = old_dom;
                return this;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function appendTo(dom) {
                null;
                if (typeof dom == "string") dom = new ria.__SYNTAX.Modifiers.SELF(dom);
                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                if (dest) {
                    null;
                    this._dom.appendTo(dest);
                }
                return this;
            }, ria.__SYNTAX.Modifiers.SELF, function appendChild(dom) {
                null;
                if (typeof dom == "string") dom = new ria.__SYNTAX.Modifiers.SELF(dom);
                var el = dom instanceof Node ? dom : dom.valueOf().shift();
                if (el) {
                    null;
                    this._dom.append(el);
                }
                return this;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function prependTo(dom) {
                null;
                if (typeof dom == "string") dom = new ria.__SYNTAX.Modifiers.SELF(dom);
                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                if (dest) {
                    null;
                    this._dom.prependTo(dest);
                }
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function fromHTML(html) {
                this._dom = jQuery(jQuery.parseHTML(html));
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function setHTML(html) {
                this._dom.html(html);
                return this;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function empty() {
                this._dom.empty();
                return this;
            }, [ [ ria.__SYNTAX.Modifiers.SELF ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function remove(node_) {
                node_ ? node_._dom.remove() : this._dom.remove();
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function descendants(selector__) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function parent(selector_) {
                return selector_ ? new ria.dom.Dom(this._dom.parents(selector_)) : new ria.dom.Dom(this._dom.parent());
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function next(selector_) {
                return new ria.dom.Dom(this._dom.next(selector_));
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function previous(selector_) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function first(selector_) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function last(selector_) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Boolean, function is(selector) {
                return this._dom.is(selector);
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Boolean, function contains(node) {
                null;
                var nodes = [];
                if (node instanceof Node) {
                    nodes = [ node ];
                } else if (Array.isArray(node)) {
                    nodes = node;
                } else if (node instanceof ria.__SYNTAX.Modifiers.SELF) {
                    nodes = node.valueOf();
                }
                var res = true;
                nodes.forEach(function(node) {
                    if (res && !jQuery.contains(this._dom[0], node)) res = false;
                }.bind(this));
                return res;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Object, function getAllAttrs() {}, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Object, function getAttr(name) {
                return this._dom.attr(name) || null;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Object, function getValue() {
                return this._dom.val();
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function setAllAttrs(obj) {}, [ [ String, Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function setAttr(name, value) {
                this._dom.attr(name, value);
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function removeAttr(name) {
                this._dom.removeAttr(name);
                return this;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function setValue(value) {
                this._dom.val(value);
                return this;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Object, function height(value_) {
                return value_ ? this._dom.height(value_) : this._dom.height();
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Object, function width(value_) {
                return value_ ? this._dom.width(value_) : this._dom.width();
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Object, function getAllData() {}, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Object, function getData(name) {
                return this._dom.data(name) === undefined ? null : this._dom.data(name);
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function setAllData(obj) {}, [ [ String, Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function setData(name, value) {
                this._dom.data(name, value);
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Boolean, function hasClass(clazz) {
                return this._dom.hasClass(clazz);
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function addClass(clazz) {
                return this.toggleClass(clazz, true);
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function removeClass(clazz) {
                return this.toggleClass(clazz, false);
            }, [ [ String, Boolean ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function toggleClass(clazz, toggleOn_) {
                this._dom.toggleClass(clazz, toggleOn_);
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, Object, function getCss(property) {}, [ [ String, Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function setCss(property, value) {
                this._dom.css(property, value);
                return this;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function updateCss(props) {}, [ [ ria.dom.DomIterator ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function forEach(iterator) {
                this._dom.each(function() {
                    iterator(ria.__SYNTAX.Modifiers.SELF(this));
                });
                return this;
            }, [ [ ria.dom.DomIterator ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.SELF, function filter(iterator) {
                this._dom = this._dom.filter(function() {
                    return iterator(ria.__SYNTAX.Modifiers.SELF(this));
                });
                return this;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, Number, function count() {
                return this._dom.length;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__API.ArrayOf(Node), function valueOf() {
                return ria.__API.clone(this._dom);
            }, [ [ String, Object ] ], ria.__SYNTAX.Modifiers.SELF, function trigger(event, params_) {
                this._dom.trigger(event, params_);
                return this;
            }, Boolean, function checked() {
                return this.parent().find(".hidden-checkbox").getData("value") || false;
            }, Object, function serialize() {
                var o = {};
                var array = this.dom.serializeArray();
                array.forEach(function() {
                    if (o[this.name] !== undefined) {
                        if (!o[this.name].push) {
                            o[this.name] = [ o[this.name] ];
                        }
                        o[this.name].push(this.value || "");
                    } else {
                        o[this.name] = this.value || "";
                    }
                });
                return o;
            } ]);
            ria.dom.setDomImpl(ria.dom.jQueryDom);
        })();
    })();
    "ria.async.Completer";
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            "use strict";
            ria.async.Task = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.async." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.ABSTRACT, "Task", ria.__SYNTAX.IMPLEMENTS(ria.async.ICancelable), [ function $() {
                this._completer = new ria.async.Completer(this);
            }, ria.__SYNTAX.Modifiers.ABSTRACT, ria.__SYNTAX.Modifiers.VOID, function do_() {}, ria.__SYNTAX.Modifiers.VOID, function cancel() {
                this._completer.cancel();
            }, ria.async.Future, function run() {
                ria.__API.defer(this, this.do_, 0);
                return this._completer.getFuture();
            } ]);
        })();
    })();
    "ria.async.Task";
    "ria.dom.Dom";
    (function() {
        (ria = ria || {}).dom = ria.dom || {};
        (function() {
            "use strict";
            var isBrowser = !!(typeof window !== "undefined" && navigator && document), isPageReady = !isBrowser, onReady = [];
            isBrowser && function() {
                function onLoaded() {
                    isPageReady = true;
                    onReady.forEach(function(_) {
                        _(null);
                    });
                }
                var DOMContentLoaded;
                if (document.addEventListener) {
                    DOMContentLoaded = function() {
                        document.removeEventListener("DOMContentLoaded", DOMContentLoaded, false);
                        onLoaded();
                    };
                } else if (document.attachEvent) {
                    DOMContentLoaded = function() {
                        if (document.readyState === "complete") {
                            document.detachEvent("onreadystatechange", DOMContentLoaded);
                            onLoaded();
                        }
                    };
                }
                if (document.readyState === "complete") {
                    return setTimeout(onLoaded, 1);
                }
                if (document.addEventListener) {
                    document.addEventListener("DOMContentLoaded", DOMContentLoaded, false);
                    window.addEventListener("load", onLoaded, false);
                } else if (document.attachEvent) {
                    document.attachEvent("onreadystatechange", DOMContentLoaded);
                    window.attachEvent("onload", onLoaded);
                }
            }();
            ria.dom.DomReadyTask = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.dom." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DomReadyTask", ria.__SYNTAX.EXTENDS(ria.async.Task), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function do_() {
                onReady.push(this._completer.complete);
            } ]);
            ria.dom.ready = function() {
                return !isPageReady ? new ria.dom.DomReadyTask().run() : ria.async.DeferredAction();
            };
        })();
    })();
    __ASSETS._o2nzrmzbsj0jm7vi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.ActionForm_mixin = function(controller, action) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes["data-controller"] = escape(controller);
            attributes["data-action"] = escape(action);
            attributes.action = attributes.action || "javascript:";
            self.prepareData(attributes);
            buf.push("<form" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + ">");
            block && block();
            buf.push("</form>");
        };
        jade.globals.Hidden_mixin = function(hidName, hidValue) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push("<input" + jade.attrs(jade.merge({
                type: "hidden",
                name: hidName,
                value: hidValue
            }, attributes), jade.merge({
                type: true,
                name: true,
                value: true
            }, escaped, true)) + "/>");
        };
        return buf.join("");
    };
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.DomEventBind = ria.__API.annotation("ria.mvc.DomEventBind", [ String, String ], [ "event", "selector_" ]);
        })();
    })();
    "ria.mvc.Control";
    "ria.dom.Dom";
    "ria.mvc.DomEventBind";
    "ria.reflection.ReflectionClass";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.DomControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DomControl", ria.__SYNTAX.EXTENDS(ria.mvc.Control), [ function $() {
                BASE();
                this._dom = null;
                this._domEvents = [];
                this.bind_();
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                var dom = this._dom = new ria.dom.Dom();
                var instance = this;
                this._domEvents.forEach(function(_) {
                    dom.on(_.event, _.selector, function(node, event) {
                        return _.methodRef.invokeOn(instance, ria.__API.clone(arguments));
                    });
                });
            }, ria.__SYNTAX.Modifiers.VOID, function bind_() {
                var ref = new ria.reflection.ReflectionClass(this.getClass());
                this._domEvents = ref.getMethodsReflector().filter(function(_) {
                    return _.isAnnotatedWith(ria.mvc.DomEventBind);
                }).map(function(_) {
                    if (_.getArguments().length != 2) throw new ria.mvc.MvcException("Methods, annotated with ria.mvc.DomBindEvent, are expected to accept two arguments (node, event)");
                    var annotation = _.findAnnotation(ria.mvc.DomEventBind).pop();
                    return {
                        event: annotation.event,
                        selector: annotation.selector_,
                        methodRef: _
                    };
                });
            } ]);
        })();
    })();
    "ria.mvc.DomControl";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.Base = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Base", ria.__SYNTAX.EXTENDS(ria.mvc.DomControl), []);
        })();
    })();
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            var r20 = /%20/g, rbracket = /\[\]$/, rCRLF = /\r?\n/g, rreturn = /\r/g, rsubmitterTypes = /^(?:submit|button|image|reset)$/i, manipulation_rcheckableType = /^(?:checkbox|radio)$/i, rsubmittable = /^(?:input|select|textarea|keygen)/i;
            function isNodeName(elem, name) {
                return elem.nodeName && elem.nodeName.toLowerCase() === name.toLowerCase();
            }
            var valHooks = {
                option: function(elem) {
                    var val = elem.attributes.value;
                    return !val || val.specified ? elem.value : elem.text;
                },
                select: function(elem) {
                    var value, option, options = elem.options, index = elem.selectedIndex, one = elem.type === "select-one" || index < 0, values = one ? null : [], max = one ? index + 1 : options.length, i = index < 0 ? max : one ? index : 0;
                    for (;i < max; i++) {
                        option = options[i];
                        if ((option.selected || i === index) && option.getAttribute("disabled") === null && (!option.parentNode.disabled || !isNodeName(option.parentNode, "optgroup"))) {
                            value = valueOfElement(option);
                            if (one) {
                                return value;
                            }
                            values.push(value);
                        }
                    }
                    return values;
                }
            };
            function valueOfElement(elem) {
                var ret;
                var hooks = valHooks[elem.type] || valHooks[elem.nodeName.toLowerCase()];
                if (hooks && (ret = hooks(elem, "value")) !== undefined) {
                    return ret;
                }
                ret = elem.value;
                return typeof ret === "string" ? ret.replace(rreturn, "") : ret == null ? "" : ret;
            }
            function isArray(obj) {
                return toString.call(obj) === "[object Array]";
            }
            function serializeForm(form) {
                var elements = form.elements || [];
                return ria.__API.clone(elements).filter(function(_) {
                    var type = _.type;
                    return _.name && !ria.dom.Dom(_).is(":disabled") && rsubmittable.test(_.nodeName) && !rsubmitterTypes.test(type) && (_.checked || !manipulation_rcheckableType.test(type));
                }).map(function(elem) {
                    if (elem.type.toLowerCase() == "file") return {
                        name: elem.name,
                        value: elem.files
                    };
                    var val = valueOfElement(elem);
                    return val == null ? null : isArray(val) ? val.map(function(val) {
                        return {
                            name: elem.name,
                            value: val.replace(rCRLF, "\r\n")
                        };
                    }) : {
                        name: elem.name,
                        value: val.replace(rCRLF, "\r\n")
                    };
                });
            }
            chlk.controls.ActionFormControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ActionFormControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_o2nzrmzbsj0jm7vi")(this);
            }, [ ria.mvc.DomEventBind("click", "FORM [type=submit]") ], [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function submitClicked_($target, event) {
                var $form = $target.parent("FORM");
                $form.setData("submit-name", $target.getAttr("name"));
                $form.setData("submit-value", $target.getValue());
                $form.setData("submit-skip", $target.hasClass("validate-skip"));
            }, [ ria.mvc.DomEventBind("submit", "FORM") ], [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function submit_($target, event) {
                if ($target.hasClass("disabled")) return false;
                var controller = $target.getData("controller");
                if (controller) {
                    var $form = jQuery($target.valueOf()[0]);
                    if (!$target.getData("submit-skip") && this.isOnlySubmitValidate()) {
                        $form.validationEngine("attach", {
                            onSuccess: function(form, status) {
                                $form.validationEngine("detach");
                            }
                        });
                    }
                    if ($form.validationEngine("validate")) {
                        var action = $target.getData("action");
                        var p = serializeForm($target.valueOf().shift());
                        var params = {};
                        p.forEach(function(o) {
                            params[o.name] = o.value;
                        });
                        var name = $target.getData("submit-name");
                        var value = $target.getData("submit-value");
                        if (name) {
                            params[name] = value;
                        }
                        $target.setData("submit-name", null);
                        $target.setData("submit-value", null);
                        var isPublic = !!$target.getData("public");
                        setTimeout(function() {
                            var state = this.context.getState();
                            state.setController(controller);
                            state.setAction(action || null);
                            state.setParams([ params ]);
                            state.setPublic(isPublic);
                            this.context.stateUpdated();
                        }.bind(this), 1);
                    }
                    return false;
                }
                return true;
            }, Boolean, "onlySubmitValidate", [ [ Object ] ], ria.__SYNTAX.Modifiers.VOID, function prepareData(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                if (attributes.onlySubmitValidate) {
                    this.setOnlySubmitValidate(true);
                } else {
                    this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                        jQuery("#" + attributes.id).validationEngine("attach");
                    }.bind(this));
                }
            } ]);
        })();
    })();
    __ASSETS._a2rog4oxpqm5nrk9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.LabeledCheckbox_mixin = function(text, name, val, isReadOnly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push('<div class="labeled-checkbox">');
            jade.globals.Checkbox_mixin.call({
                buf: buf,
                attributes: {
                    "class": "checkbox"
                },
                escaped: {}
            }, name, val, isReadOnly);
            buf.push("<label" + jade.attrs({
                "for": name
            }, {
                "for": true
            }) + ">" + jade.escape(null == (jade.interp = text) ? "" : jade.interp) + "</label></div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.LabeledCheckboxControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("LabeledCheckboxControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_a2rog4oxpqm5nrk9")(this);
            }, [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function changed_($target, event) {} ]);
        })();
    })();
    __ASSETS._9gtklxwo131bgldi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.SlideCheckbox_mixin = function(name, val, isReadOnly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "slide-checkbox"
            }, attributes), jade.merge({}, escaped, true)) + '><div class="wrapper">');
            attributes.checked = val;
            jade.globals.Checkbox_mixin.call({
                buf: buf,
                attributes: jade.merge({}, attributes),
                escaped: jade.merge({}, escaped, true)
            }, name, val);
            if (!isReadOnly) {
                buf.push("<label" + jade.attrs({
                    "for": name
                }, {
                    "for": true
                }) + "></label>");
            } else {
                buf.push("<label></label>");
            }
            buf.push("</div></div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.SlideCheckboxControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SlideCheckboxControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_9gtklxwo131bgldi")(this);
            } ]);
        })();
    })();
    __ASSETS._ss6l2rsak5nopqfr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.ActionLink_mixin = function(controller, action) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            var link = self.getLink(ria.__API.clone(arguments));
            attributes.href = attributes.href || "javascript:";
            attributes["data-link"] = link;
            buf.push("<A" + jade.attrs(jade.merge({
                "class": "action-link"
            }, attributes), jade.merge({}, escaped, true)) + ">");
            block && block();
            buf.push("</A>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            var lastClickedNode = null;
            chlk.controls.getActionLinkControlLastNode = function() {
                return lastClickedNode;
            };
            chlk.controls.ActionLinkControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ActionLinkControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_ss6l2rsak5nopqfr")(this);
            }, [ [ Array ] ], String, function getLink(values) {
                if (!values[2] || Array.isArray(values[2]) && !values[2].length) values.splice(2, 1);
                return encodeURIComponent(values.map(function(_) {
                    return Array.isArray(_) ? _.map(function(x) {
                        return JSON.stringify(x);
                    }).join(",") : JSON.stringify(_.valueOf ? _.valueOf() : _);
                }).join(","));
            }, [ [ String ] ], Array, function parseLink_(link) {
                return JSON.parse(String("[" + decodeURIComponent(link) + "]"));
            }, [ ria.mvc.DomEventBind("click", "A[data-link]:not(.disabled, .pressed)") ], [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function onActionLinkClick(node, event) {
                lastClickedNode = node;
                var link = node.getData("link");
                var args = this.parseLink_(link);
                var controller = args.shift(), action = args.shift();
                if (node.hasClass("defer")) {
                    setTimeout(function() {
                        this.updateState(controller, action, args);
                    }.bind(this), 10);
                } else {
                    this.updateState(controller, action, args);
                }
                event.preventDefault();
            }, ria.__SYNTAX.Modifiers.VOID, function updateState(controller, action, args) {
                var state = this.context.getState();
                state.setController(controller);
                state.setAction(action);
                state.setParams(args);
                state.setPublic(false);
                this.context.stateUpdated();
            } ]);
        })();
    })();
    __ASSETS._da00q73f1r2akyb9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Avatar_mixin = function(link, cls, border) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "avatar-container" + " " + ("" + (border ? "with-border" : "") + "")
            }, attributes), jade.merge({
                "class": true
            }, escaped, true)) + "><img" + jade.attrs({
                src: link,
                "class": "avatar" + " " + cls
            }, {
                "class": true,
                src: true
            }) + "/></div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.AvatarControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AvatarControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_da00q73f1r2akyb9")(this);
            } ]);
        })();
    })();
    __ASSETS._g5528ay2z8pk3xr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Button_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            var value = attributes.value;
            var type = attributes.type;
            var name = attributes.name;
            var attributes = self.processAttrs(attributes);
            var isDisabled = attributes.disabled === true ? "disabled" : undefined;
            buf.push("<span" + jade.attrs(jade.merge({
                "class": "chlk-button"
            }, attributes), jade.merge({}, escaped, true)) + "><span><span><button" + jade.attrs({
                type: type,
                name: name,
                value: value,
                disabled: isDisabled
            }, {
                type: true,
                name: true,
                value: true,
                disabled: true
            }) + ">");
            block && block();
            buf.push("</button></span></span></span>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.ButtonControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ButtonControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_g5528ay2z8pk3xr")(this);
            }, [ [ Object ] ], Object, function processAttrs(attributes) {
                if (attributes.disabled) if (Array.isArray(attributes.class)) {
                    attributes.class = attributes.class || [];
                    attributes.class.push("disabled");
                } else {
                    attributes.class = attributes.class + " disabled";
                }
                return attributes;
            } ]);
        })();
    })();
    __ASSETS._vrx1sigr0hfwp14i = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Checkbox_mixin = function(name, val, isReadOnly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes.checked = val;
            attributes.disabled = isReadOnly;
            self.prepareData(name, val);
            jade.globals.Hidden_mixin.call({
                buf: buf,
                attributes: {
                    "data-value": val,
                    "class": "hidden-checkbox"
                },
                escaped: {
                    "data-value": true,
                    "class": true
                }
            }, name, false);
            buf.push("<input" + jade.attrs(jade.merge({
                id: name,
                name: name,
                type: "checkbox"
            }, attributes), jade.merge({
                id: true,
                name: true,
                type: true
            }, escaped, true)) + "/>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.CheckboxControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("CheckboxControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_vrx1sigr0hfwp14i")(this);
            }, [ [ String, Boolean ] ], ria.__SYNTAX.Modifiers.VOID, function prepareData(name, value) {
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    var hidden = activity.getDom().find(".hidden-checkbox[name=" + name + "]");
                    activity.getDom().find("#" + name).on("change", function() {
                        var lastValue = hidden.getData("value");
                        var newValue = !lastValue;
                        hidden.setData("value", newValue);
                        hidden.setValue(newValue);
                    });
                    hidden.setData("value", value || false);
                }.bind(this));
            } ]);
        })();
    })();
    __ASSETS._8rwddvi2jawnrk9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.CheckboxList_mixin = function(listName, checkboxPref, defaultVal) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes["data-name"] = escape(listName);
            attributes["data-prefix"] = escape(checkboxPref);
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "checkbox-list"
            }, attributes), jade.merge({}, escaped, true)) + ">");
            jade.globals.Hidden_mixin.call({
                buf: buf
            }, listName, defaultVal);
            block && block();
            buf.push("</div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.CheckboxListControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("CheckboxListControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_8rwddvi2jawnrk9")(this);
            }, [ ria.mvc.DomEventBind("click", ".checkbox-list") ], [ [ ria.dom.Dom, ria.dom.Event ] ], function onClicked($target, node) {
                var checkboxes = $target.find("input[type=checkbox]");
                var res = [];
                var prefix = $target.getData("prefix");
                var name = $target.getData("name");
                checkboxes.filter(function(el) {
                    return el.is(":checked");
                }).forEach(function(el) {
                    res.push(el.getAttr("name").split(prefix).pop());
                });
                res = res.join(",");
                new ria.dom.Dom("input[name=" + name + "]").setValue(res);
            } ]);
        })();
    })();
    __ASSETS._9spda6cwx0drhpvi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.DatePicker_mixin = function(name, value_) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes = self.processAttrs(name, value_, attributes);
            buf.push("<input" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + "/>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.DatePickerControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DatePickerControl", ria.__SYNTAX.EXTENDS(ria.mvc.DomControl), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_9spda6cwx0drhpvi")(this);
            }, Date, "value", [ [ String, Object, Object ] ], Object, function processAttrs(name, value, attrs) {
                attrs.id = attrs.id || ria.dom.NewGID();
                attrs.name = name;
                if (value) attrs.value = value.format("mm/dd/yy");
                var options = attrs["data-options"];
                this.queueReanimation_(attrs.id, options);
                value && this.setValue(value.getDate());
                return attrs;
            }, ria.__SYNTAX.Modifiers.VOID, function queueReanimation_(id, options) {
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    this.reanimate_(ria.dom.Dom("#" + id), options, activity, model);
                }.bind(this));
            }, [ [ ria.dom.Dom, Object, ria.mvc.IActivity, Object ] ], ria.__SYNTAX.Modifiers.VOID, function reanimate_(node, options, activity, model) {
                var defaultOptions = {
                    dateFormat: "mm/dd/yy"
                };
                node.datepicker(ria.__API.extendWithDefault(options, defaultOptions), this.getValue());
            } ]);
        })();
    })();
    __ASSETS._i2l6uoh5jlvrt3xr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Select_mixin = function(name, controller, action, params) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes.name = name;
            attributes = self.processAttrs(attributes);
            attributes["data-controller"] = controller;
            attributes["data-action"] = action;
            attributes["data-params"] = params;
            buf.push("<select" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + ">");
            if (attributes.items) {
                (function() {
                    var $$obj = attributes.items;
                    if ("number" == typeof $$obj.length) {
                        for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                            var item = $$obj[$index];
                            jade.globals.Option_mixin.call({
                                buf: buf
                            }, item["get" + attributes.idParam.toUpperCase()], item["get" + attributes.nameParam.toUpperCase()]);
                        }
                    } else {
                        var $$l = 0;
                        for (var $index in $$obj) {
                            $$l++;
                            var item = $$obj[$index];
                            jade.globals.Option_mixin.call({
                                buf: buf
                            }, item["get" + attributes.idParam.toUpperCase()], item["get" + attributes.nameParam.toUpperCase()]);
                        }
                    }
                }).call(this);
            } else {
                block && block();
            }
            buf.push("</select>");
        };
        jade.globals.Option_mixin = function(value, displayName, selected) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes.value = value;
            attributes.selected = selected;
            {
                buf.push("<option" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + ">" + jade.escape(null == (jade.interp = displayName) ? "" : jade.interp) + "</option>");
            }
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.SelectControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SelectControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_i2l6uoh5jlvrt3xr")(this);
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.VOID, function updateSelect(node) {
                var that = this;
                node.chosen({
                    disable_search_threshold: 1e3
                }).change(function() {
                    var node = jQuery(this);
                    node.find("option[selected]").attr("selected", false);
                    node.find("option[value=" + node.val() + "]").attr("selected", true);
                    var controller = node.data("controller");
                    if (controller) {
                        var action = node.data("action");
                        var params = node.data("params") || [];
                        params.unshift(node.val());
                        var state = that.context.getState();
                        state.setController(controller);
                        state.setAction(action);
                        state.setParams(params);
                        state.setPublic(false);
                        that.context.stateUpdated();
                    }
                });
            }, [ [ Object ] ], Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    this.updateSelect(jQuery("#" + attributes.id));
                }.bind(this));
                return attributes;
            } ]);
        })();
    })();
    __ASSETS._mj8jphkmn4qp8pvi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.DateSelect_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            name = attributes.name;
            attributes = self.processAttrs(attributes);
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "date-select"
            }, attributes), jade.merge({}, escaped, true)) + "><input" + jade.attrs({
                type: "hidden",
                name: name,
                value: attributes.value ? attributes.value.toString("mm/dd/yy") : null,
                "class": "hidden-value"
            }, {
                type: true,
                name: true,
                value: true,
                "class": true
            }) + "/>");
            jade.globals.Select_mixin.call({
                buf: buf,
                block: function() {
                    (function() {
                        var $$obj = self.getMonths();
                        if ("number" == typeof $$obj.length) {
                            for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                var item = $$obj[$index];
                                jade.globals.Option_mixin.call({
                                    buf: buf
                                }, item[0], item[1], item[0] == attributes.month - 1);
                            }
                        } else {
                            var $$l = 0;
                            for (var $index in $$obj) {
                                $$l++;
                                var item = $$obj[$index];
                                jade.globals.Option_mixin.call({
                                    buf: buf
                                }, item[0], item[1], item[0] == attributes.month - 1);
                            }
                        }
                    }).call(this);
                },
                attributes: {
                    "data-placeholder": "Month",
                    "class": "month-combo"
                },
                escaped: {
                    "data-placeholder": true
                }
            }, "monthCombo");
            jade.globals.Select_mixin.call({
                buf: buf,
                block: function() {
                    (function() {
                        var $$obj = self.getDays();
                        if ("number" == typeof $$obj.length) {
                            for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                var item = $$obj[$index];
                                jade.globals.Option_mixin.call({
                                    buf: buf
                                }, item, item, item == attributes.day);
                            }
                        } else {
                            var $$l = 0;
                            for (var $index in $$obj) {
                                $$l++;
                                var item = $$obj[$index];
                                jade.globals.Option_mixin.call({
                                    buf: buf
                                }, item, item, item == attributes.day);
                            }
                        }
                    }).call(this);
                },
                attributes: {
                    "data-placeholder": "Day",
                    "class": "day-combo"
                },
                escaped: {
                    "data-placeholder": true
                }
            }, "dayCombo");
            jade.globals.Select_mixin.call({
                buf: buf,
                block: function() {
                    (function() {
                        var $$obj = self.getYears();
                        if ("number" == typeof $$obj.length) {
                            for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                var item = $$obj[$index];
                                jade.globals.Option_mixin.call({
                                    buf: buf
                                }, item, item, item == attributes.year);
                            }
                        } else {
                            var $$l = 0;
                            for (var $index in $$obj) {
                                $$l++;
                                var item = $$obj[$index];
                                jade.globals.Option_mixin.call({
                                    buf: buf
                                }, item, item, item == attributes.year);
                            }
                        }
                    }).call(this);
                },
                attributes: {
                    "data-placeholder": "Year",
                    "class": "year-combo"
                },
                escaped: {
                    "data-placeholder": true
                }
            }, "yearCombo");
            buf.push("</div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.DateSelectControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DateSelectControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_mj8jphkmn4qp8pvi")(this);
            }, Array, "days", Array, "months", Array, "years", [ [ Object ] ], Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                var value = attributes.value;
                console.info(value);
                delete attributes.name;
                if (value) {
                    attributes.day = value.getDate();
                    attributes.month = value.getMonth() + 1;
                    attributes.year = value.getFullYear();
                }
                var monthData = [ [ 0, Msg.January ], [ 1, Msg.February ], [ 2, Msg.March ], [ 3, Msg.April ], [ 4, Msg.May ], [ 5, Msg.June ], [ 6, Msg.July ], [ 7, Msg.August ], [ 8, Msg.September ], [ 9, Msg.October ], [ 10, Msg.November ], [ 11, Msg.December ] ];
                var daysData = [], days = 31, yearsData = [], currentYear = getDate().getFullYear();
                for (var i = 1; i <= days; i++) daysData.push(i);
                for (i = currentYear; i >= 1900; --i) {
                    yearsData.push(i);
                }
                this.setMonths(monthData);
                this.setDays(daysData);
                this.setYears(yearsData);
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    var container = new ria.dom.Dom("#" + attributes.id);
                    var daySelect = container.find(".day-combo");
                    var monthSelect = container.find(".month-combo");
                    var yearSelect = container.find(".year-combo");
                    var hidden = container.find(".hidden-value");
                    jQuery("#" + attributes.id).find("select").on("change", function(event, options) {
                        var dayValue = parseInt(daySelect.getValue(), 10);
                        var monthValue = (parseInt(monthSelect.getValue(), 10) || 0) + 1;
                        var yearValue = yearSelect.getValue();
                        if (!new ria.dom.Dom(this).hasClass("day-combo")) {
                            daySelect.setHTML("");
                            var days = daysInMonth(monthValue, yearValue || 2e3), wasSelected = false;
                            for (var i = 1; i <= days; i++) {
                                var option = new Option(i), selected = false;
                                if (i === dayValue) {
                                    selected = true;
                                    wasSelected = true;
                                }
                                if (i == days && !wasSelected) {
                                    selected = true;
                                    dayValue = i;
                                }
                                jQuery(option).attr("selected", selected);
                                daySelect.appendChild(option);
                            }
                            daySelect.trigger("liszt:updated");
                        }
                        if (dayValue && monthValue && yearValue) hidden.setValue(monthValue + "/" + dayValue + "/" + yearValue);
                    });
                }.bind(this));
                return attributes;
            } ]);
        })();
    })();
    __ASSETS._xaeixusor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.FileUpload_mixin = function(controller, action, params) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes["data-controller"] = controller;
            attributes["data-action"] = action;
            attributes["data-params"] = params;
            self.prepareData(attributes);
            buf.push("<input" + jade.attrs(jade.merge({
                type: "file"
            }, attributes), jade.merge({
                type: true
            }, escaped, true)) + "/>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.FileUploadControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("FileUploadControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_xaeixusor")(this);
            }, [ [ Object ] ], Object, function prepareData(attrs) {
                attrs.id = attrs.id || ria.dom.NewGID();
                var that = this, params = attrs["data-params"] || [], controller = attrs["data-controller"], action = attrs["data-action"];
                if (controller) {
                    this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                        var node = ria.dom.Dom("#" + attrs.id);
                        node.on("change", function(target, event) {
                            var files = target.valueOf()[0].files;
                            var state = that.context.getState();
                            params.push(files);
                            state.setController(controller);
                            state.setAction(action);
                            state.setParams(params);
                            state.setPublic(false);
                            that.context.stateUpdated();
                        });
                    }.bind(this));
                }
                return attrs;
            } ]);
        })();
    })();
    __ASSETS._xvd3uzjdwwylow29 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.GlanceBox_mixin = function(controller, action, data, color) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            if (controller) {
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    attributes: {
                        "class": "glance-box" + " " + "hover-action-box"
                    },
                    escaped: {}
                }, controller, action);
            } else {
                buf.push("<span" + jade.attrs({
                    "class": "glance-box" + (data.items ? " hover-action-box" : "")
                }, {
                    "class": true
                }) + "><div><div" + jade.attrs({
                    "class": "value" + " " + (self.getValueClass(data.value) + color ? " " + color : "")
                }, {
                    "class": true
                }) + ">" + jade.escape((jade.interp = data.value) == null ? "" : jade.interp) + '</div><p class="glance-title">' + jade.escape((jade.interp = data.title) == null ? "" : jade.interp) + "</p>");
                if (data.items) {
                    buf.push('<div class="details">');
                    (function() {
                        var $$obj = data.items;
                        if ("number" == typeof $$obj.length) {
                            for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                var item = $$obj[i];
                                buf.push("<div>" + jade.escape((jade.interp = self.getShortText(item.getTotal(), item.getSummary())) == null ? "" : jade.interp) + "</div>");
                            }
                        } else {
                            var $$l = 0;
                            for (var i in $$obj) {
                                $$l++;
                                var item = $$obj[i];
                                buf.push("<div>" + jade.escape((jade.interp = self.getShortText(item.getTotal(), item.getSummary())) == null ? "" : jade.interp) + "</div>");
                            }
                        }
                    }).call(this);
                    buf.push("</div>");
                }
                buf.push("</div></span>");
            }
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.GlanceBoxControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("GlanceBoxControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_xvd3uzjdwwylow29")(this);
            }, [ [ Number ] ], String, function getValueClass(value) {
                var res = "";
                if (value >= 100 && value < 1e3) res = "large"; else if (value >= 1e3) res = "small";
                return res;
            }, [ [ Number, String ] ], String, function getShortText(value1, value2) {
                var res = value1 + " " + value2;
                if (value1 !== undefined && value2 !== undefined) {
                    if (value1.length + value2.length > 9) {
                        res = value1 + " " + value2.slice(0, 8 - value1.length) + "...";
                    }
                }
                return res;
            } ]);
        })();
    })();
    __ASSETS._pt2g0mbabdx9lik9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Grid_mixin = function(controller, action, data, params) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            console.info(controller, action, data, params);
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "grid"
            }, attributes), jade.merge({}, escaped, true)) + '><div class="loader"></div><div class="scroller"><div class="container">');
            block && block();
            buf.push("</div></div>");
            if (controller) {
                jade.globals.Paginator_mixin.call({
                    buf: buf
                }, controller, action, data, params);
            }
            buf.push("</div>");
        };
        jade.globals.GridHead_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push('<div class="head panel-bg">');
            block && block();
            buf.push("</div>");
        };
        jade.globals.GridBody_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push('<div class="body">');
            block && block();
            buf.push("</div>");
        };
        jade.globals.GridRow_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push('<div class="row">');
            block && block();
            buf.push("</div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.GridControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("GridControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_pt2g0mbabdx9lik9")(this);
            } ]);
        })();
    })();
    __ASSETS._rmpex2bgtlu15rk9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.LeftRightToolbar_mixin = function(data, tplClass, controller, action, params) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            configs = self.prepareData(data, attributes);
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "lr-toolbar"
            }, attributes), jade.merge({}, escaped, true)) + "><div" + jade.attrs({
                "class": "A" + " " + (configs.disablePrevButton ? configs.disabledClass : "") + " " + "prev-button" + " " + "arrow"
            }, {}) + '></div><div class="first-container"><div class="second-container">');
            (function() {
                var $$obj = data;
                if ("number" == typeof $$obj.length) {
                    for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                        var item = $$obj[i];
                        item.setPressed && item.setPressed(configs.selectedItemId == item.getId().valueOf());
                        item.setDisabled && item.setDisabled(!configs.pressAfterClick);
                        item.setIndex && item.setIndex(i);
                        if (controller) {
                            item.setController(controller);
                            item.setAction(action);
                            item.setParams(params);
                        }
                        jade.globals.RenderWith_mixin.call({
                            buf: buf
                        }, item, tplClass);
                    }
                } else {
                    var $$l = 0;
                    for (var i in $$obj) {
                        $$l++;
                        var item = $$obj[i];
                        item.setPressed && item.setPressed(configs.selectedItemId == item.getId().valueOf());
                        item.setDisabled && item.setDisabled(!configs.pressAfterClick);
                        item.setIndex && item.setIndex(i);
                        if (controller) {
                            item.setController(controller);
                            item.setAction(action);
                            item.setParams(params);
                        }
                        jade.globals.RenderWith_mixin.call({
                            buf: buf
                        }, item, tplClass);
                    }
                }
            }).call(this);
            buf.push("</div></div><div" + jade.attrs({
                "class": "A" + " " + (configs.disableNextButton ? configs.disabledClass : "") + " " + "next-button" + " " + "arrow"
            }, {}) + "></div>");
            if (configs.needDots) {
                buf.push('<div class="paginator">');
                (function() {
                    var $$obj = configs.dots;
                    if ("number" == typeof $$obj.length) {
                        for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                            var dot = $$obj[i];
                            buf.push("<A" + jade.attrs({
                                index: i,
                                "class": dot ? "current" : ""
                            }, {
                                index: true,
                                "class": true
                            }) + "></A>");
                        }
                    } else {
                        var $$l = 0;
                        for (var i in $$obj) {
                            $$l++;
                            var dot = $$obj[i];
                            buf.push("<A" + jade.attrs({
                                index: i,
                                "class": dot ? "current" : ""
                            }, {
                                index: true,
                                "class": true
                            }) + "></A>");
                        }
                    }
                }).call(this);
                buf.push("</div>");
            }
            buf.push("</div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            var hideClass = "x-hidden";
            chlk.controls.LeftRightToolbarControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("LeftRightToolbarControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_rmpex2bgtlu15rk9")(this);
                this.setDefaultConfigs({
                    itemsCount: 8,
                    fixedPadding: false,
                    hideArrows: false,
                    disabledClass: "disabled",
                    disablePrevButton: true,
                    disableNextButton: true,
                    needDots: true,
                    pressedClass: "pressed",
                    pressAfterClick: true
                });
            }, Object, "configs", Object, "defaultConfigs", Object, "currentIndex", [ [ Object, Object ] ], Object, function prepareData(data, attributes_) {
                var configs = this.getDefaultConfigs();
                if (attributes_) {
                    configs = Object.extend(configs, attributes_);
                }
                if (configs.hideArrows) configs.disabledClass = hideClass;
                if (data.length > configs.itemsCount) {
                    configs.disableNextButton = false;
                    if (configs.needDots) {
                        var dots = [], dotsCount = Math.ceil(data.length / configs.itemsCount);
                        for (var i = 0; i < dotsCount; i++) dots.push(i == 0);
                        configs.dots = dots;
                    }
                } else {
                    configs.needDots = false;
                }
                this.setConfigs(configs);
                this.setCurrentIndex(0);
                var that = this;
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    var toolbar = activity.getDom().find(".lr-toolbar");
                    if (configs.pressAfterClick) {
                        var pressedIndex = parseInt(toolbar.find(".pressed").getAttr("index"), 10);
                        var pageIndex = Math.floor(pressedIndex / configs.itemsCount);
                        that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + pageIndex + '"]'), toolbar);
                        toolbar.on("click", ".second-container>*:not(.pressed)", function(node, event) {
                            toolbar.find(".second-container>.pressed").removeClass("pressed");
                            setTimeout(function() {
                                node.addClass("pressed");
                            }, 1);
                        });
                    }
                    toolbar.on("click", ".arrow:not(.disabled)", function(node, event) {
                        var index = that.getCurrentIndex();
                        if (node.hasClass("prev-button")) {
                            that.setCurrentIndex(--index);
                            if (configs.fixedPadding) {
                                that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), toolbar);
                            } else {
                                that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), toolbar);
                            }
                        } else {
                            that.setCurrentIndex(++index);
                            if (configs.fixedPadding) {
                                that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), toolbar);
                            } else {
                                that.setPageByCurrentDot(toolbar.find('.paginator A[index="' + index + '"]'), toolbar);
                            }
                        }
                    });
                    toolbar.find(".paginator").on("click", "a:not(.current)", function(node, event) {
                        that.setPageByCurrentDot(node, toolbar);
                        return false;
                    });
                }.bind(this));
                return configs;
            }, [ [ ria.dom.Dom, ria.dom.Dom ] ], ria.__SYNTAX.Modifiers.VOID, function setPageByCurrentDot(node, toolbar) {
                if (this.getConfigs().needDots) {
                    var index = parseInt(node.getAttr("index"), 10);
                    this.setCurrentIndex(index);
                    var nextButton = toolbar.find(".next-button");
                    var prevButton = toolbar.find(".prev-button");
                    toolbar.find(".paginator .current").removeClass("current");
                    node.addClass("current");
                    var width = toolbar.find(".first-container").width();
                    var secondContainer = toolbar.find(".second-container");
                    secondContainer.setCss("left", -width * index);
                    if (index == 0) {
                        prevButton.addClass("disabled");
                    } else {
                        prevButton.removeClass("disabled");
                    }
                    if (index == this.getConfigs().dots.length - 1) {
                        nextButton.addClass("disabled");
                    } else {
                        nextButton.removeClass("disabled");
                    }
                }
            } ]);
        })();
    })();
    __ASSETS._2wgnmpjxjhnf80k9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.ListView_mixin = function(data) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            self.prepareData(data, attributes);
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "chlk-grid"
            }, attributes), jade.merge({}, escaped, true)) + '><input class="grid-focus"/>');
            block && block();
            buf.push("</div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            var otherInputWithFocusClass = "with-grid-focus";
            chlk.controls.ListViewControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ListViewControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_2wgnmpjxjhnf80k9")(this);
                this.setDefaultConfigs({
                    selectedIndex: null,
                    infiniteScroll: false,
                    itemsName: "Items",
                    start: 0,
                    totalCount: null,
                    pageSize: 10,
                    interval: 250
                });
            }, Object, "configs", Object, "defaultConfigs", [ [ Object, Object ] ], ria.__SYNTAX.Modifiers.VOID, function prepareData(data, configs_) {
                var configs = this.getDefaultConfigs();
                if (configs_) {
                    if (data.getTotalCount) {
                        configs_.totalCount = data.getTotalCount();
                        configs_.pageSize = data.getPageSize();
                    }
                    configs = Object.extend(configs, configs_);
                }
                var getItemsMethod = "get" + configs.itemsName;
                configs.selectedIndex !== undefined && this.setCurrentIndex(configs.selectedIndex);
                this.setConfigs(configs);
                this.setCount(data.length || data[getItemsMethod]().length);
                this.context.getDefaultView().getCurrent().addRefreshCallback(function(activity, model) {
                    if (this.getCurrentIndex() !== undefined && !new ria.dom.Dom(":focus").exists()) this.focusGrid();
                    var grid = new ria.dom.Dom(".chlk-grid");
                    this.setGrid(grid);
                    if (configs.infiniteScroll && !grid.hasClass("with-scroller")) this.addInfiniteScroll(grid);
                }.bind(this));
            }, [ [ ria.dom.Dom ] ], ria.__SYNTAX.Modifiers.VOID, function addInfiniteScroll(grid) {
                grid.addClass("with-scroller");
                var baseContentHeight = grid.height(), configs = this.getConfigs();
                var pageHeight = document.documentElement.clientHeight, scrollPosition, contentHeight = baseContentHeight + grid.offset().top, interval;
                configs.currentStart = configs.start + configs.pageSize;
                interval = setInterval(function() {
                    if (!grid.hasClass("scroll-freezed")) {
                        scrollPosition = window.pageYOffset;
                        if (configs.totalCount > configs.currentStart) {
                            if (contentHeight - pageHeight - scrollPosition < 400) {
                                var form = grid.parent("form");
                                form.find("[name=start]").setValue(configs.currentStart);
                                jQuery(grid.valueOf()).parents("form").find(".scroll-start-button").click();
                                configs.currentStart += configs.pageSize;
                                var div = new ria.dom.Dom('<div class="horizontal-loader"></div>');
                                grid.addClass("scroll-freezed");
                                grid.appendChild(div);
                                contentHeight += baseContentHeight;
                            }
                        } else {
                            clearInterval(interval);
                        }
                    }
                    var node = grid.find(".horizontal-loader");
                    if (node.exists() && node.next().exists()) {
                        grid.removeClass("scroll-freezed");
                        node.remove();
                    }
                }, configs.interval);
            }, Number, "currentIndex", Number, "count", ria.dom.Dom, "grid", [ ria.mvc.DomEventBind("click", ".chlk-grid .row:not(.selected)") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function rowClick(node, event) {
                var parent = node.parent(".chlk-grid");
                parent.find(".row.selected").removeClass("selected");
                node.addClass("selected");
                this.setCurrentIndex(parseInt(node.getAttr("index"), 10));
            }, [ ria.mvc.DomEventBind("keydown", ".grid-focus, ." + otherInputWithFocusClass) ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function keyArrowsClick(node, event) {
                var currentIndex = this.getCurrentIndex();
                if (event.which == 38 && currentIndex || event.which == 40 && currentIndex < this.getCount() - 1) {
                    var parent = node.parent(".chlk-grid");
                    parent.find(".row.selected").removeClass("selected");
                    if (event.which == 38) {
                        currentIndex--;
                    } else {
                        currentIndex++;
                    }
                    parent.find('.row[index="' + currentIndex + '"]').addClass("selected");
                    this.setCurrentIndex(currentIndex);
                    this.scrollToElement();
                    this.focusGrid();
                }
            }, [ ria.mvc.DomEventBind("click", ".chlk-grid") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function onFocusGrid(node, event) {
                var target = new ria.dom.Dom(event.target);
                if (target.is(":not(input, textarea)")) {
                    var Y = window.scrollY;
                    this.focusGrid(node);
                    window.scrollTo(0, Y);
                }
            }, [ [ ria.dom.Dom ] ], ria.__SYNTAX.Modifiers.VOID, function focusGrid(node_) {
                node_ = node_ && node_.valueOf()[0] ? node_ : new ria.dom.Dom(".chlk-grid");
                var focusNode = node_.find(".row.selected").find("." + otherInputWithFocusClass);
                if (focusNode.exists()) {
                    focusNode.valueOf()[0].focus();
                } else {
                    node_.find(".grid-focus").valueOf()[0].focus();
                }
            }, ria.__SYNTAX.Modifiers.VOID, function scrollToElement() {
                var el = this.getGrid().find(".row.selected");
                var demo = new ria.dom.Dom("#demo-footer");
                window.scrollTo(0, el.offset().top + el.height() / 2 - (window.innerHeight - (demo.height() || 0)) / 2);
            } ]);
        })();
    })();
    __ASSETS._ryilsm3w4z8semi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.LoadingImage_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes = self.processAttrs(attributes);
            buf.push("<img" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + "/>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.LoadingImgControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("LoadingImgControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_ryilsm3w4z8semi")(this);
            }, [ [ Number, Object ] ], ria.__SYNTAX.Modifiers.VOID, function checkImage(timeOut, img) {
                var parent = img.parent();
                setTimeout(function() {
                    if (img.height() <= parent.height() / 2 && img.width() <= img.parent().width() / 2) {
                        img.setCss("visibility", "hidden");
                        parent.addClass("loading");
                        var src = img.getAttr("src");
                        img.setAttr("src", src);
                        this.checkImage(timeOut > 10 ? 10 : timeOut * 2, img);
                    } else {
                        img.setCss("visibility", "visible");
                        parent.removeClass("loading");
                    }
                }.bind(this), timeOut * 100);
            }, [ [ Object ] ], Object, function processAttrs(attrs) {
                attrs.id = attrs.id || ria.dom.NewGID();
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    var img = new ria.dom.Dom("#" + attrs.id);
                    this.checkImage(1, img);
                }.bind(this));
                return attrs;
            } ]);
        })();
    })();
    __ASSETS._vfh3bbb5kqzuayvi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Paginator_mixin = function(controller, action, data, params) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            var params = params || [];
            var pagingData = self.preparePaginationData(data, params);
            buf.push('<div class="paginator-container"><div class="left-container">');
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "first-page" + (pagingData.hasPreviousPage ? "" : " disabled")
                },
                escaped: {
                    "class": true
                }
            }, controller, action, params, 0);
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "prev-page" + (pagingData.hasPreviousPage ? "" : " disabled")
                },
                escaped: {
                    "class": true
                }
            }, controller, action, params, pagingData.prevPageStart);
            buf.push('<div class="info-container"><form><label for="current_page">Page</label><input' + jade.attrs({
                name: "current_page",
                value: pagingData.pageIndex,
                "class": "page-value"
            }, {
                name: true,
                value: true
            }) + "/><span>of " + jade.escape((jade.interp = pagingData.totalPages) == null ? "" : jade.interp) + '</span><input type="submit"/><input' + jade.attrs({
                type: "hidden",
                name: "controller",
                value: controller,
                "class": "controller-name"
            }, {
                type: true,
                name: true,
                value: true
            }) + "/><input" + jade.attrs({
                type: "hidden",
                name: "action",
                value: action,
                "class": "action-name"
            }, {
                type: true,
                name: true,
                value: true
            }) + "/></form></div>");
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "next-page" + (pagingData.hasNextPage ? "" : " disabled")
                },
                escaped: {
                    "class": true
                }
            }, controller, action, params, pagingData.nextPageStart);
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "last-page" + (pagingData.hasNextPage ? "" : " disabled")
                },
                escaped: {
                    "class": true
                }
            }, controller, action, params, pagingData.lastPageStart);
            buf.push('</div><div class="right-container"><SPAN>Displaying ' + jade.escape((jade.interp = pagingData.startText) == null ? "" : jade.interp) + " - " + jade.escape((jade.interp = pagingData.end) == null ? "" : jade.interp) + " of " + jade.escape((jade.interp = pagingData.totalCount) == null ? "" : jade.interp) + '</SPAN></div><div class="clear"></div></div>');
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    "chlk.controls.ActionLinkControl";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.PaginatorControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("PaginatorControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_vfh3bbb5kqzuayvi")(this);
            }, [ ria.mvc.DomEventBind("submit", ".paginator-container form") ], [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function onPrevPageClick(node, event) {
                try {
                    var state = this.context.getState();
                    var configs = this.getConfigs();
                    var action = node.find(".action-name").getAttr("value");
                    state.setAction(node.find(".action-name").getValue());
                    var pageNode = node.find(".page-value");
                    var value = (parseInt(pageNode.getValue(), 10) - 1) * configs.pageSize;
                    if (value > configs.lastPageStart) value = configs.lastPageStart;
                    if (value < 0) value = 0;
                    pageNode.setValue(1 + value / configs.pageSize);
                    var params = this.getLinkParams().slice();
                    params.push(value);
                    state.setParams(params);
                    state.setPublic(false);
                    this.context.stateUpdated();
                } catch (e) {
                    console.info(e.getMessage());
                }
                return false;
            }, Object, "configs", Object, "linkParams", [ [ Object, Array ] ], Object, function preparePaginationData(data, params_) {
                var start = data.getPageIndex() * data.getPageSize();
                this.setLinkParams(params_);
                var res = {
                    hasPreviousPage: data.isHasPreviousPage() | 0,
                    hasNextPage: data.isHasNextPage() | 0,
                    lastPageStart: (data.getTotalPages() - 1) * data.getPageSize() | 0,
                    prevPageStart: start - data.getPageSize() | 0,
                    nextPageStart: start + data.getPageSize() | 0,
                    pageIndex: data.getPageIndex() + 1 | 0,
                    totalCount: data.getTotalCount() | 0,
                    totalPages: data.getTotalPages() | 0,
                    startText: start + 1 | 0,
                    end: start + data.getPageSize() | 0,
                    pageSize: data.getPageSize() | 0
                };
                this.setConfigs(res);
                return res;
            } ]);
        })();
    })();
    __ASSETS._ydqtn0kscsaif6r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.PhotoContainer_mixin = function(link) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "photo-conainer"
            }, attributes), jade.merge({}, escaped, true)) + ">");
            jade.globals.Avatar_mixin.call({
                buf: buf,
                block: function() {
                    block && block();
                },
                attributes: {
                    "class": "photo-avatar"
                },
                escaped: {}
            }, link, "avatar-256", false);
            buf.push("</div>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.PhotoContainerControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("PhotoContainerControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_ydqtn0kscsaif6r")(this);
            } ]);
        })();
    })();
    __ASSETS._b7v3ifduos5ah5mi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Video_mixin = function(iframe) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes = self.processAttrs(attributes);
            if (iframe) {
                buf.push("<iframe" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + "></iframe>");
            } else {
                buf.push("<emded" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + "></emded>");
            }
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.VideoControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("VideoControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_b7v3ifduos5ah5mi")(this);
                this.setConfigs({
                    wmode: "transparent",
                    allowScriptAccess: "always",
                    allowfullscreen: "true",
                    frameborder: 0,
                    vspace: 0,
                    hspace: 0
                });
            }, Object, "configs", [ [ String ] ], String, function getVideoUrl(url) {
                if (url.indexOf("vimeo") > -1 || url.indexOf("/") == -1) {
                    url = "https://player.vimeo.com/video/" + (url && url.slice(url.lastIndexOf("/") + 1));
                }
                if (url.indexOf("youtube") > -1) {
                    url = "https://www.youtube.com/v/" + (url && url.slice(url.lastIndexOf("v=") + 2));
                }
                return url;
            }, [ [ Object ] ], Object, function processAttrs(attributes) {
                attributes.src = this.getVideoUrl(attributes.src);
                attributes = ria.__API.extendWithDefault(attributes, this.getConfigs());
                return attributes;
            } ]);
        })();
    })();
    __ASSETS._bjeo0opxqg2e29 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Logout_mixin = function(controller, action, userName) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push('<div class="logout-area">' + jade.escape(null == (jade.interp = userName) ? "" : jade.interp) + '</div><span id="logout-arrow" class="logout-area"></span>');
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("log out");
                },
                attributes: {
                    "class": "logout"
                },
                escaped: {}
            }, controller, action);
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.LogoutControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("LogoutControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_bjeo0opxqg2e29")(this);
                this.setLogoutShown(false);
            }, Boolean, "logoutShown", [ ria.mvc.DomEventBind("click", ".logout-area") ], [ [ ria.dom.Dom, ria.dom.Event ] ], function onClicked($target, node) {
                var elem = $target.parent().find("a");
                if (!this.isLogoutShown()) {
                    elem.setCss("visibility", "visible").setCss("opacity", 1);
                } else {
                    elem.setCss("opacity", 0);
                    setTimeout(function() {
                        elem.setCss("visibility", "hidden");
                    }, 200);
                }
                this.setLogoutShown(!this.isLogoutShown());
            } ]);
        })();
    })();
    __ASSETS._qof00lqdrmp9zfr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.TextArea_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes = self.processAttrs(attributes);
            buf.push("<textarea" + jade.attrs(jade.merge({
                "class": "animated-textarea"
            }, attributes), jade.merge({}, escaped, true)) + ">");
            block && block();
            buf.push("</textarea>");
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.TextAreaControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TextAreaControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_qof00lqdrmp9zfr")(this);
            }, [ [ Object ] ], Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    jQuery("textarea#" + attributes.id).autosize();
                }.bind(this));
                return attributes;
            } ]);
        })();
    })();
    __ASSETS._41crbiipexy74x6r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.SearchBox_mixin = function(service, method, tpl) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            self.initialize(attributes, service, method, tpl);
            buf.push("<input" + jade.attrs(jade.merge({
                type: "text",
                "search-box": "search-box"
            }, attributes), jade.merge({
                type: true,
                "search-box": true
            }, escaped, true)) + "/><input" + jade.attrs({
                id: attributes.id + "-hidden",
                name: attributes.name,
                type: "hidden"
            }, {
                id: true,
                name: true,
                type: true
            }) + "/><div></div>");
        };
        return buf.join("");
    };
    "ria.async.ICancelable";
    (function() {
        (ria = ria || {}).async = ria.async || {};
        (function() {
            "use strict";
            ria.async.TimerDelegate = ria.__API.delegate("ria.async.TimerDelegate", null, [ Object, Number ], [ "timer", "lag" ]);
            ria.async.Timer = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.async." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Timer", ria.__SYNTAX.IMPLEMENTS(ria.async.ICancelable), [ [ [ Number, ria.async.TimerDelegate ] ], function $(duration, handler) {
                var me = this;
                var lastCall = new Date();
                this.cleaner = clearInterval;
                this.timer = setInterval(function() {
                    handler(me, -(lastCall.getTime() - (lastCall = new Date()).getTime()));
                }, duration < 0 ? 0 : duration);
            }, ria.__SYNTAX.Modifiers.VOID, function cancel() {
                this.cleaner(this.timer);
            } ]);
        })();
    })();
    "ria.async.Task";
    "ria.async.Timer";
    (function() {
        (ria = ria || {}).ajax = ria.ajax || {};
        (function() {
            "use strict";
            ria.ajax.Method =             function wrapper() {
                var values = {};
                function Method(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(Method, "ria.ajax.Method");
                function MethodImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[ria.ajax.Method#" + value + "]";
                    };
                }
                ria.__API.extend(MethodImpl, Method);
                values["get"] = Method.GET = new MethodImpl("get");
                values["post"] = Method.POST = new MethodImpl("post");
                values["put"] = Method.PUT = new MethodImpl("put");
                values["delete"] = Method.DELETE = new MethodImpl("delete");
                return Method;
            }();
            ria.ajax.Task = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.ajax." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Task", ria.__SYNTAX.EXTENDS(ria.async.Task), [ [ [ String, ria.ajax.Method, Object ] ], function $(url, method_, params_) {
                BASE();
                this._method = method_;
                this._url = url;
                this._params = params_ || {};
                this._requestTimeout = null;
                this._xhr = new XMLHttpRequest();
                this._xhr.addEventListener("progress", this.updateProgress_, false);
                this._xhr.addEventListener("load", this.transferComplete_, false);
                this._xhr.addEventListener("error", this.transferFailed_, false);
                this._xhr.addEventListener("abort", this.transferCanceled_, false);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, function cancel() {
                this._xhr.abort();
            }, [ [ ria.ajax.Method ] ], ria.__SYNTAX.Modifiers.SELF, function method(method) {
                this._method = method;
                return this;
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.SELF, function params(obj) {
                var p = this._params;
                for (var key in obj) if (obj.hasOwnProperty(key) && obj[key] != undefined && obj[key] != null) {
                    p[key] = obj[key];
                }
                return this;
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.SELF, function disableCache(paramName_) {
                this._params[paramName_ || "_"] = Math.random().toString(36).substr(2) + new Date().getTime().toString(36);
                return this;
            }, [ [ Number ] ], ria.__SYNTAX.Modifiers.SELF, function timeout(duration) {
                this._requestTimeout = duration;
                return this;
            }, ria.__SYNTAX.Modifiers.FINAL, String, function getParamsAsQueryString_() {
                var p = this._params, r = [];
                for (var key in p) if (p.hasOwnProperty(key)) {
                    r.push([ key, p[key] ].map(encodeURIComponent).join("="));
                }
                return r.join("&");
            }, ria.__SYNTAX.Modifiers.FINAL, String, function getParamsString_() {
                var p = this._params, r = [];
                for (var key in p) if (p.hasOwnProperty(key)) {
                    r.push([ key, p[key] ].join("="));
                }
                return r.join("&");
            }, ria.__SYNTAX.Modifiers.VOID, function updateProgress_(oEvent) {
                this._completer.progress(oEvent);
            }, ria.__SYNTAX.Modifiers.VOID, function transferComplete_(evt) {
                this._completer.complete(this._xhr.responseText);
            }, ria.__SYNTAX.Modifiers.VOID, function transferFailed_(evt) {
                this._completer.completeError(Error(evt));
            }, ria.__SYNTAX.Modifiers.VOID, function transferCanceled_(evt) {
                this._completer.cancel();
            }, String, function getUrl_() {
                if (this._method != ria.ajax.Method.GET) return this._url;
                return this._url + (/\?/.test(this._url) ? "&" : "?") + this.getParamsAsQueryString_();
            }, Object, function getBody_() {
                return this._method != ria.ajax.Method.GET ? JSON.stringify(this._params) : this.getParamsString_();
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function do_() {
                try {
                    BASE();
                    this._xhr.open(this._method.valueOf(), this.getUrl_(), true);
                    if (this._method != ria.ajax.Method.GET) {
                        if (this._params) {
                            this._xhr.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                        }
                    }
                    this._xhr.send(this.getBody_());
                } catch (e) {
                    this._completer.completeError(e);
                }
                this._requestTimeout && new ria.async.Timer(this._requestTimeout, this.timeoutHandler_);
            }, [ [ ria.async.Timer, Number ] ], ria.__SYNTAX.Modifiers.VOID, function timeoutHandler_(timer, lag) {
                timer.cancel();
                this._completer.isCompleted() || this.cancel();
            } ]);
        })();
    })();
    "ria.ajax.Task";
    (function() {
        (ria = ria || {}).ajax = ria.ajax || {};
        (function() {
            "use strict";
            ria.ajax.JsonGetTask = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.ajax." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("JsonGetTask", ria.__SYNTAX.EXTENDS(ria.ajax.Task), [ function $(url) {
                BASE(url, ria.ajax.Method.GET);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.async.Future, function run() {
                return BASE().then(function(data) {
                    return JSON.parse(data);
                });
            } ]);
        })();
    })();
    "ria.ajax.Task";
    (function() {
        (ria = ria || {}).ajax = ria.ajax || {};
        (function() {
            "use strict";
            ria.ajax.JsonPostTask = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.ajax." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("JsonPostTask", ria.__SYNTAX.EXTENDS(ria.ajax.Task), [ function $(url) {
                BASE(url, ria.ajax.Method.POST);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.async.Future, function run() {
                return BASE().then(function(data) {
                    return JSON.parse(data);
                });
            } ]);
        })();
    })();
    "ria.ajax.Task";
    (function() {
        (ria = ria || {}).ajax = ria.ajax || {};
        (function() {
            "use strict";
            ria.ajax.UploadFileTask = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.ajax." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("UploadFileTask", ria.__SYNTAX.EXTENDS(ria.ajax.Task), [ [ [ String, Object ] ], function $(url, files) {
                BASE(url, ria.ajax.Method.POST);
                this._files = files;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.async.Future, function run() {
                return BASE().then(function(data) {
                    return JSON.parse(data);
                });
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function do_() {
                try {
                    var formData = new FormData();
                    for (var i = 0, file; file = this._files[i]; ++i) {
                        formData.append(file.name, file);
                        for (var param in this._params) if (this._params.hasOwnProperty(param)) {
                            formData.append(param, this._params[param]);
                        }
                    }
                    this._xhr.open(this._method.valueOf(), this.getUrl_(), true);
                    this._xhr.send(formData);
                } catch (e) {
                    this._completer.completeError(e);
                }
                this._requestTimeout && new ria.async.Timer(this._requestTimeout, this.timeoutHandler_);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.PaginatedList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("PaginatedList", [ [ [ Function ] ], function $(itemClass) {
                this.itemClass = itemClass;
            }, ria.__SYNTAX.Modifiers.READONLY, Function, "itemClass", ria.__API.ArrayOf(Object), "items", Number, "pageIndex", Number, "pageSize", Number, "totalCount", Number, "totalPages", Boolean, "hasNextPage", Boolean, "hasPreviousPage", ria.__SYNTAX.Modifiers.VOID, function setItems(values) {
                null;
                this.items = values;
            } ]);
        })();
    })();
    "ria.serialize.JsonSerializer";
    "ria.ajax.JsonGetTask";
    "ria.ajax.JsonPostTask";
    "ria.ajax.UploadFileTask";
    "chlk.models.common.PaginatedList";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.DataException = function ExceptionCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateException(def);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DataException", [ function $(msg, inner_) {
                BASE(msg, inner_);
            } ]);
            var Serializer = new ria.serialize.JsonSerializer();
            chlk.services.BaseService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BaseService", ria.__SYNTAX.IMPLEMENTS(ria.mvc.IContextable), [ ria.mvc.IContext, "context", String, function getServiceRoot() {
                return this.getContext().getSession().get("siteRoot");
            }, [ [ String ] ], String, function resolveUri(uri) {
                return this.getServiceRoot() + uri;
            }, String, function getUrl(uri, params) {
                var p = params, r = [];
                for (var key in p) if (p.hasOwnProperty(key)) {
                    r.push([ key, p[key] ].join("="));
                }
                return this.resolveUri(uri) + "?" + r.join("&");
            }, [ [ String, Object, Object ] ], ria.async.Future, function get(uri, clazz_, gParams_) {
                return new ria.ajax.JsonGetTask(this.resolveUri(uri)).params(gParams_ || {}).run().then(function(data) {
                    if (!data.success) throw chlk.services.DataException("Server error", Error(data.message));
                    if (!clazz_) return data.data || null;
                    return Serializer.deserialize(data.data, clazz_);
                });
            }, [ [ String, Object, Object, Object ] ], ria.async.Future, function uploadFiles(uri, files, clazz_, gParams_) {
                return new ria.ajax.UploadFileTask(this.resolveUri(uri), files).params(gParams_ || {}).run().then(function(data) {
                    if (!data.success) throw chlk.services.DataException("Server error", Error(data.message));
                    if (!clazz_) return data.data || null;
                    return Serializer.deserialize(data.data, clazz_);
                });
            }, [ [ String, Object, Object ] ], ria.async.Future, function post(uri, clazz, gParams) {
                return new ria.ajax.JsonPostTask(this.resolveUri(uri)).params(gParams).run().then(function(data) {
                    if (!data.success) throw chlk.services.DataException("Server error", Error(data.message));
                    return Serializer.deserialize(data.data, clazz);
                });
            }, [ [ String, Object, Object ] ], ria.async.Future, function postArray(uri, clazz, gParams) {
                return new ria.ajax.JsonPostTask(this.resolveUri(uri)).params(gParams).run().then(function(data) {
                    if (!data.success) throw chlk.services.DataException("Server error", Error(data.message));
                    return Serializer.deserialize(data.data, ria.__API.ArrayOf(clazz));
                });
            }, [ [ String, Object, Object ] ], ria.async.Future, function getArray(uri, clazz, gParams) {
                return new ria.ajax.JsonGetTask(this.resolveUri(uri)).params(gParams).run().then(function(data) {
                    if (!data.success) throw chlk.services.DataException("Server error", Error(data.message));
                    return Serializer.deserialize(data.data, ria.__API.ArrayOf(clazz));
                });
            }, [ [ String, Object, Object ] ], ria.async.Future, function getPaginatedList(uri, clazz, gParams) {
                return new ria.ajax.JsonGetTask(this.resolveUri(uri)).params(gParams).run().then(function(data) {
                    var model = new chlk.models.common.PaginatedList(clazz);
                    model.setItems(Serializer.deserialize(data.data, ria.__API.ArrayOf(clazz)));
                    model.setPageIndex(Number(data.pageindex));
                    model.setPageSize(Number(data.pagesize));
                    model.setTotalCount(Number(data.totalcount));
                    model.setTotalPages(Number(data.totalpages));
                    model.setHasNextPage(Boolean(data.hasnextpage));
                    model.setHasPreviousPage(Boolean(data.haspreviouspage));
                    return model;
                });
            } ]);
        })();
    })();
    (function() {
        (ria = ria || {}).templates = ria.templates || {};
        (function() {
            ria.templates.Exception = function ExceptionCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateException(def);
                var name = "ria.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Exception", [ function $(msg, inner_) {
                BASE(msg, inner_);
            } ]);
        })();
    })();
    (function() {
        (ria = ria || {}).templates = ria.templates || {};
        (function() {
            "use strict";
            ria.templates.IConverter = ria.__API.ifc(function() {}, "ria.templates.IConverter", [ [ "convert", Object, [], [ "source" ] ] ]);
        })();
    })();
    "ria.templates.IConverter";
    (function() {
        (ria = ria || {}).templates = ria.templates || {};
        (function() {
            "use strict";
            ria.templates.IConverterFactory = ria.__API.ifc(function() {}, "ria.templates.IConverterFactory", [ [ "canCreate", Boolean, [ ria.__API.ImplementerOf(ria.templates.IConverter) ], [ "converterClass" ] ], [ "create", ria.templates.IConverter, [ ria.__API.ImplementerOf(ria.templates.IConverter) ], [ "converterClass" ] ] ]);
        })();
    })();
    "ria.templates.Exception";
    "ria.templates.IConverterFactory";
    "ria.reflection.ReflectionClass";
    (function() {
        (ria = ria || {}).templates = ria.templates || {};
        (function() {
            ria.templates.ConverterFactoriesManager = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ConverterFactoriesManager", [ function $() {
                this._map = {};
                this._cache = {};
            }, [ [ ria.templates.IConverterFactory ] ], ria.__SYNTAX.Modifiers.VOID, function register(factory) {
                var hashCode = factory.getHashCode();
                if (this._map.hasOwnProperty(hashCode)) throw new ria.templates.Exception("Factory " + ria.__API.getIdentifierOfValue(factory) + " already registered");
                this._map[hashCode] = factory;
            }, [ [ ria.templates.IConverterFactory ] ], ria.__SYNTAX.Modifiers.VOID, function unregister(factory) {
                var hashCode = factory.getHashCode();
                if (!this._map.hasOwnProperty(hashCode)) throw new ria.templates.Exception("Factory " + ria.__API.getIdentifierOfValue(factory) + " not registered");
                delete this._map[factory.getHashCode()];
            }, [ [ ria.__API.ImplementerOf(ria.templates.IConverter) ] ], ria.templates.IConverter, function create(converterClass) {
                var name = ria.__API.getIdentifierOfType(converterClass);
                if (this._cache.hasOwnProperty(name)) return this._cache[name];
                for (var key in this._map) if (this._map.hasOwnProperty(key)) {
                    var factory = this._map[key];
                    if (factory.canCreate(converterClass)) return this._cache[name] = factory.create(converterClass);
                }
                throw new ria.__API.Exception("No factory agreed to create convertor " + name);
            } ]);
            ria.templates.ConverterFactories = new ria.templates.ConverterFactoriesManager();
        })();
    })();
    "ria.templates.Exception";
    "ria.templates.ConverterFactories";
    "ria.dom.Dom";
    "ria.reflection.ReflectionClass";
    (function() {
        (ria = ria || {}).templates = ria.templates || {};
        (function() {
            "use strict";
            function appendTo(content, to) {
                var dom = new ria.dom.Dom();
                dom.fromHTML(content).appendTo(to);
            }
            ria.templates.ModelBind = ria.__API.annotation("ria.templates.ModelBind", [], [ "model" ]);
            ria.templates.ModelPropertyBind = ria.__API.annotation("ria.templates.ModelPropertyBind", [ String, ria.__API.ImplementerOf(ria.templates.IConverter) ], [ "name_", "converter_" ]);
            ria.templates.TemplateBind = ria.__API.annotation("ria.templates.TemplateBind", [ String ], [ "tpl" ]);
            ria.templates.Template = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.ABSTRACT, "Template", [ Number, "collectionIndex", Array, "collection", function $() {
                this._modelClass = null;
                this._bindings = [];
                this._bundle = "";
                this._tpl = null;
                this._model = null;
                this.bind_();
            }, ria.__SYNTAX.Modifiers.VOID, function bind_() {
                var self = ria.reflection.ReflectionClass(this.getClass());
                if (!self.isAnnotatedWith(ria.templates.TemplateBind)) throw new ria.templates.Exception("Template class is not bound to template. Please use " + ria.__API.getIdentifierOfType(ria.templates.TemplateBind));
                this._tpl = self.findAnnotation(ria.templates.TemplateBind).pop().tpl;
                if (!self.isAnnotatedWith(ria.templates.ModelBind)) throw new ria.templates.Exception("Template class is not bound to model. Please use " + ria.__API.getIdentifierOfType(ria.templates.ModelBind));
                this._modelClass = self.findAnnotation(ria.templates.ModelBind).pop().model;
                if (this._modelClass === undefined) throw new ria.templates.Exception("Template class is bound to model. But model not loaded");
                if (!ria.__API.isClassConstructor(this._modelClass)) return;
                var model = ria.reflection.ReflectionClass(this._modelClass);
                var selfProperties = self.getPropertiesReflector(), bindings = this._bindings;
                selfProperties.filter(function(_) {
                    return _.isAnnotatedWith(ria.templates.ModelPropertyBind);
                }).forEach(function(property) {
                    var modelBind = property.findAnnotation(ria.templates.ModelPropertyBind).pop();
                    var modelPropertyName = modelBind.name_ || property.getShortName();
                    var modelProperty = model.getPropertyReflector(modelPropertyName);
                    if (modelProperty == null) throw ria.templates.Exception('Property "' + modelPropertyName + '" not found in model ' + model.getName());
                    var converter = modelBind.converter_;
                    if (converter !== undefined) {
                        var ref = ria.reflection.ReflectionClass(converter.converter);
                        if (!ref.implementsIfc(ria.templates.IConverter)) throw new ria.templates.Exception("Converter class " + ref.getName() + " expected to implement " + ria.__API.getIdentifierOfType(ria.templates.IConverter));
                        converter = bind.converter;
                    }
                    bindings.push({
                        sourceProp: modelProperty,
                        destProp: property,
                        converter: converter
                    });
                });
            }, ria.__API.ClassOf(ria.__API.Class), function getModelClass() {
                return this._modelClass;
            }, ria.__SYNTAX.Modifiers.VOID, function assign(model) {
                if (ria.__API.isArrayOfDescriptor(this._modelClass) && !Array.isArray(model)) {
                    throw ria.templates.Exception("Expected instance of " + ria.__API.getIdentifierOfType(this._modelClass) + " but got " + ria.__API.getIdentifierOfValue(model));
                } else if (ria.__API.isClassConstructor(this._modelClass) && !(model instanceof this._modelClass)) {
                    throw ria.templates.Exception("Expected instance of " + ria.__API.getIdentifierOfType(this._modelClass) + " but got " + ria.__API.getIdentifierOfValue(model));
                }
                this._model = model;
                var convertWith = this.convertWith, scope = this;
                this._bindings.forEach(function(_) {
                    var value = _.sourceProp.invokeGetterOn(model);
                    if (_.converter) {
                        value = convertWith(value, _.converter);
                    }
                    try {
                        _.destProp.invokeSetterOn(scope, value);
                    } catch (e) {
                        throw new ria.templates.Exception("Error assigning property " + _destProp.getName(), e);
                    }
                });
            }, ria.__SYNTAX.Modifiers.VOID, function options(options) {
                if ("function" == typeof options.block) {
                    this.setBlock(options.block);
                }
                delete options.block;
                if ("undefined" !== typeof options.collection) {
                    this.setCollection(options.collection);
                }
                delete options.collection;
                if ("number" === typeof options.collectionIndex) {
                    this.setCollectionIndex(options.collectionIndex);
                }
                delete options.collectionIndex;
                var ref = ria.reflection.ReflectionClass(this.getClass()), scope = this;
                var handled = {};
                options = ria.__API.clone(options);
                ref.getPropertiesReflector().filter(function(_) {
                    return !_.isAnnotatedWith(ria.templates.ModelBind);
                }).forEach(function(property) {
                    var key = property.getShortName();
                    if (options.hasOwnProperty(key)) {
                        property.invokeSetterOn(scope, options[key]);
                        delete options[key];
                    }
                });
                Object.getOwnPropertyNames(options).forEach(function(k) {
                    throw new ria.templates.Exception("Unknown property " + k + " in template " + ref.getName());
                });
            }, ria.__SYNTAX.Modifiers.ABSTRACT, String, function render() {}, ria.__SYNTAX.Modifiers.VOID, function renderTo(to) {
                appendTo(this.render(), to);
            }, ria.__SYNTAX.Modifiers.VOID, function renderBuffer() {
                this._bundle += this.render();
            }, String, function flushBuffer() {
                var buffer = this._bundle;
                this._bundle = "";
                return buffer;
            }, ria.__SYNTAX.Modifiers.VOID, function flushBufferTo(to) {
                appendTo(this.flushBuffer(), to);
            }, ria.__SYNTAX.Modifiers.SELF, function getInstanceOfTemplate_(tplClass, options_) {
                var tpl = new tplClass();
                if (!(tpl instanceof ria.__SYNTAX.Modifiers.SELF)) throw new ria.__API.Exception("Can render model only with " + hwax.templates.Template.__IDENTIFIER__);
                options_ && tpl.options(options_);
                return tpl;
            }, [ [ Object, ria.__API.ClassOf(ria.__SYNTAX.Modifiers.SELF), Object ] ], String, function renderWith(data, tplClass, options_) {
                var tpl = this.getInstanceOfTemplate_(tplClass, options_ || {});
                if (!Array.isArray(data)) {
                    data = [ data ];
                } else {
                    tpl.setCollection(data);
                }
                data.forEach(function(_, i) {
                    tpl.assign(_);
                    tpl.setCollectionIndex(i);
                    tpl.renderBuffer();
                });
                return tpl.flushBuffer();
            }, [ [ Object, ria.__API.ImplementerOf(ria.templates.IConverter) ] ], Object, function convertWith(value, clazz) {
                return ria.templates.ConverterFactories.create(clazz).convert(clazz);
            }, Object, function getContext_() {
                return this;
            } ]);
        })();
    })();
    "chlk.controls.Base";
    "chlk.services.BaseService";
    "ria.templates.Template";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.SearchBoxControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SearchBoxControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_41crbiipexy74x6r")(this);
            }, [ [ Object, ria.__API.ClassOf(chlk.services.BaseService), String, ria.__API.ClassOf(ria.templates.Template) ] ], ria.__SYNTAX.Modifiers.VOID, function initialize(attrs, service, method, tpl) {
                attrs.id = attrs.id || ria.dom.NewGID();
                var serviceIns = this.getContext().getService(service);
                var ref = ria.reflection.ReflectionClass(service);
                var methodRef = ref.getMethodReflector(method);
                var stub = function() {
                    return methodRef.invokeOn(serviceIns, ria.__API.clone(arguments));
                };
                this.queueReanimation_(attrs.id, attrs["default-value"] ? attrs["default-value"].valueOf() : null, stub, tpl);
            }, [ [ String, String, Function, ria.__API.ClassOf(ria.templates.Template) ] ], ria.__SYNTAX.Modifiers.VOID, function queueReanimation_(id, defaultValue_, serviceF, tpl) {
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    if (defaultValue_) ria.dom.Dom("#" + id + "-hidden").setValue(defaultValue_);
                    this.reanimate_(ria.dom.Dom("#" + id), serviceF, tpl, activity, model);
                }.bind(this));
            }, [ [ ria.dom.Dom, Function, ria.__API.ClassOf(ria.templates.Template), ria.mvc.IActivity, Object ] ], ria.__SYNTAX.Modifiers.VOID, function reanimate_(node, serviceF, tplClass, activity, model) {
                var id = node.getAttr("id");
                var tpl = new tplClass();
                jQuery(node.valueOf()).autocomplete({
                    source: function(request, response) {
                        serviceF(request.term).then(response);
                    },
                    focus: function() {
                        return false;
                    },
                    select: function(event, ui) {
                        var li = jQuery(event.toElement).closest("li");
                        ria.dom.Dom("#" + id + "-hidden").setValue(li.data("value"));
                        node.setValue(li.data("title"));
                        return false;
                    }
                }).data("ui-autocomplete")._renderItem = function(ul, item) {
                    var fixedInstance = ria.__API.inheritFrom(item.constructor);
                    for (var k in item) if (item.hasOwnProperty(k)) fixedInstance[k] = item[k];
                    tpl.assign(fixedInstance);
                    return jQuery(jQuery.parseHTML(tpl.render())).appendTo(ul);
                };
            } ]);
        })();
    })();
    __ASSETS._smj47o6q7kg1fw29 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Image_mixin = function() {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            attributes = self.processAttrs(attributes);
            buf.push("<img" + jade.attrs(jade.merge({}, attributes), jade.merge({}, escaped, true)) + "/>");
        };
        jade.globals.PersonImg_mixin = function(person, size) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            if (person.getGender()) {
                var src = chlk.templates.JadeTemplate.prototype.getPictureURL(person.getId, size, size);
                var isWoman = person.getGender().toLowerCase() == "f";
                var alternativeHref = window.WEB_SITE_ROOT + "Content/avatars/" + size + "/.no-avatar";
                if (isWoman) {
                    var alternativeHref = alternativeHref + "-female.png";
                } else {
                    var alternativeHref = alternativeHref + ".png";
                }
                attributes.src = src;
                attributes.alternativeSrc = alternativeHref;
                attributes.class = attributes.class || [];
                attributes.class.push("avatar");
                attributes.class.push("avatar" + size);
                jade.globals.Image_mixin.call({
                    buf: buf,
                    attributes: jade.merge({}, attributes),
                    escaped: jade.merge({}, escaped, true)
                });
            }
        };
        jade.globals.ClassImg_mixin = function(course, width, height) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            var srcId = course.getDepartmentId();
            var alternativeSrcId = course.getId();
            if (!srcId) {
                srcId = alternativeSrcId;
                alternativeSrcId = null;
            }
            var src = chlk.templates.JadeTemplate.prototype.getPictureURL(srcId, height, width);
            var alternativeSrc = null;
            if (alternativeSrcId) {
                alternativeSrc = chlk.templates.JadeTemplate.prototype.getPictureURL(alternativeSrcId, height, width);
            }
            defaultSrc = window.WEB_SITE_ROOT + "Content/images/common/course-icons/all.png";
            attributes.src = src;
            attributes.alternativeSrc = alternativeSrc;
            attributes.defaultSrc = defaultSrc;
            jade.globals.Image_mixin.call({
                buf: buf,
                attributes: jade.merge({}, attributes),
                escaped: jade.merge({}, escaped, true)
            });
        };
        return buf.join("");
    };
    "chlk.controls.Base";
    (function() {
        (chlk = chlk || {}).controls = chlk.controls || {};
        (function() {
            chlk.controls.ImageControl = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controls." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ImageControl", ria.__SYNTAX.EXTENDS(chlk.controls.Base), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                ASSET("_smj47o6q7kg1fw29")(this);
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.VOID, function onImgError(event) {
                var node = jQuery(event.target);
                var alternativeSrc = node.attr("alternativeSrc");
                var defaultSrc = node.attr("defaultSrc");
                var needAlternative = node.attr("src") != alternativeSrc;
                var src = needAlternative ? alternativeSrc : defaultSrc;
                node.attr("src", src);
                if (!(needAlternative && defaultSrc)) node.off("error.load");
            }, String, "alternativeSrc", String, "defaultSrc", [ [ Object ] ], Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                if (!attributes.alternativeSrc && attributes.defaultSrc) {
                    attributes.alternativeSrc = attributes.defaultSrc;
                    delete attributes.defaultSrc;
                }
                if (attributes.alternativeSrc) this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    jQuery("#" + attributes.id).on("error.load", this.onImgError);
                }.bind(this));
                return attributes;
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.RoleEnum =             function wrapper() {
                var values = {};
                function RoleEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(RoleEnum, "chlk.models.common.RoleEnum");
                function RoleEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.common.RoleEnum#" + value + "]";
                    };
                }
                ria.__API.extend(RoleEnumImpl, RoleEnum);
                values[1] = RoleEnum.SYSADMIN = new RoleEnumImpl(1);
                values[2] = RoleEnum.ADMINVIEW = new RoleEnumImpl(2);
                values[3] = RoleEnum.ADMINGRADE = new RoleEnumImpl(3);
                values[4] = RoleEnum.ADMINEDIT = new RoleEnumImpl(4);
                values[5] = RoleEnum.TEACHER = new RoleEnumImpl(5);
                values[6] = RoleEnum.STUDENT = new RoleEnumImpl(6);
                values[7] = RoleEnum.PARENT = new RoleEnumImpl(7);
                values[8] = RoleEnum.DEVELOPER = new RoleEnumImpl(8);
                return RoleEnum;
            }();
            chlk.models.common.RoleNamesEnum =             function wrapper() {
                var values = {};
                function RoleNamesEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(RoleNamesEnum, "chlk.models.common.RoleNamesEnum");
                function RoleNamesEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.common.RoleNamesEnum#" + value + "]";
                    };
                }
                ria.__API.extend(RoleNamesEnumImpl, RoleNamesEnum);
                values["sysadmin"] = RoleNamesEnum.SYSADMIN = new RoleNamesEnumImpl("sysadmin");
                values["adminview"] = RoleNamesEnum.ADMINVIEW = new RoleNamesEnumImpl("adminview");
                values["admingrade"] = RoleNamesEnum.ADMINGRADE = new RoleNamesEnumImpl("admingrade");
                values["adminedit"] = RoleNamesEnum.ADMINEDIT = new RoleNamesEnumImpl("adminedit");
                values["teacher"] = RoleNamesEnum.TEACHER = new RoleNamesEnumImpl("teacher");
                values["student"] = RoleNamesEnum.STUDENT = new RoleNamesEnumImpl("student");
                values["parent"] = RoleNamesEnum.PARENT = new RoleNamesEnumImpl("parent");
                values["developer"] = RoleNamesEnum.DEVELOPER = new RoleNamesEnumImpl("developer");
                return RoleNamesEnum;
            }();
            chlk.models.common.Role = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Role", [ chlk.models.common.RoleEnum, "roleId", String, "roleName", [ [ chlk.models.common.RoleEnum, String ] ], function $(roleId, roleName) {
                this.setRoleId(roleId);
                this.setRoleName(roleName);
            } ]);
        })();
    })();
    "ria.serialize.IDeserializable";
    function getDate(str, a, b) {
        if (typeof str == "string" && str.length == 10) {
            return new Date(str.replace(/-/g, "/"));
        } else {
            return str ? a !== undefined && b !== undefined ? new Date(str, a, b) : new Date(str) : new Date();
        }
    }
    function formatDate(date, format) {
        return $.datepicker.formatDate(format, date || getDate());
    }
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            var second = 1e3, minute = 60 * second, hour = 60 * minute, day = 24 * hour, week = 7 * day;
            chlk.models.common.MillisecondsEnum =             function wrapper() {
                var values = {};
                function MillisecondsEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(MillisecondsEnum, "chlk.models.common.MillisecondsEnum");
                function MillisecondsEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.common.MillisecondsEnum#" + value + "]";
                    };
                }
                ria.__API.extend(MillisecondsEnumImpl, MillisecondsEnum);
                values[1e3] = MillisecondsEnum.SECOND = new MillisecondsEnumImpl(1e3);
                values[6e4] = MillisecondsEnum.MINUTE = new MillisecondsEnumImpl(6e4);
                values[36e5] = MillisecondsEnum.HOUR = new MillisecondsEnumImpl(36e5);
                values[864e5] = MillisecondsEnum.DAY = new MillisecondsEnumImpl(864e5);
                values[6048e5] = MillisecondsEnum.WEEK = new MillisecondsEnumImpl(6048e5);
                return MillisecondsEnum;
            }();
            chlk.models.common.ChlkDateEnum =             function wrapper() {
                var values = {};
                function ChlkDateEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(ChlkDateEnum, "chlk.models.common.ChlkDateEnum");
                function ChlkDateEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.common.ChlkDateEnum#" + value + "]";
                    };
                }
                ria.__API.extend(ChlkDateEnumImpl, ChlkDateEnum);
                values["SECOND"] = ChlkDateEnum.SECOND = new ChlkDateEnumImpl("SECOND");
                values["MINUTE"] = ChlkDateEnum.MINUTE = new ChlkDateEnumImpl("MINUTE");
                values["HOUR"] = ChlkDateEnum.HOUR = new ChlkDateEnumImpl("HOUR");
                values["DAY"] = ChlkDateEnum.DAY = new ChlkDateEnumImpl("DAY");
                values["WEEK"] = ChlkDateEnum.WEEK = new ChlkDateEnumImpl("WEEK");
                values["MONTH"] = ChlkDateEnum.MONTH = new ChlkDateEnumImpl("MONTH");
                values["YEAR"] = ChlkDateEnum.YEAR = new ChlkDateEnumImpl("YEAR");
                return ChlkDateEnum;
            }();
            chlk.models.common.ChlkDate = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ChlkDate", ria.__SYNTAX.IMPLEMENTS(ria.serialize.IDeserializable), [ Date, "date", [ [ Date ] ], function $(date_) {
                date_ && this.setDate(date_);
            }, [ [ String ] ], String, function toString(format_) {
                return this.format(format_ || "m-dd-yy");
            }, [ [ chlk.models.common.ChlkDateEnum, Number ] ], ria.__SYNTAX.Modifiers.SELF, function add(type, count) {
                var dateEnum = chlk.models.common.ChlkDateEnum;
                var thisDate = this.getDate();
                var sec = thisDate.getSeconds();
                var min = thisDate.getMinutes();
                var h = thisDate.getHours();
                var day = thisDate.getDate();
                var mon = thisDate.getMinutes();
                var y = thisDate.getFullYear();
                var date = new chlk.models.common.ChlkDate(), res;
                switch (type) {
                  case dateEnum.YEAR:
                    res = new Date(y + count, mon, day, h, min, sec);
                    break;

                  case dateEnum.MONTH:
                    res = new Date(y, mon + count, day, h, min, sec);
                    break;

                  default:
                    res = thisDate.getTime() + count * chlk.models.common.MillisecondsEnum[type.valueOf()].valueOf();
                }
                date.setDate(new Date(res));
                return date;
            }, [ [ String ] ], String, function format(format) {
                format = format.replace(/min/g, this.timepartToStr(this.getDate().getMinutes()));
                var res = $.datepicker.formatDate(format, this.getDate() || getDate());
                res = res.replace(/hh/g, this.timepartToStr(this.getDate().getHours()));
                res = res.replace(/ss/g, this.timepartToStr(this.getDate().getSeconds()));
                return res;
            }, [ [ ria.__SYNTAX.Modifiers.SELF ] ], Boolean, function isSameDay(date) {
                return this.format("mm-dd-yy") == date.format("mm-dd-yy");
            }, ria.__SYNTAX.Modifiers.VOID, function deserialize(raw) {
                var date = raw ? getDate(raw) : getDate();
                this.setDate(date);
            }, [ [ Number ] ], String, function timepartToStr(t) {
                return "" + (t - t % 10) / 10 + t % 10;
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.MarkingPeriodId =             function wrapper() {
                var values = {};
                function MarkingPeriodId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new MarkingPeriodIdImpl(value);
                }
                ria.__API.identifier(MarkingPeriodId, "chlk.models.id.MarkingPeriodId");
                function MarkingPeriodIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.MarkingPeriodId#" + value + "]";
                    };
                }
                ria.__API.extend(MarkingPeriodIdImpl, MarkingPeriodId);
                return MarkingPeriodId;
            }();
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.id.MarkingPeriodId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).schoolYear = chlk.models.schoolYear || {};
        (function() {
            "use strict";
            chlk.models.schoolYear.MarkingPeriod = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.schoolYear." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("MarkingPeriod", [ chlk.models.id.MarkingPeriodId, "id", String, "description", [ ria.serialize.SerializeProperty("startdate") ], chlk.models.common.ChlkDate, "startDate", [ ria.serialize.SerializeProperty("enddate") ], chlk.models.common.ChlkDate, "endDate", Number, "weekdays", String, "name" ]);
        })();
    })();
    "ria.mvc.Application";
    "ria.dom.jQueryDom";
    "ria.dom.ready";
    "chlk.controls.ActionFormControl";
    "chlk.controls.LabeledCheckboxControl";
    "chlk.controls.SlideCheckboxControl";
    "chlk.controls.ActionLinkControl";
    "chlk.controls.AvatarControl";
    "chlk.controls.ButtonControl";
    "chlk.controls.CheckboxControl";
    "chlk.controls.CheckboxListControl";
    "chlk.controls.DatePickerControl";
    "chlk.controls.SelectControl";
    "chlk.controls.DateSelectControl";
    "chlk.controls.FileUploadControl";
    "chlk.controls.GlanceBoxControl";
    "chlk.controls.GridControl";
    "chlk.controls.LeftRightToolbarControl";
    "chlk.controls.ListViewControl";
    "chlk.controls.LoadingImgControl";
    "chlk.controls.PaginatorControl";
    "chlk.controls.PhotoContainerControl";
    "chlk.controls.VideoControl";
    "chlk.controls.LogoutControl";
    "chlk.controls.TextAreaControl";
    "chlk.controls.SearchBoxControl";
    "chlk.controls.ImageControl";
    "chlk.models.common.Role";
    "chlk.models.schoolYear.MarkingPeriod";
    (function() {
        chlk = chlk || {};
        (function() {
            chlk.BaseApp = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BaseApp", ria.__SYNTAX.EXTENDS(ria.mvc.Application), [ function $() {
                BASE();
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                this.saveInSession(session, "markingPeriod", chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, "nextMarkingPeriod", chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, "finalizedClassesIds");
                this.saveInSession(session, "currentChlkPerson", chlk.models.people.User, "currentPerson");
                this.saveInSession(session, "WEB_SITE_ROOT", null, "webSiteRoot");
                this.saveInSession(session, "azurePictureUrl");
                var siteRoot = window.location.toString().split(window.location.pathname).shift();
                var serviceRoot = "/";
                session.set("siteRoot", siteRoot + serviceRoot);
                return session;
            }, [ [ ria.mvc.ISession, String, Object, String ] ], function saveInSession(session, key, cls_, destKey_) {
                var serializer = new ria.serialize.JsonSerializer();
                var value = window[key] || {};
                var destK = destKey_ ? destKey_ : key;
                if (value) {
                    cls_ ? session.set(destK, serializer.deserialize(value, cls_)) : session.set(destK, value);
                }
            }, function getCurrentPerson() {
                return this.getContext().getSession().get("currentPerson", null);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.async.Future, function onStart_() {
                jQuery(document).on("mouseover", "[data-tooltip]", function() {
                    var node = jQuery(this), tooltip = jQuery("#chlk-tooltip-item"), offset = node.offset();
                    var value = node.data("tooltip");
                    if (value) {
                        tooltip.show();
                        tooltip.find(".tooltip-content").html(node.data("tooltip"));
                        tooltip.css("left", offset.left + (node.width() - tooltip.width()) / 2).css("top", offset.top - tooltip.height());
                    }
                });
                jQuery(document).on("mouseleave", "[data-tooltip]", function() {
                    var tooltip = jQuery("#chlk-tooltip-item");
                    tooltip.hide();
                    tooltip.find(".tooltip-content").html("");
                });
                return BASE().then(function(data) {
                    if (this.getCurrentPerson()) new ria.dom.Dom().fromHTML(ASSET("_tsj822rosin0o1or")(this)).appendTo("#logout-block");
                    return data;
                }, this);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.SchoolPersonId =             function wrapper() {
                var values = {};
                function SchoolPersonId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new SchoolPersonIdImpl(value);
                }
                ria.__API.identifier(SchoolPersonId, "chlk.models.id.SchoolPersonId");
                function SchoolPersonIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.SchoolPersonId#" + value + "]";
                    };
                }
                ria.__API.extend(SchoolPersonIdImpl, SchoolPersonId);
                return SchoolPersonId;
            }();
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AddressId =             function wrapper() {
                var values = {};
                function AddressId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AddressIdImpl(value);
                }
                ria.__API.identifier(AddressId, "chlk.models.id.AddressId");
                function AddressIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AddressId#" + value + "]";
                    };
                }
                ria.__API.extend(AddressIdImpl, AddressId);
                return AddressId;
            }();
        })();
    })();
    "chlk.models.id.AddressId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.Address = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Address", [ chlk.models.id.AddressId, "id", Number, "type", String, "value" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.PhoneId =             function wrapper() {
                var values = {};
                function PhoneId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new PhoneIdImpl(value);
                }
                ria.__API.identifier(PhoneId, "chlk.models.id.PhoneId");
                function PhoneIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.PhoneId#" + value + "]";
                    };
                }
                ria.__API.extend(PhoneIdImpl, PhoneId);
                return PhoneId;
            }();
        })();
    })();
    "chlk.models.id.PhoneId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.Phone = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Phone", [ chlk.models.id.PhoneId, "id", [ ria.serialize.SerializeProperty("isprimary") ], Boolean, "isPrimary", Number, "type", String, "value" ]);
        })();
    })();
    "chlk.models.id.SchoolPersonId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.Role = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Role", [ String, "description", Number, "id", String, "name", [ ria.serialize.SerializeProperty("namelowered") ], String, "nameLowered" ]);
        })();
    })();
    "chlk.models.id.SchoolPersonId";
    "chlk.models.common.ChlkDate";
    "chlk.models.people.Address";
    "chlk.models.people.Phone";
    "chlk.models.people.Role";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.User = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("User", [ Boolean, "active", ria.__API.ArrayOf(chlk.models.people.Address), "addresses", [ ria.serialize.SerializeProperty("displayname") ], String, "displayName", String, "email", [ ria.serialize.SerializeProperty("firstname") ], String, "firstName", [ ria.serialize.SerializeProperty("fullname") ], String, "fullName", String, "gender", String, "grade", chlk.models.id.SchoolPersonId, "id", [ ria.serialize.SerializeProperty("lastname") ], String, "lastName", [ ria.serialize.SerializeProperty("localid") ], String, "localId", String, "password", String, "pictureUrl", [ ria.serialize.SerializeProperty("role") ], chlk.models.people.Role, "role", [ ria.serialize.SerializeProperty("schoolid") ], Number, "schoolId", [ ria.serialize.SerializeProperty("birthdate") ], chlk.models.common.ChlkDate, "birthDate", String, "birthDateText", String, "genderFullText", ria.__API.ArrayOf(chlk.models.people.Phone), "phones", String, "salutation", Boolean, "ableEdit", chlk.models.people.Phone, "primaryPhone", chlk.models.people.Phone, "homePhone", String, "addressesValue", String, "phonesValue", Number, "index", Boolean, "selected" ]);
        })();
    })();
    "ria.mvc.IActivity";
    "ria.async.Observable";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            "use strict";
            ria.mvc.Activity = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }(ria.__SYNTAX.Modifiers.ABSTRACT, "Activity", ria.__SYNTAX.IMPLEMENTS(ria.mvc.IActivity), [ function $() {
                BASE();
                this._inited = false;
                this._started = false;
                this._paused = false;
                this._stopped = false;
                this._onClose = new ria.async.Observable(ria.mvc.ActivityClosedEvent);
                this._onRefresh = new ria.async.Observable(ria.mvc.ActivityRefreshedEvent);
            }, ria.__SYNTAX.Modifiers.VOID, function show() {
                if (!this._inited) {
                    this.onCreate_();
                    this._inited = true;
                }
                if (!this._paused) {
                    this._stopped && this.onRestart_();
                    this.onStart_();
                }
                this.onResume_();
                this._stopped = false;
                this._paused = false;
            }, ria.__SYNTAX.Modifiers.VOID, function pause() {
                if (!this._paused) {
                    this.onPause_();
                    this._paused = true;
                }
            }, ria.__SYNTAX.Modifiers.VOID, function stop() {
                if (!this._stopped) {
                    !this._paused && this.onPause_();
                    this.onStop_();
                }
                this._stopped = true;
                this._paused = false;
            }, Boolean, function isForeground() {
                return this._inited && !this._paused && !this._stopped;
            }, Boolean, function isStarted() {
                return this._inited && !this._stopped;
            }, ria.async.Future, function getModelEvents_(msg_) {
                var me = this;
                var head = new ria.async.Future();
                me.onModelWait_(msg_);
                head.handleProgress(function(progress) {
                    me.onModelProgress_(progress, msg_);
                }).complete(function() {
                    me.onModelComplete_(msg_);
                }).catchError(function(error) {
                    me.onModelError_(error, msg_);
                    throw error;
                }).then(function(model) {
                    me.onModelReady_(model, msg_);
                    return model;
                });
                return head;
            }, [ [ ria.async.Future ] ], ria.async.Future, function refreshD(future) {
                this.startFullLoading();
                var me = this;
                return future.attach(this.getModelEvents_()).then(function(model) {
                    me.onRender_(model);
                    return model;
                }).then(function(model) {
                    me.onRefresh_(model);
                    return model;
                });
            }, [ [ ria.async.Future, String ] ], ria.async.Future, function partialRefreshD(future, msg_) {
                var msg = msg_ || "";
                var me = this;
                return future.attach(this.getModelEvents_(msg)).then(function(model) {
                    me.onPartialRender_(model, msg);
                    return model;
                }).then(function(model) {
                    me.onPartialRefresh_(model, msg);
                    return model;
                });
            }, ria.__SYNTAX.Modifiers.VOID, function startLoading() {}, ria.__SYNTAX.Modifiers.VOID, function startFullLoading() {}, ria.__SYNTAX.Modifiers.VOID, function stopLoading() {}, [ [ Object ] ], ria.__SYNTAX.Modifiers.VOID, function refresh(model) {
                this.onModelReady_(model);
                this.onRender_(model);
                ria.async.DeferredData(model).next(this.onRefresh_);
            }, ria.__SYNTAX.Modifiers.ABSTRACT, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {}, ria.__SYNTAX.Modifiers.VOID, function onStart_() {}, ria.__SYNTAX.Modifiers.VOID, function onRestart_() {}, ria.__SYNTAX.Modifiers.VOID, function onResume_() {}, ria.__SYNTAX.Modifiers.VOID, function onPause_() {}, ria.__SYNTAX.Modifiers.VOID, function onStop_() {}, [ [ String ] ], ria.__SYNTAX.Modifiers.VOID, function onModelWait_(msg_) {}, [ [ Object, String ] ], ria.__SYNTAX.Modifiers.VOID, function onModelProgress_(data, msg_) {}, [ [ Object, String ] ], ria.__SYNTAX.Modifiers.VOID, function onModelError_(data, msg_) {}, [ [ Object, String ] ], ria.__SYNTAX.Modifiers.VOID, function onModelReady_(data, msg_) {}, [ [ String ] ], ria.__SYNTAX.Modifiers.VOID, function onModelComplete_(msg_) {
                this.stopLoading();
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.VOID, function onRender_(data) {}, [ [ Object, String ] ], ria.__SYNTAX.Modifiers.VOID, function onPartialRender_(data, msg_) {
                this.dom.removeClass("loading");
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.VOID, function onRefresh_(data) {
                this._onRefresh.notify([ this, data ]);
            }, [ [ Object, String ] ], ria.__SYNTAX.Modifiers.VOID, function onPartialRefresh_(data, msg_) {
                this._onRefresh.notify([ this, data, msg_ ]);
            }, ria.__SYNTAX.Modifiers.VOID, function onDispose_() {
                this.stop();
            }, ria.__SYNTAX.Modifiers.VOID, function close() {
                this._onClose.notifyAndClear([ this ]);
            }, [ [ ria.mvc.ActivityClosedEvent ] ], ria.__SYNTAX.Modifiers.VOID, function addCloseCallback(callback) {
                this._onClose.on(callback);
            }, [ [ ria.mvc.ActivityRefreshedEvent ] ], ria.__SYNTAX.Modifiers.VOID, function addRefreshCallback(callback) {
                this._onRefresh.on(callback);
            } ]);
        })();
    })();
    "ria.mvc.MvcException";
    "ria.mvc.Activity";
    "ria.dom.Dom";
    "ria.mvc.DomEventBind";
    "ria.reflection.ReflectionClass";
    window.noLoadingMsg = "no-loading";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            var MODEL_WAIT_CLASS = "activity-model-wait";
            function camel2dashed(_) {
                return _.replace(/[a-z][A-Z]/g, function(str, offset) {
                    return str[0] + "-" + str[1].toLowerCase();
                });
            }
            var MODEL_WAIT_CLASS = "activity-model-wait";
            function camel2dashed(_) {
                return _.replace(/[a-z][A-Z]/g, function(str, offset) {
                    return str[0] + "-" + str[1].toLowerCase();
                });
            }
            ria.mvc.DomAppendTo = ria.__API.annotation("ria.mvc.DomAppendTo", [ String ], [ "node" ]);
            ria.mvc.DomActivity = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DomActivity", ria.__SYNTAX.EXTENDS(ria.mvc.Activity), [ ria.dom.Dom, "dom", function $() {
                BASE();
                this._actitivyClass = null;
                this._domAppendTo = null;
                this._domEvents = [];
                this.processAnnotations_(new ria.reflection.ReflectionClass(this.getClass()));
            }, [ [ ria.reflection.ReflectionClass ] ], ria.__SYNTAX.Modifiers.VOID, function processAnnotations_(ref) {
                this._activityClass = camel2dashed(ref.getShortName());
                if (!ref.isAnnotatedWith(ria.mvc.DomAppendTo)) throw new ria.mvc.MvcException("ria.mvc.DomActivity expects annotation ria.mvc.DomAppendTo");
                this._domAppendTo = new ria.dom.Dom(ref.findAnnotation(ria.mvc.DomAppendTo).pop().node);
                this._domEvents = ref.getMethodsReflector().filter(function(_) {
                    return _.isAnnotatedWith(ria.mvc.DomEventBind);
                }).map(function(_) {
                    if (_.getArguments().length != 2) throw new ria.mvc.MvcException("Methods, annotated with ria.mvc.DomBindEvent, are expected to accept two arguments (node, event)");
                    var annotation = _.findAnnotation(ria.mvc.DomEventBind).pop();
                    return {
                        event: annotation.event,
                        selector: annotation.selector_,
                        methodRef: _
                    };
                });
            }, ria.__SYNTAX.Modifiers.ABSTRACT, ria.dom.Dom, function onDomCreate_() {}, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onCreate_() {
                BASE();
                var dom = this.dom = this.onDomCreate_().addClass(this._actitivyClass);
                var instance = this;
                this._domEvents.forEach(function(_) {
                    var handler = function(node, event) {
                        return _.methodRef.invokeOn(instance, ria.__API.clone(arguments));
                    };
                    var selector = _.selector;
                    if (!selector) {
                        selector = handler;
                        handler = undefined;
                    }
                    dom.on(_.event, selector, handler);
                });
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onStart_() {
                BASE();
                this.dom.appendTo(this._domAppendTo);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onStop_() {
                BASE();
                this._domAppendTo.remove(this.dom.empty());
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function startLoading() {
                this.dom.addClass(MODEL_WAIT_CLASS);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function stopLoading() {
                this.dom.removeClass(MODEL_WAIT_CLASS);
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onModelWait_(msg_) {
                BASE(msg_);
                msg_ != window.noLoadingMsg && this.startLoading();
            }, [ [ String ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onModelComplete_(msg_) {
                BASE(msg_);
                this.stopLoading();
            } ]);
        })();
    })();
    "ria.mvc.DomActivity";
    "ria.templates.Template";
    (function() {
        (ria = ria || {}).mvc = ria.mvc || {};
        (function() {
            ria.mvc.TemplateBind = ria.__API.annotation("ria.mvc.TemplateBind", [ ria.__API.ClassOf(ria.templates.Template) ], [ "tpl" ]);
            ria.mvc.PartialUpdateRuleActions =             function wrapper() {
                var values = {};
                function PartialUpdateRuleActions(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(PartialUpdateRuleActions, "ria.mvc.PartialUpdateRuleActions");
                function PartialUpdateRuleActionsImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[ria.mvc.PartialUpdateRuleActions#" + value + "]";
                    };
                }
                ria.__API.extend(PartialUpdateRuleActionsImpl, PartialUpdateRuleActions);
                values["prepend"] = PartialUpdateRuleActions.Prepend = new PartialUpdateRuleActionsImpl("prepend");
                values["replace"] = PartialUpdateRuleActions.Replace = new PartialUpdateRuleActionsImpl("replace");
                values["append"] = PartialUpdateRuleActions.Append = new PartialUpdateRuleActionsImpl("append");
                return PartialUpdateRuleActions;
            }();
            ria.mvc.PartialUpdateRule = ria.__API.annotation("ria.mvc.PartialUpdateRule", [ ria.__API.ClassOf(ria.templates.Template), String, String, ria.mvc.PartialUpdateRuleActions ], [ "tpl", "msg_", "selector_", "action_" ]);
            ria.mvc.TemplateActivity = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.mvc." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TemplateActivity", ria.__SYNTAX.EXTENDS(ria.mvc.DomActivity), [ function $() {
                BASE();
            }, [ [ ria.reflection.ReflectionClass ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function processAnnotations_(ref) {
                BASE(ref);
                if (!ref.isAnnotatedWith(ria.mvc.TemplateBind)) throw new ria.mvc.MvcException("ria.mvc.TemplateActivity expects annotation ria.mvc.TemplateBind");
                var tpls = ref.findAnnotation(ria.mvc.TemplateBind).pop().tpl;
                if (!Array.isArray(tpls)) tpls = [ tpls ];
                if (tpls.some(function(_) {
                    return _ === undefined;
                })) throw new ria.mvc.MvcException(ref.getName() + " is annotated with ria.mvc.TemplateBind" + ", but some templates classes appears to be not loaded: [" + tpls.map(function(_) {
                    return ria.__API.getIdentifierOfType(_);
                }) + "]");
                this._templateClasses = tpls.map(function(tpl) {
                    var tplRef = new ria.reflection.ReflectionClass(tpl);
                    if (!tplRef.extendsClass(ria.templates.Template)) throw new ria.mvc.MvcException(ref.getName() + " is annotated with ria.mvc.TemplateBind" + ", but templates " + tplRef.getName() + " is not descedant of ria.templates.Template");
                    return tplRef.instantiate();
                });
                var partialUpdateWithMethods = ref.getMethodsReflector().filter(function(_) {
                    return _.isAnnotatedWith(ria.mvc.PartialUpdateRule);
                }).map(function(_) {
                    var annotation = _.findAnnotation(ria.mvc.PartialUpdateRule).pop();
                    var tplRef = new ria.reflection.ReflectionClass(annotation.tpl);
                    return {
                        tpl: tplRef.instantiate(),
                        msg: _.msg_ || null,
                        methodRef: _
                    };
                });
                this._partialUpdateRules = ref.findAnnotation(ria.mvc.PartialUpdateRule).map(function(_) {
                    if (_.tpl === undefined) throw new ria.mvc.MvcException(ref.getName() + " is annotated with ria.mvc.PartialUpdateRule" + ", but some templates classes appears to be not loaded.");
                    var tplRef = new ria.reflection.ReflectionClass(_.tpl);
                    if (!tplRef.extendsClass(ria.templates.Template)) throw new ria.mvc.MvcException(ref.getName() + " is annotated with ria.mvc.PartialUpdateRule" + ", but templates " + tplRef.getName() + " is not descedant of ria.templates.Template");
                    return {
                        tpl: tplRef.instantiate(),
                        msg: _.msg_ || null,
                        selector: _.selector_ || null,
                        action: _.action_ || ria.mvc.PartialUpdateRuleActions.Replace
                    };
                }).concat(partialUpdateWithMethods);
                if (this._partialUpdateRules.length < 1 && this._templateClasses.length == 1) {
                    this._partialUpdateRules.push({
                        tpl: this._templateClasses[0],
                        msg: null,
                        selector: null,
                        action: ria.mvc.PartialUpdateRuleActions.Replace
                    });
                }
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.dom.Dom, function onDomCreate_() {
                return new ria.dom.Dom().fromHTML("<div></div>");
            }, ria.templates.Template, function doFindTemplateForModel_(model) {
                var matches = this._templateClasses.filter(function(_) {
                    return model instanceof _.getModelClass();
                });
                if (matches.length == 0) throw new ria.mvc.MvcException("Found no template that can render " + ria.__API.getIdentifierOfValue(model));
                if (matches.length > 1) throw new ria.mvc.MvcException("Found multiple templates that can render " + ria.__API.getIdentifierOfValue(model) + ", matches " + matches.map(function(_) {
                    return ria.__API.getIdentifierOfValue(_);
                }));
                return matches.pop();
            }, [ [ ria.templates.Template, Object, String ] ], ria.__SYNTAX.Modifiers.VOID, function onPrepareTemplate_(tpl, model, msg_) {}, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onRender_(model) {
                BASE(model);
                var tpl = this.doFindTemplateForModel_(model);
                this.onPrepareTemplate_(tpl, model);
                tpl.assign(model);
                tpl.renderTo(this.dom.empty());
            }, Object, function doFindTemplateForPartialModel_(model, msg) {
                var matches = this._partialUpdateRules.filter(function(_) {
                    return (_.msg === null || _.msg == msg) && model instanceof _.tpl.getModelClass();
                });
                if (matches.length == 0) throw new ria.mvc.MvcException("Found no template that can render " + ria.__API.getIdentifierOfValue(model) + " with message " + msg);
                if (matches.length > 1) throw new ria.mvc.MvcException("Found multiple templates that can render " + ria.__API.getIdentifierOfValue(model) + " with message " + msg + ", matches " + matches.map(function(_) {
                    return ria.__API.getIdentifierOfValue(_);
                }));
                return matches.pop();
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                var rule = this.doFindTemplateForPartialModel_(model, msg_ || "");
                var tpl = rule.tpl;
                this.onPrepareTemplate_(tpl, model, msg_);
                tpl.assign(model);
                if (rule.methodRef) {
                    rule.methodRef.invokeOn(this, [ tpl, model, msg_ ]);
                } else {
                    var dom = new ria.dom.Dom().fromHTML(tpl.render());
                    var target = this.dom;
                    if (rule.selector) target = target.find(rule.selector);
                    switch (rule.action) {
                      case ria.mvc.PartialUpdateRuleActions.Prepend:
                        dom.prependTo(target);
                        break;

                      case ria.mvc.PartialUpdateRuleActions.Append:
                        dom.appendTo(target);
                        break;

                      default:
                        dom.appendTo(target.empty());
                    }
                }
            } ]);
        })();
    })();
    "ria.mvc.TemplateActivity";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).lib = chlk.activities.lib || {};
        (function() {
            var UNDER_OVERLAY_CLASS = "under-overlay";
            var HIDDEN_CLASS = "x-hidden";
            chlk.activities.lib.TemplateDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.lib." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("body") ], "TemplateDialog", ria.__SYNTAX.EXTENDS(ria.mvc.TemplateActivity), [ function $() {
                BASE();
                this._overlay = new ria.dom.Dom("#chlk-overlay");
                this._dialogsHolder = new ria.dom.Dom("#chlk-dialogs");
                this._clickMeHandler = function() {
                    this.close();
                    return false;
                }.bind(this);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onResume_() {
                BASE();
                this._dialogsHolder.removeClass(HIDDEN_CLASS);
                this._overlay.removeClass(HIDDEN_CLASS).on("click", this._clickMeHandler);
                this.dom.removeClass(UNDER_OVERLAY_CLASS);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onPause_() {
                this.dom.addClass(UNDER_OVERLAY_CLASS);
                this._dialogsHolder.addClass(HIDDEN_CLASS);
                this._overlay.addClass(HIDDEN_CLASS).off("click", this._clickMeHandler);
                BASE();
            }, [ ria.mvc.DomEventBind("click", ".close") ], [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function onCloseBtnClick(node, event) {
                this.close();
                event.preventDefault();
            } ]);
        })();
    })();
    __ASSETS._a34nm9vam57e3ik9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push("<div" + jade.attrs({
            "class": (self.getClazz() || "") + " " + "dialog" + " " + "info-msg" + " " + "gray"
        }, {}) + ">");
        if (self.getHeader()) {
            buf.push("<h1>" + jade.escape(null == (jade.interp = self.getHeader()) ? "" : jade.interp) + "</h1>");
        }
        buf.push("<pre>" + jade.escape(null == (jade.interp = self.getText()) ? "" : jade.interp) + '</pre><div class="buttons-container center">');
        (function() {
            var $$obj = self.getButtons();
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var button = $$obj[$index];
                    attrs = button.getAttributes() || {};
                    attrs.clazz = (attrs.clazz || "") + " " + (button.getColor() ? button.getColor().valueOf() : "blue");
                    if (button.getController()) {
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                jade.globals.Button_mixin.call({
                                    buf: buf,
                                    attributes: {
                                        id: attrs.id,
                                        "class": attrs.clazz + " " + "special-button2"
                                    },
                                    escaped: {
                                        id: true
                                    }
                                });
                            },
                            attributes: {
                                "class": "button" + " " + "defer"
                            },
                            escaped: {}
                        }, button.getController(), button.getAction(), button.getParams() || []);
                    } else {
                        jade.globals.Button_mixin.call({
                            buf: buf,
                            attributes: {
                                id: attrs.id,
                                "class": attrs.clazz + " " + "special-button2" + " " + "button"
                            },
                            escaped: {
                                id: true
                            }
                        });
                    }
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var button = $$obj[$index];
                    attrs = button.getAttributes() || {};
                    attrs.clazz = (attrs.clazz || "") + " " + (button.getColor() ? button.getColor().valueOf() : "blue");
                    if (button.getController()) {
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                jade.globals.Button_mixin.call({
                                    buf: buf,
                                    attributes: {
                                        id: attrs.id,
                                        "class": attrs.clazz + " " + "special-button2"
                                    },
                                    escaped: {
                                        id: true
                                    }
                                });
                            },
                            attributes: {
                                "class": "button" + " " + "defer"
                            },
                            escaped: {}
                        }, button.getController(), button.getAction(), button.getParams() || []);
                    } else {
                        jade.globals.Button_mixin.call({
                            buf: buf,
                            attributes: {
                                id: attrs.id,
                                "class": attrs.clazz + " " + "special-button2" + " " + "button"
                            },
                            escaped: {
                                id: true
                            }
                        });
                    }
                }
            }
        }).call(this);
        buf.push("</div></div>");
        return buf.join("");
    };
    __ASSETS._3vynirkx0kpdgqfr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.RenderWith_mixin = function(data, tplClass) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            var tpl = new tplClass();
            tpl.options(attributes);
            tpl.setBlock && tpl.setBlock(block || null);
            if (Array.isArray(data)) {
                tpl.setCollection(data);
                (function() {
                    var $$obj = data;
                    if ("number" == typeof $$obj.length) {
                        for (var index = 0, $$l = $$obj.length; index < $$l; index++) {
                            var item = $$obj[index];
                            tpl.setCollectionIndex(index);
                            tpl.assign(item);
                            tpl.renderBuffer();
                        }
                    } else {
                        var $$l = 0;
                        for (var index in $$obj) {
                            $$l++;
                            var item = $$obj[index];
                            tpl.setCollectionIndex(index);
                            tpl.assign(item);
                            tpl.renderBuffer();
                        }
                    }
                }).call(this);
            } else {
                tpl.assign(data);
                tpl.renderBuffer();
            }
            buf.push(tpl.flushBuffer());
        };
        return buf.join("");
    };
    "ria.templates.Template";
    (function() {
        (ria = ria || {}).templates = ria.templates || {};
        (function() {
            ria.templates.CompiledTemplate = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "ria.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("CompiledTemplate", ria.__SYNTAX.EXTENDS(ria.templates.Template), [ ria.__SYNTAX.Modifiers.OVERRIDE, String, function render() {
                return this._tpl(this.getContext_());
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function bind_() {
                BASE();
                this._tpl = ASSET(this._tpl);
            } ]);
        })();
    })();
    "ria.templates.CompiledTemplate";
    (function() {
        (chlk = chlk || {}).templates = chlk.templates || {};
        (function() {
            "use strict";
            ASSET("_3vynirkx0kpdgqfr")();
            chlk.templates.JadeTemplate = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("JadeTemplate", ria.__SYNTAX.EXTENDS(ria.templates.CompiledTemplate), [ Function, "block", [ [ Object, Number ] ], String, function getPictureURL(id, sizeH_, sizeW_) {
                if (!id) return null;
                var url = window.azurePictureUrl + id.valueOf();
                if (sizeH_ && sizeW_) return url + "-" + sizeH_ + "x" + sizeW_;
                if (sizeH_) return url + "-" + sizeH_ + "x" + sizeH_;
                return url;
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.CourseId =             function wrapper() {
                var values = {};
                function CourseId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new CourseIdImpl(value);
                }
                ria.__API.identifier(CourseId, "chlk.models.id.CourseId");
                function CourseIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.CourseId#" + value + "]";
                    };
                }
                ria.__API.extend(CourseIdImpl, CourseId);
                return CourseId;
            }();
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.DepartmentId =             function wrapper() {
                var values = {};
                function DepartmentId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new DepartmentIdImpl(value);
                }
                ria.__API.identifier(DepartmentId, "chlk.models.id.DepartmentId");
                function DepartmentIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.DepartmentId#" + value + "]";
                    };
                }
                ria.__API.extend(DepartmentIdImpl, DepartmentId);
                return DepartmentId;
            }();
        })();
    })();
    "chlk.models.people.User";
    "chlk.models.id.CourseId";
    "chlk.models.id.DepartmentId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).course = chlk.models.course || {};
        (function() {
            "use strict";
            chlk.models.course.Course = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.course." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Course", [ String, "code", [ ria.serialize.SerializeProperty("departmentid") ], chlk.models.id.DepartmentId, "departmentId", chlk.models.id.CourseId, "id", String, "title" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.GradeLevelId =             function wrapper() {
                var values = {};
                function GradeLevelId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new GradeLevelIdImpl(value);
                }
                ria.__API.identifier(GradeLevelId, "chlk.models.id.GradeLevelId");
                function GradeLevelIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.GradeLevelId#" + value + "]";
                    };
                }
                ria.__API.extend(GradeLevelIdImpl, GradeLevelId);
                return GradeLevelId;
            }();
        })();
    })();
    "chlk.models.id.GradeLevelId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).grading = chlk.models.grading || {};
        (function() {
            "use strict";
            chlk.models.grading.GradeLevel = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.grading." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("GradeLevel", [ chlk.models.id.GradeLevelId, "id", String, "name", Number, "number" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.ClassId =             function wrapper() {
                var values = {};
                function ClassId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new ClassIdImpl(value);
                }
                ria.__API.identifier(ClassId, "chlk.models.id.ClassId");
                function ClassIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.ClassId#" + value + "]";
                    };
                }
                ria.__API.extend(ClassIdImpl, ClassId);
                return ClassId;
            }();
        })();
    })();
    "chlk.models.people.User";
    "chlk.models.course.Course";
    "chlk.models.grading.GradeLevel";
    "chlk.models.id.ClassId";
    "chlk.models.id.MarkingPeriodId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).class = chlk.models.class || {};
        (function() {
            "use strict";
            chlk.models.class.Class = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.class." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Class", [ chlk.models.course.Course, "course", String, "description", [ ria.serialize.SerializeProperty("gradelevel") ], chlk.models.grading.GradeLevel, "gradeLevel", chlk.models.id.ClassId, "id", [ ria.serialize.SerializeProperty("markingperiodsid") ], ria.__API.ArrayOf(chlk.models.id.MarkingPeriodId), "markingPeriodsId", String, "name", chlk.models.people.User, "teacher" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.ButtonColor =             function wrapper() {
                var values = {};
                function ButtonColor(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(ButtonColor, "chlk.models.common.ButtonColor");
                function ButtonColorImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.common.ButtonColor#" + value + "]";
                    };
                }
                ria.__API.extend(ButtonColorImpl, ButtonColor);
                values["red"] = ButtonColor.RED = new ButtonColorImpl("red");
                values["blue"] = ButtonColor.BLUE = new ButtonColorImpl("blue");
                values["green"] = ButtonColor.GREEN = new ButtonColorImpl("green");
                values["gray"] = ButtonColor.GRAY = new ButtonColorImpl("gray");
                return ButtonColor;
            }();
            chlk.models.common.Button = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Button", [ String, "controller", String, "action", Array, "params", chlk.models.common.ButtonColor, "color", Object, "attributes", String, "text" ]);
        })();
    })();
    "chlk.models.common.Button";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.InfoMsg = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("InfoMsg", [ [ [ String, String, ria.__API.ArrayOf(chlk.models.common.Button), String ] ], function $(text_, header_, buttons_, clazz_) {
                text_ && this.setText(text_);
                clazz_ && this.setClazz(clazz_);
                header_ && this.setHeader(header_);
                buttons_ && this.setButtons(buttons_);
            }, String, "text", String, "header", String, "clazz", ria.__API.ArrayOf(chlk.models.common.Button), "buttons" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.class.Class";
    "chlk.models.id.ClassId";
    "chlk.models.id.CourseId";
    "chlk.models.common.InfoMsg";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).common = chlk.templates.common || {};
        (function() {
            chlk.templates.common.InfoMsg = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_a34nm9vam57e3ik9") ], [ ria.templates.ModelBind(chlk.models.common.InfoMsg) ], "InfoMsg", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], String, "text", [ ria.templates.ModelPropertyBind ], String, "header", [ ria.templates.ModelPropertyBind ], String, "clazz", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.common.Button), "buttons" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.common.InfoMsg";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).common = chlk.activities.common || {};
        (function() {
            chlk.activities.common.InfoMsgDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.TemplateBind(chlk.templates.common.InfoMsg) ], "InfoMsgDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), [ [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onRender_(data) {
                BASE(data);
                var node = this.dom.find(".centered");
                node.setCss("margin-top", -parseInt(node.height() / 2, 10) + "px");
                node.setCss("margin-left", -parseInt(node.width() / 2, 10) + "px");
            }, [ ria.mvc.DomEventBind("click", "button, a") ], [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function onBtnClick(node, event) {
                this.close();
                event.preventDefault();
            } ]);
        })();
    })();
    "ria.mvc.Controller";
    "chlk.models.common.Role";
    "chlk.models.people.User";
    "chlk.activities.common.InfoMsgDialog";
    "chlk.models.common.InfoMsg";
    "chlk.models.common.Button";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.SidebarButton = ria.__API.annotation("chlk.controllers.SidebarButton", [ String ], [ "clazz" ]);
            chlk.controllers.AccessForRoles = ria.__API.annotation("chlk.controllers.AccessForRoles", [ ria.__API.ArrayOf(chlk.models.common.RoleEnum) ], [ "roles" ]);
            function toCamelCase(str) {
                return str.replace(/(\-[a-z])/g, function($1) {
                    return $1.substring(1).toUpperCase();
                });
            }
            var PRESSED_CLS = "pressed";
            var ACTION_SUFFIX = "Action";
            var SIDEBAR_CONTROLS_ID = "#sidebar-controls";
            ria.__SYNTAX.Modifiers.ABSTRACT, chlk.controllers.BaseController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BaseController", ria.__SYNTAX.EXTENDS(ria.mvc.Controller), [ ria.async.Future, function validateResponse_() {
                var head, me = this;
                (head = new ria.async.Future()).catchError(function(error) {
                    throw chlk.services.DataException("Failed to load data", error);
                }).then(function(data) {
                    return data;
                }).catchException(chlk.services.DataException, function(error) {
                    ria.async.BREAK;
                    console.error(error.toString());
                });
                return head;
            }, chlk.models.common.Role, function getCurrentRole() {
                return this.getContext().getSession().get("role");
            }, [ [ String, String, Array, String ] ], function ShowMsgBox(text_, header_, buttons_, clazz_) {
                var instance = new chlk.activities.common.InfoMsgDialog();
                var buttons = [];
                if (buttons_) {
                    var serializer = new ria.serialize.JsonSerializer();
                    buttons_.forEach(function(item) {
                        buttons.push(serializer.deserialize(item, chlk.models.common.Button));
                    });
                } else {
                    var button = new chlk.models.common.Button();
                    button.setText("Ok");
                    buttons.push(button);
                }
                var model = new chlk.models.common.InfoMsg(text_, header_, buttons, clazz_);
                this.view.shadeD(instance, ria.async.DeferredData(model));
            }, [ [ chlk.models.common.RoleEnum ] ], Boolean, function userInRole(roleId) {
                return this.getCurrentRole().getRoleId() == roleId;
            }, Boolean, function userIsAdmin() {
                return this.userInRole(chlk.models.common.RoleEnum.ADMINEDIT) || this.userInRole(chlk.models.common.RoleEnum.ADMINGRADE) || this.userInRole(chlk.models.common.RoleEnum.ADMINVIEW);
            }, chlk.models.people.User, function getCurrentPerson() {
                return this.getContext().getSession().get("currentPerson");
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.reflection.ReflectionMethod, function resolveRoleAction_(state) {
                var ref = new ria.reflection.ReflectionClass(this.getClass());
                var role = this.getContext().getSession().get("role");
                var roleAction = toCamelCase(state.getAction()) + role.getRoleName() + "Action";
                var method = ref.getMethodReflector(roleAction);
                if (!method) {
                    method = BASE(state);
                    var accessForAnnotation = method.findAnnotation(chlk.controllers.AccessForRoles)[0];
                    if (accessForAnnotation) {
                        var filteredRoles = accessForAnnotation.roles.filter(function(r) {
                            return r == role.getRoleId();
                        });
                        if (filteredRoles.length != 1) {
                            throw new ria.mvc.MvcException("Controller " + ref.getName() + " has no method " + method.getName() + " available for role " + role.getRoleName());
                        }
                    }
                }
                return method;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function postDispatchAction_() {
                BASE();
                var state = this.context.getState();
                new ria.dom.Dom(SIDEBAR_CONTROLS_ID + " ." + PRESSED_CLS).removeClass(PRESSED_CLS);
                var methodReflector = this.resolveRoleAction_(state);
                if (methodReflector.isAnnotatedWith(chlk.controllers.SidebarButton)) {
                    var buttonCls = methodReflector.findAnnotation(chlk.controllers.SidebarButton)[0].clazz;
                    new ria.dom.Dom(SIDEBAR_CONTROLS_ID + " ." + buttonCls).addClass(PRESSED_CLS);
                }
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.Timezone = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Timezone", [ String, "id", [ ria.serialize.SerializeProperty("displayname") ], String, "displayName", String, "standartTime", String, "daylightName", Boolean, "supportsDaylightSavingTime" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.DistrictId =             function wrapper() {
                var values = {};
                function DistrictId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new DistrictIdImpl(value);
                }
                ria.__API.identifier(DistrictId, "chlk.models.id.DistrictId");
                function DistrictIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.DistrictId#" + value + "]";
                    };
                }
                ria.__API.extend(DistrictIdImpl, DistrictId);
                return DistrictId;
            }();
        })();
    })();
    "chlk.models.id.DistrictId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).district = chlk.models.district || {};
        (function() {
            "use strict";
            chlk.models.district.District = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.district." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("District", [ chlk.models.id.DistrictId, "id", String, "name", [ ria.serialize.SerializeProperty("sisurl") ], String, "sisUrl", [ ria.serialize.SerializeProperty("dbname") ], String, "dbName", [ ria.serialize.SerializeProperty("sisusername") ], String, "sisUserName", [ ria.serialize.SerializeProperty("sispassword") ], String, "sisPassword", [ ria.serialize.SerializeProperty("sissystemtype") ], Number, "sisSystemType" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.SchoolId =             function wrapper() {
                var values = {};
                function SchoolId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new SchoolIdImpl(value);
                }
                ria.__API.identifier(SchoolId, "chlk.models.id.SchoolId");
                function SchoolIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.SchoolId#" + value + "]";
                    };
                }
                ria.__API.extend(SchoolIdImpl, SchoolId);
                return SchoolId;
            }();
        })();
    })();
    "chlk.models.school.Timezone";
    "chlk.models.district.District";
    "chlk.models.id.SchoolId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.School = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("School", [ chlk.models.id.SchoolId, "id", String, "name", [ ria.serialize.SerializeProperty("localid") ], Number, "localId", [ ria.serialize.SerializeProperty("ncesid") ], Number, "ncesId", [ ria.serialize.SerializeProperty("schooltype") ], String, "schoolType", [ ria.serialize.SerializeProperty("schoolurl") ], String, "schoolUrl", [ ria.serialize.SerializeProperty("sendemailnotofications") ], Boolean, "sendEmailNotifications", [ ria.serialize.SerializeProperty("timezoneid") ], String, "timezoneId", ria.__API.ArrayOf(chlk.models.school.Timezone), "timezones", [ ria.serialize.SerializeProperty("districtid") ], chlk.models.id.DistrictId, "districtId" ]);
        })();
    })();
    "chlk.models.common.ChlkDate";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).import = chlk.models.import || {};
        (function() {
            "use strict";
            chlk.models.import.SchoolYear = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.import." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolYear", [ String, "name", [ ria.serialize.SerializeProperty("calendarid") ], Number, "calendarId", [ ria.serialize.SerializeProperty("schoolid") ], Number, "sisSchoolId", [ ria.serialize.SerializeProperty("startdate") ], chlk.models.common.ChlkDate, "startDate", [ ria.serialize.SerializeProperty("enddate") ], chlk.models.common.ChlkDate, "endDate" ]);
        })();
    })();
    "chlk.models.import.SchoolYear";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).import = chlk.models.import || {};
        (function() {
            "use strict";
            chlk.models.import.School = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.import." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("School", [ String, "address", String, "city", String, "email", String, "name", String, "phone", [ ria.serialize.SerializeProperty("principalemail") ], String, "principalEmail", [ ria.serialize.SerializeProperty("principalname") ], String, "principalName", [ ria.serialize.SerializeProperty("principaltitle") ], String, "principalTitle", [ ria.serialize.SerializeProperty("schoolid") ], Number, "sisSchoolId", [ ria.serialize.SerializeProperty("schoolyears") ], ria.__API.ArrayOf(chlk.models.import.SchoolYear), "schoolYears" ]);
        })();
    })();
    "chlk.models.school.School";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.SchoolDetails = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolDetails", ria.__SYNTAX.EXTENDS(chlk.models.school.School), [ [ ria.serialize.SerializeProperty("statusnumber") ], Number, "statusNumber", String, "status", ria.__API.ArrayOf(Number), "buttons", ria.__API.ArrayOf(String), "emails" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.SchoolSisInfoId =             function wrapper() {
                var values = {};
                function SchoolSisInfoId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new SchoolSisInfoIdImpl(value);
                }
                ria.__API.identifier(SchoolSisInfoId, "chlk.models.id.SchoolSisInfoId");
                function SchoolSisInfoIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.SchoolSisInfoId#" + value + "]";
                    };
                }
                ria.__API.extend(SchoolSisInfoIdImpl, SchoolSisInfoId);
                return SchoolSisInfoId;
            }();
        })();
    })();
    "chlk.models.id.SchoolSisInfoId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.SchoolSisInfo = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolSisInfo", [ Number, "attendanceSyncFreq", Number, "disciplineSyncFreq", Number, "personSyncFreq", Number, "scheduleSyncFreq", chlk.models.id.SchoolSisInfoId, "id", String, "sisUrl", String, "sisUserName", String, "sisPassword", String, "sisName" ]);
        })();
    })();
    (function() {
        (chlk = chlk || {}).models = chlk.models || {};
        (function() {
            "use strict";
            chlk.models.Success = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Success", [ Boolean, "data" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.school.School";
    "chlk.models.import.School";
    "chlk.models.school.SchoolDetails";
    "chlk.models.school.Timezone";
    "chlk.models.school.SchoolSisInfo";
    "chlk.models.district.District";
    "chlk.models.id.SchoolId";
    "chlk.models.Success";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.SchoolService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.DistrictId, Number, Boolean, Boolean ] ], ria.async.Future, function getSchools(districtId, start_, demoOnly_, unimportedOnly_) {
                return this.getPaginatedList("School/List.json", chlk.models.school.School, {
                    start: start_,
                    districtId: districtId.valueOf(),
                    demoOnly: demoOnly_,
                    unimportedOnly: unimportedOnly_
                });
            }, [ [ chlk.models.id.DistrictId ] ], ria.async.Future, function getSchoolsForImport(districtId) {
                return this.getPaginatedList("School/GetSchoolsForImport.json", chlk.models.import.School, {
                    districtId: districtId.valueOf()
                });
            }, [ [ chlk.models.id.DistrictId, Number, Number ] ], ria.async.Future, function runSchoolImport(districtId, sisSchoolId, sisSchoolYearId) {
                return this.get("School/RunSchoolImport.json", Boolean, {
                    districtId: districtId.valueOf(),
                    sisSchoolId: sisSchoolId,
                    sisSchoolYearId: sisSchoolYearId
                });
            }, [ [ chlk.models.id.SchoolId ] ], ria.async.Future, function getDetails(schoolId) {
                return this.get("School/Summary.json", chlk.models.school.SchoolDetails, {
                    schoolId: schoolId.valueOf()
                });
            }, [ [ chlk.models.id.SchoolId ] ], ria.async.Future, function getSisInfo(schoolId) {
                return this.get("chalkable2/data/schoolSisInfo.json", chlk.models.school.SchoolSisInfo, {
                    schoolId: schoolId.valueOf()
                });
            }, [ [ chlk.models.id.SchoolId ] ], ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get("School/People.json", chlk.models.school.SchoolPeopleSummary, {
                    schoolId: schoolId.valueOf()
                });
            }, [ [ chlk.models.id.SchoolId ] ], ria.async.Future, function del(schoolId) {
                return this.post("School/delete.json", chlk.models.Success, {
                    schoolId: schoolId.valueOf()
                });
            }, ria.async.Future, function getTimezones() {
                return this.getPaginatedList("School/ListTimezones.json", chlk.models.school.Timezone, {});
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.id.SchoolId";
    "chlk.models.people.User";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.AdminService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AdminService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.SchoolId, Number, Number, Boolean, Number ] ], ria.async.Future, function getUsers(schoolId, roleId, gradeLevelId, byLastName, start) {
                return this.getPaginatedList("Admin/GetPersons.json", chlk.models.people.User, {
                    schoolId: schoolId.valueOf(),
                    start: start,
                    roleId: roleId,
                    gradeLevelId: gradeLevelId,
                    byLastName: byLastName
                });
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.SimpleResult = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SimpleResult", [ Boolean, "success" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.common.SimpleResult";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.AccountService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AccountService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ ria.async.Future, function getRoles() {
                return this.get("chalkable2/app/data/roles.json", ria.__API.ArrayOf(chlk.models.common.NameId), {});
            }, ria.async.Future, function logOut() {
                return this.post("User/LogOut.json", chlk.models.common.SimpleResult, {});
            }, [ [ String, String, String ] ], ria.async.Future, function changePassword(oldPassword, newPassword, newPasswordConfirmation) {
                return this.get("user/ChangePassword.json", Boolean, {
                    oldPassword: oldPassword,
                    newPassword: newPassword,
                    newPasswordConfirmation: newPasswordConfirmation
                });
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AppGradeLevelId =             function wrapper() {
                var values = {};
                function AppGradeLevelId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AppGradeLevelIdImpl(value);
                }
                ria.__API.identifier(AppGradeLevelId, "chlk.models.id.AppGradeLevelId");
                function AppGradeLevelIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AppGradeLevelId#" + value + "]";
                    };
                }
                ria.__API.extend(AppGradeLevelIdImpl, AppGradeLevelId);
                return AppGradeLevelId;
            }();
        })();
    })();
    "ria.serialize.IDeserializable";
    "chlk.models.id.AppGradeLevelId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppGradeLevel = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppGradeLevel", [ chlk.models.id.AppGradeLevelId, "id", String, "name", [ [ chlk.models.id.AppGradeLevelId, String ] ], function $(id_, name_) {
                if (id_) this.setId(id_);
                if (name_) this.setName(name_);
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "chlk.models.id.GradeLevelId";
    "chlk.models.apps.AppGradeLevel";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.GradeLevelService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("GradeLevelService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ ria.__API.ArrayOf(chlk.models.apps.AppGradeLevel), function getGradeLevels() {
                return [ new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(1), "1st"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(2), "2nd"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(3), "3rd"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(4), "4th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(5), "5th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(6), "6th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(7), "7th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(8), "8th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(9), "9th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(10), "10th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(11), "11th"), new chlk.models.apps.AppGradeLevel(new chlk.models.id.AppGradeLevelId(12), "12th") ].reverse();
            } ]);
        })();
    })();
    "chlk.models.school.SchoolDetails";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.SchoolPeopleSummary = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolPeopleSummary", ria.__SYNTAX.EXTENDS(chlk.models.school.SchoolDetails), [ [ ria.serialize.SerializeProperty("invitescount") ], Number, "invitesCount", [ ria.serialize.SerializeProperty("staffcount") ], Number, "staffCount", [ ria.serialize.SerializeProperty("studentscount") ], Number, "studentsCount", [ ria.serialize.SerializeProperty("teacherscount") ], Number, "teachersCount" ]);
        })();
    })();
    "chlk.models.common.PaginatedList";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.UsersList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("UsersList", [ chlk.models.common.PaginatedList, "users", Number, "selectedIndex", Boolean, "byLastName", String, "filter", Number, "start" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.NameId = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("NameId", [ Number, "id", String, "name", [ [ Number, String ] ], function $(id_, name_) {
                if (id_) this.setId(id_);
                if (name_) this.setName(name_);
            } ]);
        })();
    })();
    "chlk.models.school.SchoolPeopleSummary";
    "chlk.models.people.UsersList";
    "chlk.models.common.NameId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.SchoolPeople = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolPeople", [ chlk.models.people.UsersList, "usersPart", ria.__API.ArrayOf(chlk.models.common.NameId), "roles", ria.__API.ArrayOf(chlk.models.common.NameId), "gradeLevels", chlk.models.school.SchoolPeopleSummary, "schoolInfo" ]);
        })();
    })();
    "ria.mvc.TemplateActivity";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).lib = chlk.activities.lib || {};
        (function() {
            chlk.activities.lib.PageClass = ria.__API.annotation("chlk.activities.lib.PageClass", [ String ], [ "clazz" ]);
            chlk.activities.lib.BodyClass = ria.__API.annotation("chlk.activities.lib.BodyClass", [ String ], [ "clazz" ]);
            var LOADING_CLASS = "loading-page";
            chlk.activities.lib.TemplatePage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.lib." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TemplatePage", ria.__SYNTAX.EXTENDS(ria.mvc.TemplateActivity), [ function $() {
                BASE();
                this._wrapper = new ria.dom.Dom("#content-wrapper");
                this._body = new ria.dom.Dom("body");
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function processAnnotations_(ref) {
                BASE(ref);
                if (ref.isAnnotatedWith(chlk.activities.lib.PageClass)) {
                    this._pageClass = ref.findAnnotation(chlk.activities.lib.PageClass)[0].clazz;
                } else {
                    this._pageClass = null;
                }
                if (ref.isAnnotatedWith(chlk.activities.lib.BodyClass)) {
                    this._bodyClass = ref.findAnnotation(chlk.activities.lib.BodyClass)[0].clazz;
                } else {
                    this._bodyClass = null;
                }
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onRender_(model) {
                BASE(model);
                this._pageClass && this._wrapper.addClass(this._pageClass);
                this._bodyClass && this._body.addClass(this._bodyClass);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function startFullLoading() {
                BASE();
                this.dom.addClass(LOADING_CLASS);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function stopLoading() {
                BASE();
                this.dom.removeClass(LOADING_CLASS);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onStop_() {
                BASE();
                this._pageClass && this._wrapper.removeClass(this._pageClass);
                this._bodyClass && this._body.removeClass(this._bodyClass);
            } ]);
        })();
    })();
    __ASSETS._c6y20ai4cpk9ms4i = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="school-details">');
        buttonText = getActionButtons()[self.getButtons()[0] - 1];
        buf.push('<div class="action-bar not-transparent buttons"><div class="container panel-bg"><!--move this to some mixin--><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Now");
            },
            attributes: {
                "class": "pressed"
            },
            escaped: {}
        }, "schools", "details", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Schedule");
            }
        }, "schools", "details", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("People");
            }
        }, "schools", "people", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("SIS");
            }
        }, "schools", "sisInfo", self.getId());
        buf.push('</div></div></div><div class="content profile-page-margin"><div class="left">');
        jade.globals.PhotoContainer_mixin.call({
            buf: buf
        }, "https://dev.chalkable.com/PersonPicture/Read?pictureId=132310&width=256&height=256&gender=m");
        buf.push('</div><div class="left school-info"><h1>' + jade.escape(null == (jade.interp = self.getName()) ? "" : jade.interp) + '</h1><div class="status">' + jade.escape((jade.interp = self.getStatusNumber()) == null ? "" : jade.interp) + ". " + jade.escape((jade.interp = self.getStatus()) == null ? "" : jade.interp) + "");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        "class": "special-button2" + " " + "blue"
                    },
                    escaped: {}
                });
            },
            attributes: {
                id: "show-buttons-popup"
            },
            escaped: {}
        }, "schools", "actionButtons", self.getId());
        buf.push('</div></div><div class="clear"></div></div></div>');
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.school.School";
    "chlk.models.id.SchoolId";
    "chlk.models.id.DistrictId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).school = chlk.templates.school || {};
        (function() {
            chlk.templates.school.School = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.ModelBind(chlk.models.school.School) ], "School", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolId, "id", [ ria.templates.ModelPropertyBind ], String, "name", [ ria.templates.ModelPropertyBind ], String, "localId", [ ria.templates.ModelPropertyBind ], String, "ncesId", [ ria.templates.ModelPropertyBind ], String, "schoolType", [ ria.templates.ModelPropertyBind ], String, "schoolUrl", [ ria.templates.ModelPropertyBind ], String, "timezoneId", [ ria.templates.ModelPropertyBind ], Boolean, "sendEmailNotifications", [ ria.templates.ModelPropertyBind ], chlk.models.id.DistrictId, "districtId" ]);
        })();
    })();
    "chlk.templates.school.School";
    "chlk.models.school.SchoolDetails";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).school = chlk.templates.school || {};
        (function() {
            chlk.templates.school.SchoolDetails = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_c6y20ai4cpk9ms4i") ], [ ria.templates.ModelBind(chlk.models.school.SchoolDetails) ], "SchoolDetails", ria.__SYNTAX.EXTENDS(chlk.templates.school.School), [ [ ria.templates.ModelPropertyBind ], Number, "statusNumber", [ ria.templates.ModelPropertyBind ], String, "status", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(Number), "buttons", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(String), "emails" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.school.SchoolDetails";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).school = chlk.activities.school || {};
        (function() {
            chlk.activities.school.SchoolDetailsPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.TemplateBind(chlk.templates.school.SchoolDetails) ], "SchoolDetailsPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._nc1rktwz2n6fxbt9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="school-people">');
        school = self.getSchoolInfo();
        roles = self.getRoles();
        gradeLevels = self.getGradeLevels();
        buf.push('<div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Now");
            }
        }, "schools", "details", school.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Schedule");
            }
        }, "schools", "details", school.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("People");
            },
            attributes: {
                "class": "pressed"
            },
            escaped: {}
        }, "schools", "people", school.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("SIS");
            }
        }, "schools", "details", school.getId());
        buf.push('</div></div></div><div class="content profile-page-margin"><div class="i-b">');
        jade.globals.PhotoContainer_mixin.call({
            buf: buf
        }, "https://dev.chalkable.com/PersonPicture/Read?pictureId=132310&width=256&height=256&gender=m");
        buf.push('</div><h1 class="i-b v-t">' + jade.escape(null == (jade.interp = school.getName()) ? "" : jade.interp) + '</h1><div class="line"></div><div class="center glance-container">');
        jade.globals.GlanceBox_mixin.call({
            buf: buf
        }, null, null, {
            value: school.getTeachersCount(),
            title: "Teachers"
        });
        jade.globals.GlanceBox_mixin.call({
            buf: buf
        }, null, null, {
            value: school.getStaffCount(),
            title: "Staff"
        });
        jade.globals.GlanceBox_mixin.call({
            buf: buf
        }, null, null, {
            value: school.getStudentsCount(),
            title: "Sudents"
        });
        jade.globals.GlanceBox_mixin.call({
            buf: buf
        }, null, null, {
            value: school.getInvitesCount(),
            title: "Invites"
        });
        buf.push("</div><h2>People</h2>");
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        type: "submit",
                        "class": "x-hidden"
                    },
                    escaped: {
                        type: true
                    }
                });
                buf.push('<div class="loader-container"><div class="loader"></div><div class="action-bar not-transparent people-tb"><div class="container panel-bg"><div class="right">');
                jade.globals.Select_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = roles;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.Option_mixin.call({
                                        buf: buf
                                    }, item.getId(), item.getName());
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.Option_mixin.call({
                                        buf: buf
                                    }, item.getId(), item.getName());
                                }
                            }
                        }).call(this);
                    },
                    attributes: {
                        id: "roleId"
                    },
                    escaped: {}
                }, "roleId");
                jade.globals.Select_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = gradeLevels;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.Option_mixin.call({
                                        buf: buf
                                    }, item.getId(), item.getName());
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.Option_mixin.call({
                                        buf: buf
                                    }, item.getId(), item.getName());
                                }
                            }
                        }).call(this);
                    },
                    attributes: {
                        id: "gradeLevelId"
                    },
                    escaped: {}
                }, "gradeLevelId");
                buf.push("</div></div></div>");
                jade.globals.RenderWith_mixin.call({
                    buf: buf
                }, self.getUsersPart(), chlk.templates.people.UsersList);
                buf.push("</div>");
            },
            attributes: {
                id: "people-form"
            },
            escaped: {}
        }, "schools", "setPeopleFilter");
        buf.push("</div></div>");
        return buf.join("");
    };
    "chlk.models.school.SchoolPeopleSummary";
    "chlk.models.school.SchoolPeople";
    "chlk.models.people.UsersList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).school = chlk.templates.school || {};
        (function() {
            chlk.templates.school.SchoolPeople = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_nc1rktwz2n6fxbt9") ], [ ria.templates.ModelBind(chlk.models.school.SchoolPeople) ], "SchoolPeople", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.people.UsersList, "usersPart", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.common.NameId), "roles", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.common.NameId), "gradeLevels", [ ria.templates.ModelPropertyBind ], chlk.models.school.SchoolPeopleSummary, "schoolInfo" ]);
        })();
    })();
    __ASSETS._4ionxqe3es3gzaor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        users = self.getUsers();
        selectedIndex = self.getSelectedIndex();
        buf.push('<div class="people-list-container not-transparent"><div class="top panel-bg"><span class="total-count">' + jade.escape((jade.interp = users.getTotalCount()) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = Msg.Person(users.getTotalCount() != 1)) == null ? "" : jade.interp) + '</span></div><div class="action-bar buttons filter"><div class="container panel-bg"><div class="left"><A' + jade.attrs({
            "data-value": false,
            "class": (self.isByLastName() ? "" : "pressed") + " " + "action-button" + " " + "first-last"
        }, {
            "data-value": true
        }) + ">First name</A><A" + jade.attrs({
            "data-value": true,
            "class": (self.isByLastName() ? "pressed" : "") + " " + "action-button" + " " + "first-last"
        }, {
            "data-value": true
        }) + ">Last name</A><input" + jade.attrs({
            type: "hidden",
            name: "byLastName",
            value: self.isByLastName()
        }, {
            type: true,
            name: true,
            value: true
        }) + '/></div><div class="right"><input' + jade.attrs({
            placeholder: "Filter list...",
            name: "filter",
            value: self.getFilter(),
            "class": "people-search"
        }, {
            placeholder: true,
            name: true,
            value: true
        }) + '/><div class="people-search-imgs"><label class="people-search-img"></label><label class="people-search-close opacity0"></label></div></div></div></div><div class="grid-container loader-container">');
        jade.globals.RenderWith_mixin.call({
            buf: buf
        }, self._model, chlk.templates.people.UsersGrid);
        buf.push("</div></div>");
        return buf.join("");
    };
    "chlk.models.people.UsersList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).people = chlk.templates.people || {};
        (function() {
            chlk.templates.people.UsersList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_4ionxqe3es3gzaor") ], [ ria.templates.ModelBind(chlk.models.people.UsersList) ], "UsersList", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.common.PaginatedList, "users", [ ria.templates.ModelPropertyBind ], Number, "selectedIndex", [ ria.templates.ModelPropertyBind ], Boolean, "byLastName", [ ria.templates.ModelPropertyBind ], String, "filter", [ ria.templates.ModelPropertyBind ], Number, "start" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.school.SchoolPeople";
    "chlk.templates.people.UsersList";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).school = chlk.activities.school || {};
        (function() {
            chlk.activities.school.SchoolPeoplePage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.TemplateBind(chlk.templates.school.SchoolPeople) ], [ ria.mvc.PartialUpdateRule(chlk.templates.people.UsersList, "", ".people-list-container", ria.mvc.PartialUpdateRuleActions.Replace) ], "SchoolPeoplePage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("change", "#roleId") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function roleSelect(node, event) {
                this.dom.find("#people-form").triggerEvent("submit");
            }, [ ria.mvc.DomEventBind("change", "#gradeLevelId") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function gradeLevelSelect(node, event) {
                this.dom.find("#people-form").triggerEvent("submit");
            }, [ ria.mvc.DomEventBind("click", ".action-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function actionButtonClick(node, event) {
                var value = node.getData("value");
                this.dom.find('input[name="byLastName"]').setValue(value || "");
                this.dom.find("#people-form").triggerEvent("submit");
            } ]);
        })();
    })();
    __ASSETS._87nbv7ihkkvuz0k9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="action-buttons-popup"><div class="buttons">');
        (function() {
            var $$obj = getActionButtons();
            if ("number" == typeof $$obj.length) {
                for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                    var button = $$obj[i];
                    index = i + 1;
                    clazz = self.getButtons()[0] == index ? "blue " : self.getButtons().indexOf(index) == -1 ? "grey disabled " : "green ";
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        attributes: {
                            "data-index": index,
                            "class": "special-button2 action-link-button " + clazz
                        },
                        escaped: {
                            "class": true,
                            "data-index": true
                        }
                    });
                }
            } else {
                var $$l = 0;
                for (var i in $$obj) {
                    $$l++;
                    var button = $$obj[i];
                    index = i + 1;
                    clazz = self.getButtons()[0] == index ? "blue " : self.getButtons().indexOf(index) == -1 ? "grey disabled " : "green ";
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        attributes: {
                            "data-index": index,
                            "class": "special-button2 action-link-button " + clazz
                        },
                        escaped: {
                            "class": true,
                            "data-index": true
                        }
                    });
                }
            }
        }).call(this);
        buf.push("</div>");
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Select_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.getEmails();
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var email = $$obj[$index];
                                    jade.globals.Option_mixin.call({
                                        buf: buf
                                    }, email, email);
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var email = $$obj[$index];
                                    jade.globals.Option_mixin.call({
                                        buf: buf
                                    }, email, email);
                                }
                            }
                        }).call(this);
                    },
                    attributes: {
                        id: "admin-emails"
                    },
                    escaped: {}
                }, "email");
                buf.push('<input type="hidden" name="index"/><input class="submit x-hidden"/>');
            },
            attributes: {
                id: "emails-form",
                "class": "emails"
            },
            escaped: {}
        }, "schools", "actionLink");
        buf.push("</div>");
        return buf.join("");
    };
    (function() {
        (chlk = chlk || {}).models = chlk.models || {};
        (function() {
            "use strict";
            chlk.models.Popup = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Popup", [ ria.dom.Dom, "target", ria.dom.Dom, "container" ]);
        })();
    })();
    "chlk.models.Popup";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.ActionButtons = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ActionButtons", ria.__SYNTAX.EXTENDS(chlk.models.Popup), [ ria.__API.ArrayOf(Number), "buttons", ria.__API.ArrayOf(String), "emails" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.Popup";
    (function() {
        (chlk = chlk || {}).templates = chlk.templates || {};
        (function() {
            chlk.templates.Popup = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.ModelBind(chlk.models.Popup) ], "Popup", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], ria.dom.Dom, "target", [ ria.templates.ModelPropertyBind ], ria.dom.Dom, "container" ]);
        })();
    })();
    "chlk.models.school.ActionButtons";
    "chlk.templates.Popup";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).school = chlk.templates.school || {};
        (function() {
            chlk.templates.school.ActionButtons = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_87nbv7ihkkvuz0k9") ], [ ria.templates.ModelBind(chlk.models.school.ActionButtons) ], "ActionButtons", ria.__SYNTAX.EXTENDS(chlk.templates.Popup), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(Number), "buttons", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(String), "emails" ]);
        })();
    })();
    "ria.mvc.TemplateActivity";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).lib = chlk.activities.lib || {};
        (function() {
            var HIDDEN_CLASS = "x-hidden";
            var positionClasses = {
                top: "popup-top",
                left: "popup-left",
                right: "popup-right",
                bottom: "popup-bottom"
            };
            chlk.activities.lib.IsHorizontalAxis = ria.__API.annotation("chlk.activities.lib.IsHorizontalAxis", [ Boolean ], [ "isHorizontal" ]);
            chlk.activities.lib.isTopLeftPosition = ria.__API.annotation("chlk.activities.lib.isTopLeftPosition", [ Boolean ], [ "isTopLeft" ]);
            chlk.activities.lib.TemplatePopup = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.lib." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-pop-up-container") ], "TemplatePopup", ria.__SYNTAX.EXTENDS(ria.mvc.TemplateActivity), [ function $() {
                BASE();
                this._body = new ria.dom.Dom("body");
                this.setContainer(this._body);
                this._popupHolder = new ria.dom.Dom("#chlk-pop-up-container");
                this._clickMeHandler = function(target, event) {
                    if (!this._popupHolder.areEquals(target) && !this._popupHolder.contains(target)) this.close();
                }.bind(this);
            }, ria.dom.Dom, "target", ria.dom.Dom, "container", String, "currentClass", ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function processAnnotations_(ref) {
                BASE(ref);
                if (ref.isAnnotatedWith(chlk.activities.lib.IsHorizontalAxis)) {
                    this._isHorizontal = ref.findAnnotation(chlk.activities.lib.IsHorizontalAxis)[0].isHorizontal;
                } else {
                    throw new ria.mvc.MvcException("There is no chlk.activities.lib.IsHorizontalAxis annotation for Popup activity");
                }
                this._isTopLeft = ref.isAnnotatedWith(chlk.activities.lib.isTopLeftPosition) ? ref.findAnnotation(chlk.activities.lib.isTopLeftPosition)[0].isTopLeft : true;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onResume_() {
                BASE();
                this._body.on("click", this._clickMeHandler);
            }, [ [ Object ] ], ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onRefresh_(model) {
                BASE(model);
                var target = model.getTarget();
                var offset = target.offset();
                this._popupHolder = model.getContainer() || this._popupHolder;
                this._popupHolder.removeClass(HIDDEN_CLASS);
                if (!target) throw new ria.mvc.MvcException("There is no target for Popup activity");
                var container = this.getContainer(), res;
                if (this._isHorizontal) {
                    this._popupHolder.setCss("top", offset.top - 10);
                    if (this._isTopLeft && container.offset().left + container.width() > this._popupHolder.offset().left + this._popupHolder.width()) {
                        this._popupHolder.setCss("left", offset.left - 10 - this._popupHolder.width());
                        res = positionClasses.left;
                    } else {
                        this._popupHolder.setCss("left", offset.left + 10 + target.width());
                        res = positionClasses.right;
                    }
                } else {
                    this._popupHolder.setCss("left", offset.left - (this._popupHolder.width() - target.width()) / 2);
                    if (this._isTopLeft && container.offset().top + container.height() > this._popupHolder.offset().top + this._popupHolder.height()) {
                        this._popupHolder.setCss("top", offset.top - 10 - this._popupHolder.height());
                        res = positionClasses.top;
                    } else {
                        this._popupHolder.setCss("top", offset.top + 10 + target.height());
                        res = positionClasses.bottom;
                    }
                }
                this._popupHolder.addClass(res);
                this.setCurrentClass(res);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onPause_() {
                this._popupHolder.addClass(HIDDEN_CLASS);
                this._popupHolder.removeClass(this.getCurrentClass());
                this._body.off("click", this._clickMeHandler);
                BASE();
            } ]);
        })();
    })();
    "chlk.templates.school.ActionButtons";
    "chlk.activities.lib.TemplatePopup";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).school = chlk.activities.school || {};
        (function() {
            chlk.activities.school.ActionButtonsPopup = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-pop-up-container") ], [ chlk.activities.lib.IsHorizontalAxis(false) ], [ chlk.activities.lib.isTopLeftPosition(false) ], [ ria.mvc.TemplateBind(chlk.templates.school.ActionButtons) ], "ActionButtonsPopup", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePopup), [ [ ria.mvc.DomEventBind("click", ".action-link-button:not(.disabled)") ], [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function handleClick(node, event) {
                var index = node.getData("index");
                var form = this.getDom().find("#emails-form");
                form.setFormValues({
                    index: index
                });
                form.triggerEvent("submit");
                return false;
            } ]);
        })();
    })();
    __ASSETS._r5wp2gz1jz90y66r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="school-sis"><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Now");
            }
        }, "schools", "details", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Schedule");
            }
        }, "schools", "details", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("People");
            }
        }, "schools", "people", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("SIS");
            },
            attributes: {
                "class": "pressed"
            },
            escaped: {}
        }, "schools", "sisInfo", self.getId());
        buf.push('</div></div></div><div class="content"><div class="sis-user-info">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="x-form-field"><label>Url:</label><input' + jade.attrs({
                    type: "text",
                    name: "sisUrl",
                    value: self.sisUrl
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>Name:</label><input' + jade.attrs({
                    type: "text",
                    name: "sisName",
                    value: self.sisName
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>User Name:</label><input' + jade.attrs({
                    type: "text",
                    name: "sisUserName",
                    value: self.sisUserName
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>Password:</label><input' + jade.attrs({
                    type: "text",
                    name: "sisPassword",
                    value: self.sisPassword
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + "/></div>");
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Save");
                    },
                    attributes: {
                        type: "submit",
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {
                        type: true
                    }
                });
            }
        }, "schools", "updateSisInfo");
        buf.push('</div><div class="sis-sync-info">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="x-form-field"><label>Attendance sync frequency:</label><input type="text" name="attendanceSyncFreq"/>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Run");
                    },
                    attributes: {
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {}
                });
                buf.push('</div><div class="x-form-field"><label>Discipline sync frequency:</label><input type="text" name="disciplineSyncFreq"/>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Run");
                    },
                    attributes: {
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {}
                });
                buf.push('</div><div class="x-form-field"><label>Person sync frequency:</label><input type="text" name="personSyncFreq"/>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Run");
                    },
                    attributes: {
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {}
                });
                buf.push('</div><div class="x-form-field"><label>Schedule sync frequency:</label><input type="text" name="scheduleSyncFreq"/>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Run");
                    },
                    attributes: {
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {}
                });
                buf.push("</div>");
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Save");
                    },
                    attributes: {
                        type: "submit",
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {
                        type: true
                    }
                });
            }
        }, "schools", "updateSisSyncInfo");
        buf.push("</div></div></div>");
        return buf.join("");
    };
    "chlk.models.school.SchoolSisInfo";
    "chlk.models.id.SchoolSisInfoId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).school = chlk.templates.school || {};
        (function() {
            chlk.templates.school.SchoolSisInfo = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_r5wp2gz1jz90y66r") ], [ ria.templates.ModelBind(chlk.models.school.SchoolSisInfo) ], "SchoolSisInfo", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Number, "attendanceSyncFreq", [ ria.templates.ModelPropertyBind ], Number, "disciplineSyncFreq", [ ria.templates.ModelPropertyBind ], Number, "personSyncFreq", [ ria.templates.ModelPropertyBind ], Number, "scheduleSyncFreq", [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolSisInfoId, "id", [ ria.templates.ModelPropertyBind ], String, "sisUrl", [ ria.templates.ModelPropertyBind ], String, "sisUserName", [ ria.templates.ModelPropertyBind ], String, "sisPassword", [ ria.templates.ModelPropertyBind ], String, "sisName" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.school.SchoolSisInfo";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).school = chlk.activities.school || {};
        (function() {
            chlk.activities.school.SchoolSisPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.TemplateBind(chlk.templates.school.SchoolSisInfo) ], "SchoolSisPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._qgg7licbkt6v42t9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="schools"><div class="action-bar not-transparent"><div class="container panel-bg">');
        districtId = self.getDistrictId() ? self.getDistrictId().valueOf() : null;
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Import schools");
            },
            attributes: {
                "class": "chlk-add" + " " + "opacity-button"
            },
            escaped: {}
        }, "schools", "import", districtId);
        buf.push("</div></div>");
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">School Name</div><div class="th">ID</div><div class="th">School Type</div><div class="th">URL</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items.getItems();
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getId().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolType()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolUrl()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "schools", "details", item.getId().valueOf());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "schools", "delete", [ item.getId().valueOf(), districtId.valueOf() ]);
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getId().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolType()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolUrl()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "schools", "details", item.getId().valueOf());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "schools", "delete", [ item.getId().valueOf(), districtId.valueOf() ]);
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "schools", "page", self.items, [ districtId ]);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.common.PaginatedList";
    "chlk.models.id.DistrictId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).school = chlk.models.school || {};
        (function() {
            "use strict";
            chlk.models.school.SchoolListViewData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolListViewData", [ chlk.models.common.PaginatedList, "items", chlk.models.id.DistrictId, "districtId", [ [ chlk.models.id.DistrictId, chlk.models.common.PaginatedList ] ], function $(districtId, items) {
                this.setItems(items);
                this.setDistrictId(districtId);
            } ]);
        })();
    })();
    "chlk.models.school.SchoolListViewData";
    "chlk.models.school.School";
    "chlk.models.district.District";
    "chlk.models.id.DistrictId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).school = chlk.templates.school || {};
        (function() {
            chlk.templates.school.Schools = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_qgg7licbkt6v42t9") ], [ ria.templates.ModelBind(chlk.models.school.SchoolListViewData) ], "Schools", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.common.PaginatedList, "items", [ ria.templates.ModelPropertyBind ], chlk.models.id.DistrictId, "districtId" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.school.Schools";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).school = chlk.activities.school || {};
        (function() {
            chlk.activities.school.SchoolsListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.school.Schools) ], "SchoolsListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._q769kjf819vn29 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="dialog add-school gray"><div class="x-window-header"><span>Import schools</span></div><div class="x-window-body"><div class="container panel-bg">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">sis school id</div><div class="th">sis school Year id</div><div class="th">Name</div><div class="th">email</div><div class="th">phone</div><div class="th">principal Email</div><div class="th">principal Name</div><div class="th">principal Title</div><div class="th">address</div><div class="th">city</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.schools;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.ActionForm_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            jade.globals.Hidden_mixin.call({
                                                buf: buf
                                            }, "sisSchoolId", item.getSisSchoolId());
                                            jade.globals.Hidden_mixin.call({
                                                buf: buf
                                            }, "districtId", self.getDistrictId().valueOf());
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getSisSchoolId()) ? "" : jade.interp) + '</div><div class="td">');
                                            jade.globals.Select_mixin.call({
                                                buf: buf,
                                                block: function() {
                                                    (function() {
                                                        var $$obj = item.getSchoolYears();
                                                        if ("number" == typeof $$obj.length) {
                                                            for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                                                var el = $$obj[$index];
                                                                jade.globals.Option_mixin.call({
                                                                    buf: buf
                                                                }, el.getCalendarId(), el.getName());
                                                            }
                                                        } else {
                                                            var $$l = 0;
                                                            for (var $index in $$obj) {
                                                                $$l++;
                                                                var el = $$obj[$index];
                                                                jade.globals.Option_mixin.call({
                                                                    buf: buf
                                                                }, el.getCalendarId(), el.getName());
                                                            }
                                                        }
                                                    }).call(this);
                                                }
                                            }, "sisSchoolYearId");
                                            buf.push('</div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getEmail()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPhone()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPrincipalEmail()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPrincipalTitle()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPrincipalName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getAddress()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCity()) ? "" : jade.interp) + '</div><div class="td">');
                                            jade.globals.Button_mixin.call({
                                                buf: buf,
                                                block: function() {
                                                    buf.push("Import");
                                                }
                                            });
                                            buf.push("</div>");
                                        },
                                        attributes: {
                                            "class": "row"
                                        },
                                        escaped: {}
                                    }, "schools", "importSchools");
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.ActionForm_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            jade.globals.Hidden_mixin.call({
                                                buf: buf
                                            }, "sisSchoolId", item.getSisSchoolId());
                                            jade.globals.Hidden_mixin.call({
                                                buf: buf
                                            }, "districtId", self.getDistrictId().valueOf());
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getSisSchoolId()) ? "" : jade.interp) + '</div><div class="td">');
                                            jade.globals.Select_mixin.call({
                                                buf: buf,
                                                block: function() {
                                                    (function() {
                                                        var $$obj = item.getSchoolYears();
                                                        if ("number" == typeof $$obj.length) {
                                                            for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                                                var el = $$obj[$index];
                                                                jade.globals.Option_mixin.call({
                                                                    buf: buf
                                                                }, el.getCalendarId(), el.getName());
                                                            }
                                                        } else {
                                                            var $$l = 0;
                                                            for (var $index in $$obj) {
                                                                $$l++;
                                                                var el = $$obj[$index];
                                                                jade.globals.Option_mixin.call({
                                                                    buf: buf
                                                                }, el.getCalendarId(), el.getName());
                                                            }
                                                        }
                                                    }).call(this);
                                                }
                                            }, "sisSchoolYearId");
                                            buf.push('</div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getEmail()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPhone()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPrincipalEmail()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPrincipalTitle()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPrincipalName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getAddress()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCity()) ? "" : jade.interp) + '</div><div class="td">');
                                            jade.globals.Button_mixin.call({
                                                buf: buf,
                                                block: function() {
                                                    buf.push("Import");
                                                }
                                            });
                                            buf.push("</div>");
                                        },
                                        attributes: {
                                            "class": "row"
                                        },
                                        escaped: {}
                                    }, "schools", "importSchools");
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        });
        buf.push("</div></div></div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.common.PaginatedList";
    (function() {
        (chlk = chlk || {}).templates = chlk.templates || {};
        (function() {
            chlk.templates.PaginatedList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_vfh3bbb5kqzuayvi") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "PaginatedList", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(Object), "items", [ ria.templates.ModelPropertyBind ], Number, "pageIndex", [ ria.templates.ModelPropertyBind ], Number, "pageSize", [ ria.templates.ModelPropertyBind ], Number, "totalCount", [ ria.templates.ModelPropertyBind ], Number, "totalPages", [ ria.templates.ModelPropertyBind ], Boolean, "hasNextPage", [ ria.templates.ModelPropertyBind ], Boolean, "hasPreviousPage" ]);
        })();
    })();
    "chlk.models.id.DistrictId";
    "chlk.models.import.School";
    "chlk.models.common.PaginatedList";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).import = chlk.models.import || {};
        (function() {
            "use strict";
            chlk.models.import.SchoolImportViewData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.import." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolImportViewData", [ chlk.models.id.DistrictId, "districtId", ria.__API.ArrayOf(chlk.models.import.School), "schools", [ [ chlk.models.id.DistrictId, ria.__API.ArrayOf(chlk.models.import.School) ] ], function $(districtId, schools) {
                this.setDistrictId(districtId);
                this.setSchools(schools);
            } ]);
        })();
    })();
    "chlk.templates.PaginatedList";
    "chlk.models.import.School";
    "chlk.models.import.SchoolImportViewData";
    "chlk.models.id.DistrictId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).school = chlk.templates.school || {};
        (function() {
            chlk.templates.school.ImportSchoolDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_q769kjf819vn29") ], [ ria.templates.ModelBind(chlk.models.import.SchoolImportViewData) ], "ImportSchoolDialog", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.DistrictId, "districtId", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.import.School), "schools" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.school.ImportSchoolDialog";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).school = chlk.activities.school || {};
        (function() {
            chlk.activities.school.ImportSchoolDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.school." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.ActivityGroup("AddSchool") ], [ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.TemplateBind(chlk.templates.school.ImportSchoolDialog) ], "ImportSchoolDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), []);
        })();
    })();
    "chlk.models.id.DistrictId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).import = chlk.models.import || {};
        (function() {
            "use strict";
            chlk.models.import.ImportTaskData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.import." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ImportTaskData", [ Number, "sisSchoolId", Number, "sisSchoolYearId", chlk.models.id.DistrictId, "districtId" ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.SchoolService";
    "chlk.services.AdminService";
    "chlk.services.AccountService";
    "chlk.services.GradeLevelService";
    "chlk.models.school.SchoolPeople";
    "chlk.models.district.District";
    "chlk.activities.school.SchoolDetailsPage";
    "chlk.activities.school.SchoolPeoplePage";
    "chlk.activities.school.ActionButtonsPopup";
    "chlk.models.school.SchoolPeople";
    "chlk.controls.ActionLinkControl";
    "chlk.activities.school.SchoolSisPage";
    "chlk.activities.school.SchoolsListPage";
    "chlk.activities.school.ImportSchoolDialog";
    "chlk.models.id.DistrictId";
    "chlk.models.id.SchoolId";
    "chlk.models.import.ImportTaskData";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.SchoolsController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SchoolsController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.SchoolService, "schoolService", [ ria.mvc.Inject ], chlk.services.AdminService, "adminService", [ ria.mvc.Inject ], chlk.services.AccountService, "accountService", [ ria.mvc.Inject ], chlk.services.GradeLevelService, "gradeLevelService", [ chlk.controllers.SidebarButton("schools") ], [ [ chlk.models.id.DistrictId, Number ] ], function pageAction(districtId_, start_) {
                var result = this.schoolService.getSchools(districtId_, start_ || 0).attach(this.validateResponse_()).then(function(data) {
                    return new ria.async.DeferredData(new chlk.models.school.SchoolListViewData(districtId, data));
                });
                return this.UpdateView(chlk.activities.school.SchoolsListPage, result);
            }, [ chlk.controllers.SidebarButton("schools") ], [ [ chlk.models.id.DistrictId, Number ] ], function listAction(districtId, pageIndex_) {
                var result = this.schoolService.getSchools(districtId, pageIndex_ | 0, false, false).attach(this.validateResponse_()).then(function(data) {
                    return new ria.async.DeferredData(new chlk.models.school.SchoolListViewData(districtId, data));
                });
                return this.PushView(chlk.activities.school.SchoolsListPage, result);
            }, [ [ chlk.models.id.DistrictId ] ], ria.__SYNTAX.Modifiers.VOID, function importAction(districtId) {
                var result = this.schoolService.getSchoolsForImport(districtId).attach(this.validateResponse_()).then(function(data) {
                    var schools = data.getItems();
                    return new ria.async.DeferredData(new chlk.models.import.SchoolImportViewData(districtId, schools));
                });
                return this.ShadeView(chlk.activities.school.ImportSchoolDialog, result);
            }, [ [ chlk.models.import.ImportTaskData ] ], ria.__SYNTAX.Modifiers.VOID, function importSchoolsAction(model) {
                var result = this.schoolService.runSchoolImport(model.getDistrictId(), model.getSisSchoolId(), model.getSisSchoolYearId()).attach(this.validateResponse_()).then(function(data) {
                    if (data) alert("Task was scheduled");
                });
            }, [ [ chlk.models.id.SchoolId ] ], function detailsAction(id) {
                var result = this.schoolService.getDetails(id).attach(this.validateResponse_());
                return this.PushView(chlk.activities.school.SchoolDetailsPage, result);
            }, [ [ chlk.models.id.SchoolId ] ], function sisInfoAction(id) {
                var result = this.schoolService.getSisInfo(id).attach(this.validateResponse_());
                return this.PushView(chlk.activities.school.SchoolSisPage, result);
            }, [ [ chlk.models.id.SchoolId ] ], function peopleAction(id) {
                var newGradeLevels = this.gradeLevelService.getGradeLevels().slice();
                var result = ria.async.wait([ this.schoolService.getPeopleSummary(id), this.adminService.getUsers(id, null, null, null, 0), this.accountService.getRoles() ]).then(function(result) {
                    var serializer = new ria.serialize.JsonSerializer();
                    var model = new chlk.models.school.SchoolPeople();
                    model.setSchoolInfo(result[0]);
                    var usersModel = new chlk.models.school.UsersList();
                    usersModel.setSelectedIndex(0);
                    usersModel.setByLastName(true);
                    usersModel.setUsers(result[1]);
                    model.setUsersPart(usersModel);
                    var roles = result[2];
                    newGradeLevels.unshift(serializer.deserialize({
                        name: "All Grades",
                        id: null
                    }, chlk.models.common.NameId));
                    roles.unshift(serializer.deserialize({
                        name: "All Roles",
                        id: null
                    }, chlk.models.common.NameId));
                    model.setGradeLevels(newGradeLevels);
                    model.setRoles(roles);
                    return model;
                });
                return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
            }, [ [ chlk.models.people.UsersList ] ], ria.__SYNTAX.Modifiers.VOID, function setPeopleFilterAction(model_) {
                var result = ria.async.wait([ this.adminService.getUsers(model_.getSchoolId(), model_.getRoleId() || null, model_.getGradeLevelId() || null, model_.isByLastName(), 0) ]).then(function(result) {
                    model_.setUsers(result[0]);
                    model_.setSelectedIndex(0);
                    return model_;
                });
                return this.UpdateView(chlk.activities.school.SchoolPeoplePage, result);
            }, [ [ chlk.models.id.SchoolId ] ], ria.__SYNTAX.Modifiers.VOID, function actionButtonsAction(id) {
                var result = ria.async.wait([ this.schoolService.getDetails(id) ]).then(function(result) {
                    var model = new chlk.models.school.ActionButtons();
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    model.setButtons(result[0].getButtons());
                    model.setEmails(result[0].getEmails());
                    return model;
                });
                return this.ShadeView(chlk.activities.school.ActionButtonsPopup, result);
            }, [ [ Object ] ], function actionLinkAction(form_) {
                if (confirm(form_.index + " " + form_.email)) this.context.getDefaultView().getCurrent().close();
            }, [ [ chlk.models.id.SchoolId, chlk.models.id.DistrictId ] ], ria.__SYNTAX.Modifiers.VOID, function deleteAction(id, districtId) {
                this.schoolService.del(id).then(function() {
                    this.ShowMsgBox("School will be deleted", "School delete task is created");
                    this.pageAction(districtId);
                }.bind(this));
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.district.District";
    "chlk.models.id.DistrictId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.DistrictService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DistrictService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getDistricts(pageIndex_) {
                return this.getPaginatedList("District/List.json", chlk.models.district.District, {
                    start: pageIndex_ | 0
                });
            }, [ [ String, String, String, String, String, Number ] ], ria.async.Future, function addDistrict(name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType) {
                return this.post("District/Create.json", chlk.models.district.District, {
                    name: name,
                    dbName: dbName,
                    sisUrl: sisUrl,
                    sisUserName: sisUserName,
                    sisPassword: sisPassword,
                    sisSystemType: sisSystemType
                });
            }, [ [ chlk.models.id.DistrictId, String, String, String, String, String, Number ] ], ria.async.Future, function updateDistrict(id, name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType) {
                return this.post("District/Update.json", chlk.models.district.District, {
                    districtId: id.valueOf(),
                    name: name,
                    dbName: dbName,
                    sisUrl: sisUrl,
                    sisUserName: sisUserName,
                    sisPassword: sisPassword,
                    sisSystemType: sisSystemType
                });
            }, [ [ chlk.models.id.DistrictId, String, String, String, String, String, Number ] ], ria.async.Future, function saveDistrict(id_, name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType) {
                if (id_ && id_.valueOf()) return this.updateDistrict(id_, name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType);
                return this.addDistrict(name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType);
            }, [ [ chlk.models.id.DistrictId ] ], ria.async.Future, function removeDistrict(id) {
                return this.post("District/Delete.json", chlk.models.district.District, {
                    districtId: id.valueOf()
                });
            }, [ [ chlk.models.id.DistrictId ] ], ria.async.Future, function getDistrict(id) {
                return this.post("District/Info.json", chlk.models.district.District, {
                    districtId: id.valueOf()
                });
            } ]);
        })();
    })();
    __ASSETS._vtvho1zexl7eqaor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="districts"><div class="action-bar not-transparent"><div class="container panel-bg">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Add District");
            },
            attributes: {
                "class": "chlk-add" + " " + "opacity-button"
            },
            escaped: {}
        }, "district", "add");
        buf.push("</div></div>");
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">Name</div><div class="th">DB Name</div><div class="th">SIS URL</div><div class="th">SIS User name</div><div class="th">SIS password</div><div class="th">SIS system type</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getDbName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisUrl()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisUserName()) ? "" : jade.interp) + '</div><div class="td">*</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisSystemType()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "schools", "list", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "view"
                                                },
                                                escaped: {}
                                            }, "district", "update", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "district", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getDbName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisUrl()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisUserName()) ? "" : jade.interp) + '</div><div class="td">*</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisSystemType()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "schools", "list", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "view"
                                                },
                                                escaped: {}
                                            }, "district", "update", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "district", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "district", "page", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.district.District";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).district = chlk.templates.district || {};
        (function() {
            chlk.templates.district.Districts = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.district." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_vtvho1zexl7eqaor") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "Districts", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.district.District), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.district.Districts";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).district = chlk.activities.district || {};
        (function() {
            chlk.activities.district.DistrictListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.district." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.district.Districts) ], "DistrictListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._zterdsozqcri19k9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="dialog add-district gray"><div class="x-window-header"><span>Add District</span></div><div class="x-window-body"><div class="general-info">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", self.id);
                buf.push('<div class="x-form-field"><label>Name:</label><input' + jade.attrs({
                    type: "text",
                    name: "name",
                    value: self.name
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>DB Name:</label><input' + jade.attrs({
                    type: "text",
                    name: "dbname",
                    value: self.dbName
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>SIS URL:</label><input' + jade.attrs({
                    type: "text",
                    name: "sisurl",
                    value: self.sisUrl
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>SIS Username:</label><input' + jade.attrs({
                    type: "text",
                    name: "sisusername",
                    value: self.sisUserName
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>SIS Password:</label><input' + jade.attrs({
                    type: "text",
                    name: "sispassword",
                    value: self.sisPassword
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>SIS System Type:</label><input' + jade.attrs({
                    type: "text",
                    name: "sissystemtype",
                    value: self.sisSystemType
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + "/></div>");
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Submit");
                    },
                    attributes: {
                        type: "submit",
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {
                        type: true
                    }
                });
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Cancel");
                    },
                    attributes: {
                        "class": "special-button" + " " + "red-button" + " " + "close"
                    },
                    escaped: {}
                });
            }
        }, "district", "save");
        buf.push("</div></div></div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.district.District";
    "chlk.models.id.DistrictId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).district = chlk.templates.district || {};
        (function() {
            chlk.templates.district.DistrictDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.district." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_zterdsozqcri19k9") ], [ ria.templates.ModelBind(chlk.models.district.District) ], "DistrictDialog", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.DistrictId, "id", [ ria.templates.ModelPropertyBind ], String, "name", [ ria.templates.ModelPropertyBind ], String, "sisUrl", [ ria.templates.ModelPropertyBind ], String, "dbName", [ ria.templates.ModelPropertyBind ], String, "sisUserName", [ ria.templates.ModelPropertyBind ], String, "sisPassword", [ ria.templates.ModelPropertyBind ], Number, "sisSystemType" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.district.DistrictDialog";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).district = chlk.activities.district || {};
        (function() {
            chlk.activities.district.DistrictDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.district." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.TemplateBind(chlk.templates.district.DistrictDialog) ], "DistrictDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.DistrictService";
    "chlk.models.district.District";
    "chlk.activities.district.DistrictListPage";
    "chlk.activities.district.DistrictDialog";
    "chlk.models.id.DistrictId";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.DistrictController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DistrictController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.DistrictService, "districtService", [ chlk.controllers.SidebarButton("districts") ], [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.districtService.getDistricts(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.district.DistrictListPage, result);
            }, [ [ Number ] ], function listSysAdminAction(pageIndex_) {
                var result = this.districtService.getDistricts(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.district.DistrictListPage, result);
            }, [ [ Number ] ], function pageAction(pageIndex) {
                var result = this.districtService.getDistricts(pageIndex).attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.district.DistrictListPage, result);
            }, [ [ chlk.models.id.DistrictId ] ], function updateAction(id) {
                var result = this.districtService.getDistrict(id).attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.district.DistrictDialog, result);
            }, function addAction() {
                var result = new ria.async.DeferredData(new chlk.models.district.District());
                return this.ShadeView(chlk.activities.district.DistrictDialog, result);
            }, [ [ chlk.models.district.District ] ], function saveAction(model) {
                var result = this.districtService.saveDistrict(model.getId(), model.getName(), model.getDbName(), model.getSisUrl(), model.getSisUserName(), model.getSisPassword(), model.getSisSystemType()).attach(this.validateResponse_()).then(function(data) {
                    this.view.getCurrent().close();
                    return this.districtService.getDistricts(0);
                }.bind(this));
                return this.UpdateView(chlk.activities.district.DistrictListPage, result);
            }, [ [ chlk.models.id.DistrictId ] ], function deleteAction(id) {
                var result = this.districtService.removeDistrict(id).attach(this.validateResponse_()).then(function(data) {
                    return this.districtService.getDistricts(0);
                }, this);
                return this.UpdateView(chlk.activities.district.DistrictListPage, result);
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "ria.async.Observable";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.UserInfoChangeEvent = ria.__API.delegate("chlk.services.UserInfoChangeEvent", null, [ String ], [ "string" ]);
            chlk.services.BaseInfoService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BaseInfoService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ ria.__SYNTAX.Modifiers.READONLY, ria.async.IObservable, "userInfoChange", function $() {
                BASE();
                this.userInfoChange = new ria.async.Observable(chlk.services.UserInfoChangeEvent);
            } ]);
        })();
    })();
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.SchoolId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).developer = chlk.models.developer || {};
        (function() {
            "use strict";
            chlk.models.developer.DeveloperInfo = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.developer." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DeveloperInfo", [ chlk.models.id.SchoolPersonId, "id", [ ria.serialize.SerializeProperty("displayname") ], String, "displayName", String, "email", [ ria.serialize.SerializeProperty("firstname") ], String, "firstName", [ ria.serialize.SerializeProperty("lastname") ], String, "lastName", String, "name", [ ria.serialize.SerializeProperty("schoolid") ], chlk.models.id.SchoolId, "schoolId", [ ria.serialize.SerializeProperty("websitelink") ], String, "webSite" ]);
        })();
    })();
    "chlk.services.BaseInfoService";
    "ria.async.Future";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.developer.DeveloperInfo";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.DeveloperService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DeveloperService", ria.__SYNTAX.EXTENDS(chlk.services.BaseInfoService), [ [ [ chlk.models.id.SchoolPersonId ] ], ria.async.Future, function getInfo(id) {
                return this.get("Developer/DeveloperInfo.json", chlk.models.developer.DeveloperInfo, {
                    developerId: id.valueOf()
                });
            }, [ [ chlk.models.id.SchoolPersonId, String, String, String, String ] ], ria.async.Future, function saveInfo(id, name, webSite, email) {
                return this.post("Developer/UpdateInfo.json", chlk.models.developer.DeveloperInfo, {
                    developerId: id.valueOf(),
                    name: name,
                    webSiteLink: webSite,
                    email: email
                }).then(function(result) {
                    this.userInfoChange.notify([ result.getName() ]);
                    return result;
                }, this);
            } ]);
        })();
    })();
    __ASSETS._bnn880zb7m5z5mi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="developer"><div class="info"><h2>Base Info</h2>');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                id = self.id ? self.id.valueOf() : null;
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", id);
                buf.push("<div><label>Developer name</label><input" + jade.attrs({
                    name: "name",
                    value: self.name
                }, {
                    name: true,
                    value: true
                }) + "/></div><div><label>Developer email</label><input" + jade.attrs({
                    name: "email",
                    value: self.email
                }, {
                    name: true,
                    value: true
                }) + "/></div><div><label>Developer web site</label><input" + jade.attrs({
                    name: "website",
                    value: self.webSite
                }, {
                    name: true,
                    value: true
                }) + '/></div><div class="align-right"><Button>Save</Button></div>');
            }
        }, "account", "profileSave");
        buf.push('<div class="bg-line"></div></div></div>');
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.developer.DeveloperInfo";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).profile = chlk.templates.profile || {};
        (function() {
            chlk.templates.profile.DeveloperProfile = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.profile." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_bnn880zb7m5z5mi") ], [ ria.templates.ModelBind(chlk.models.developer.DeveloperInfo) ], "DeveloperProfile", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolPersonId, "id", [ ria.templates.ModelPropertyBind ], String, "displayName", [ ria.templates.ModelPropertyBind ], String, "email", [ ria.templates.ModelPropertyBind ], String, "firstName", [ ria.templates.ModelPropertyBind ], String, "lastName", [ ria.templates.ModelPropertyBind ], String, "name", [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolId, "schoolId", [ ria.templates.ModelPropertyBind ], String, "webSite" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.profile.DeveloperProfile";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).profile = chlk.activities.profile || {};
        (function() {
            chlk.activities.profile.DeveloperPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.profile." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.TemplateBind(chlk.templates.profile.DeveloperProfile) ], "DeveloperPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._tzp1m4sxb6426gvi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="info-edit">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="title"><div class="name-block"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Old_Password) ? "" : jade.interp) + '</div><input name="oldPassword" type="password" class="validate[required]"/><div class="info-title">' + jade.escape(null == (jade.interp = Msg.New_Password) ? "" : jade.interp) + '</div><input name="newPassword" id="newPassword" type="password" class="validate[required]"/><div class="info-title">' + jade.escape(null == (jade.interp = Msg.New_Password_Confirm) ? "" : jade.interp) + '</div><input name="newPasswordConfirmation" id="newPasswordConfirmation" type="password" class="validate[required, equals[newPassword]]"/></div></div><br/><div class="section-buttons">');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        type: "submit",
                        id: "submit-info-button",
                        "class": "special-button" + " " + "blue-button"
                    },
                    escaped: {
                        type: true
                    }
                });
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        type: "button",
                        id: "cancell-edit-info-button",
                        "class": "special-button" + " " + "blue-button"
                    },
                    escaped: {
                        type: true
                    }
                });
                buf.push("</div>");
            },
            attributes: {
                id: "info-edit-form",
                "class": "with-ok"
            },
            escaped: {}
        }, "account", "doChangePassword");
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).profile = chlk.templates.profile || {};
        (function() {
            chlk.templates.profile.ChangePassword = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.profile." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_tzp1m4sxb6426gvi") ], [ ria.templates.ModelBind(ria.__API.Class) ], "ChangePassword", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), []);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.profile.ChangePassword";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).profile = chlk.activities.profile || {};
        (function() {
            chlk.activities.profile.ChangePasswordPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.profile." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.TemplateBind(chlk.templates.profile.ChangePassword) ], "ChangePasswordPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).account = chlk.models.account || {};
        (function() {
            "use strict";
            chlk.models.account.ChangePassword = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.account." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ChangePassword", [ String, "oldPassword", String, "newPassword", String, "newPasswordConfirmation" ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.DeveloperService";
    "chlk.services.AccountService";
    "chlk.activities.profile.DeveloperPage";
    "chlk.activities.profile.ChangePasswordPage";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.developer.DeveloperInfo";
    "chlk.models.account.ChangePassword";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.AccountController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AccountController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.DeveloperService, "developerService", [ ria.mvc.Inject ], chlk.services.AccountService, "accountService", [ [ chlk.models.account.ChangePassword ] ], function teacherChangePasswordAction(model) {
                this.accountService.changePassword(model.getOldPassword(), model.getNewPassword(), model.getNewPasswordConfirmation()).then(function(success) {
                    if (success) return this.redirect_("settings", "dashboardTeacher", []);
                }.bind(this));
            }, function logoutAction() {
                this.accountService.logOut().then(function(res) {
                    if (res.isSuccess()) {
                        location.href = this.getContext().getSession().get("webSiteRoot");
                    }
                }, this);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ chlk.controllers.SidebarButton("settings") ], [ [ chlk.models.id.SchoolPersonId ] ], function profileDeveloperAction(id) {
                var result = this.developerService.getInfo(id);
                return this.PushView(chlk.activities.profile.DeveloperPage, result);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ chlk.controllers.SidebarButton("settings") ], [ [ chlk.models.developer.DeveloperInfo ] ], function profileSaveDeveloperAction(model) {
                var result = this.developerService.saveInfo(model.getId(), model.getName(), model.getWebSite(), model.getEmail()).attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.profile.DeveloperPage, result);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AppId =             function wrapper() {
                var values = {};
                function AppId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AppIdImpl(value);
                }
                ria.__API.identifier(AppId, "chlk.models.id.AppId");
                function AppIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AppId#" + value + "]";
                    };
                }
                ria.__API.extend(AppIdImpl, AppId);
                return AppId;
            }();
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.PictureId =             function wrapper() {
                var values = {};
                function PictureId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new PictureIdImpl(value);
                }
                ria.__API.identifier(PictureId, "chlk.models.id.PictureId");
                function PictureIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.PictureId#" + value + "]";
                    };
                }
                ria.__API.extend(PictureIdImpl, PictureId);
                return PictureId;
            }();
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppPrice = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppPrice", [ Number, "price", [ ria.serialize.SerializeProperty("priceperclass") ], Number, "pricePerClass", [ ria.serialize.SerializeProperty("priceperschool") ], Number, "pricePerSchool", function $(price_, pricePerClass_, pricePerSchool_) {
                if (price_) this.setPrice(price_);
                if (pricePerClass_) {
                    this.setPricePerClass(pricePerClass_);
                }
                if (pricePerSchool_) {
                    this.setPricePerSchool(pricePerSchool_);
                }
            }, Object, function getPostData() {
                return {
                    price: this.getPrice(),
                    priceperclass: this.getPricePerClass(),
                    priceperschool: this.getPricePerSchool()
                };
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppAccess = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppAccess", [ [ ria.serialize.SerializeProperty("hasstudentmyapps") ], Boolean, "studentMyAppsEnabled", [ ria.serialize.SerializeProperty("hasteachermyapps") ], Boolean, "teacherMyAppsEnabled", [ ria.serialize.SerializeProperty("hasadminmyapps") ], Boolean, "adminMyAppsEnabled", [ ria.serialize.SerializeProperty("hasparentmyapps") ], Boolean, "parentMyAppsEnabled", [ ria.serialize.SerializeProperty("hasmypappsview") ], Boolean, "myAppsViewVisible", [ ria.serialize.SerializeProperty("canattach") ], Boolean, "attachEnabled", [ ria.serialize.SerializeProperty("showingradeview") ], Boolean, "visibleInGradingView", Object, function getPostData() {
                return {
                    hasstudentmyapps: this.isStudentMyAppsEnabled(),
                    hasteachermyapps: this.isTeacherMyAppsEnabled(),
                    hasadminmyapps: this.isAdminMyAppsEnabled(),
                    hasparentmyapps: this.isParentMyAppsEnabled(),
                    canattach: this.isAttachEnabled(),
                    showingradeview: this.isVisibleInGradingView()
                };
            }, [ [ Boolean, Boolean, Boolean, Boolean, Boolean, Boolean ] ], function $(hasStudentMyApps_, hasTeacherMyApps_, hasAdminMyApps_, hasParentMyApps_, canAttach_, showInGradeView_) {
                if (hasStudentMyApps_) this.setStudentMyAppsEnabled(hasStudentMyApps_);
                if (hasStudentMyApps_) this.setTeacherMyAppsEnabled(hasStudentMyApps_);
                if (hasAdminMyApps_) this.setAdminMyAppsEnabled(hasAdminMyApps_);
                if (hasParentMyApps_) this.setParentMyAppsEnabled(hasParentMyApps_);
                if (canAttach_) this.setAttachEnabled(canAttach_);
                if (showInGradeView_) this.setVisibleInGradingView(showInGradeView_);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AppPermissionId =             function wrapper() {
                var values = {};
                function AppPermissionId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AppPermissionIdImpl(value);
                }
                ria.__API.identifier(AppPermissionId, "chlk.models.id.AppPermissionId");
                function AppPermissionIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AppPermissionId#" + value + "]";
                    };
                }
                ria.__API.extend(AppPermissionIdImpl, AppPermissionId);
                return AppPermissionId;
            }();
        })();
    })();
    "ria.serialize.IDeserializable";
    "chlk.models.id.AppPermissionId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppPermission = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppPermission", ria.__SYNTAX.IMPLEMENTS(ria.serialize.IDeserializable), [ chlk.models.id.AppPermissionId, "id", Number, "type", String, "name", [ [ chlk.models.id.AppPermissionId, String, Number ] ], function $(id_, name_, type_) {
                if (id_) this.setId(id_);
                if (name_) this.setName(name_);
                if (type_) this.setType(type_);
            }, ria.__SYNTAX.Modifiers.VOID, function deserialize(raw) {
                this.setId(new chlk.models.id.AppPermissionId(raw.id));
                this.setType(Number(raw.type));
                this.setName(raw.name);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AppCategoryId =             function wrapper() {
                var values = {};
                function AppCategoryId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AppCategoryIdImpl(value);
                }
                ria.__API.identifier(AppCategoryId, "chlk.models.id.AppCategoryId");
                function AppCategoryIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AppCategoryId#" + value + "]";
                    };
                }
                ria.__API.extend(AppCategoryIdImpl, AppCategoryId);
                return AppCategoryId;
            }();
        })();
    })();
    "chlk.models.id.AppCategoryId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppCategory = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppCategory", [ chlk.models.id.AppCategoryId, "id", String, "name", String, "description" ]);
        })();
    })();
    "chlk.models.id.PictureId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppPicture = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppPicture", [ chlk.models.id.PictureId, "pictureId", String, "pictureUrl", String, "pictureClass", Number, "width", Number, "height", String, "title", Boolean, "editable", [ [ chlk.models.id.PictureId, String, Number, Number, String, Boolean ] ], function $(pictureId_, pictureUrl_, width_, height_, title_, editable_) {
                if (pictureId_) this.setPictureId(pictureId_);
                if (pictureUrl_) this.setPictureUrl(pictureUrl_);
                if (width_) this.setWidth(width_);
                if (height_) this.setHeight(height_);
                if (title_) this.setTitle(title_);
                var cls = "small-icon-item";
                if (this.getWidth() == 170) cls = "big-icon-item";
                if (this.getWidth() == 640) cls = "screeshot-item";
                this.setPictureClass(cls);
                if (editable_) this.setEditable(editable_);
            } ]);
        })();
    })();
    "ria.serialize.IDeserializable";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppStateEnum =             function wrapper() {
                var values = {};
                function AppStateEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(AppStateEnum, "chlk.models.apps.AppStateEnum");
                function AppStateEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.apps.AppStateEnum#" + value + "]";
                    };
                }
                ria.__API.extend(AppStateEnumImpl, AppStateEnum);
                values[1] = AppStateEnum.DRAFT = new AppStateEnumImpl(1);
                values[2] = AppStateEnum.SUBMIT_FOR_APPROVE = new AppStateEnumImpl(2);
                values[3] = AppStateEnum.APPROVED = new AppStateEnumImpl(3);
                values[4] = AppStateEnum.REJECTED = new AppStateEnumImpl(4);
                values[5] = AppStateEnum.LIVE = new AppStateEnumImpl(5);
                return AppStateEnum;
            }();
            chlk.models.apps.AppState = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppState", ria.__SYNTAX.IMPLEMENTS(ria.serialize.IDeserializable), [ chlk.models.apps.AppStateEnum, "stateId", function $() {
                this._states = {};
                this._states[chlk.models.apps.AppStateEnum.DRAFT] = "Not Submitted";
                this._states[chlk.models.apps.AppStateEnum.SUBMIT_FOR_APPROVE] = "Submitted for approve";
                this._states[chlk.models.apps.AppStateEnum.APPROVED] = "Approved";
                this._states[chlk.models.apps.AppStateEnum.REJECTED] = "Rejected";
                this._states[chlk.models.apps.AppStateEnum.LIVE] = "Live";
            }, [ [ Boolean ] ], String, function toString(hasLiveCopy_) {
                var prefix = "";
                var currentStateId = this.getStateId();
                if (hasLiveCopy_) {
                    if (currentStateId != chlk.models.apps.AppStateEnum.DRAFT && currentStateId != chlk.models.apps.AppStateEnum.LIVE) prefix = "Live - ";
                }
                return prefix + this._states[this.getStateId()];
            }, [ [ Number ] ], ria.__SYNTAX.Modifiers.VOID, function deserialize(raw) {
                this.setStateId(new chlk.models.apps.AppStateEnum(Number(raw)));
            } ]);
        })();
    })();
    "chlk.models.id.AppId";
    "chlk.models.id.PictureId";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.AppGradeLevelId";
    "chlk.models.apps.AppPrice";
    "chlk.models.apps.AppAccess";
    "chlk.models.apps.AppPermission";
    "chlk.models.apps.AppCategory";
    "chlk.models.apps.AppPicture";
    "chlk.models.apps.AppState";
    "chlk.models.common.NameId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.Application = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Application", [ chlk.models.id.AppId, "id", [ ria.serialize.SerializeProperty("isinternal") ], Boolean, "internal", String, "name", String, "url", [ ria.serialize.SerializeProperty("videodemourl") ], String, "videoDemoUrl", [ ria.serialize.SerializeProperty("shortdescription") ], String, "shortDescription", String, "description", [ ria.serialize.SerializeProperty("smallpictureid") ], chlk.models.id.PictureId, "smallPictureId", [ ria.serialize.SerializeProperty("bigpictureid") ], chlk.models.id.PictureId, "bigPictureId", chlk.models.apps.AppPicture, "iconPicture", chlk.models.apps.AppPicture, "bannerPicture", ria.__API.ArrayOf(chlk.models.apps.AppPicture), "screenshotPictures", [ ria.serialize.SerializeProperty("myappsurl") ], String, "myAppsUrl", [ ria.serialize.SerializeProperty("secretkey") ], String, "secretKey", chlk.models.apps.AppState, "state", [ ria.serialize.SerializeProperty("developerid") ], chlk.models.id.SchoolPersonId, "developerId", [ ria.serialize.SerializeProperty("liveappid") ], chlk.models.id.AppId, "liveAppId", [ ria.serialize.SerializeProperty("applicationprice") ], chlk.models.apps.AppPrice, "applicationPrice", [ ria.serialize.SerializeProperty("picturesid") ], ria.__API.ArrayOf(chlk.models.id.PictureId), "pictureIds", [ ria.serialize.SerializeProperty("applicationaccess") ], chlk.models.apps.AppAccess, "appAccess", ria.__API.ArrayOf(chlk.models.apps.AppPermission), "permissions", ria.__API.ArrayOf(chlk.models.apps.AppCategory), "categories", [ ria.serialize.SerializeProperty("gradelevels") ], ria.__API.ArrayOf(chlk.models.id.AppGradeLevelId), "gradeLevels" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.ShortAppInfo = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ShortAppInfo", [ String, "name", String, "url", [ ria.serialize.SerializeProperty("videodemourl") ], String, "videoDemoUrl", [ ria.serialize.SerializeProperty("shortdescription") ], String, "shortDescription", String, "description", [ ria.serialize.SerializeProperty("smallpictureid") ], chlk.models.id.PictureId, "smallPictureId", [ ria.serialize.SerializeProperty("bigpictureid") ], chlk.models.id.PictureId, "bigPictureId", [ [ String, String, String, String, String, chlk.models.id.PictureId, chlk.models.id.PictureId ] ], function $(name, url, videoDemoUrl, shortDescr, descr, smallPictureId_, bigPictureId_) {
                this.setName(name);
                this.setUrl(url);
                this.setVideoDemoUrl(videoDemoUrl);
                this.setShortDescription(shortDescr);
                this.setDescription(descr);
                if (smallPictureId_) this.setSmallPictureId(smallPictureId_);
                if (bigPictureId_) this.setBigPictureId(bigPictureId_);
            }, Object, function getPostData() {
                return {
                    name: this.getName(),
                    url: this.getUrl(),
                    videodemourl: this.getVideoDemoUrl(),
                    shortdescription: this.getShortDescription(),
                    description: this.getDescription(),
                    smallpictureid: this.getSmallPictureId() ? this.getSmallPictureId().valueOf() : null,
                    bigpictureid: this.getBigPictureId() ? this.getBigPictureId().valueOf() : null
                };
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "ria.async.Observable";
    "chlk.models.apps.Application";
    "chlk.models.apps.AppPermission";
    "chlk.models.apps.AppAccess";
    "chlk.models.apps.ShortAppInfo";
    "chlk.models.id.GradeLevelId";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.AppPermissionId";
    "chlk.models.id.PictureId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.DevApplicationListChangeEvent = ria.__API.delegate("chlk.services.DevApplicationListChangeEvent", null, [ ria.__API.ArrayOf(chlk.models.apps.Application) ], [ "list" ]);
            chlk.services.ApplicationService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ApplicationService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ ria.__SYNTAX.Modifiers.READONLY, ria.async.IObservable, "devApplicationListChange", function $() {
                BASE();
                this.devApplicationListChange = new ria.async.Observable(chlk.services.DevApplicationListChangeEvent);
            }, [ [ Number ] ], ria.async.Future, function getApps(pageIndex_) {
                return this.getPaginatedList("Application/List.json", chlk.models.apps.Application, {
                    start: pageIndex_
                });
            }, [ [ chlk.models.apps.Application ] ], function switchApp(app) {
                this.getContext().getSession().set("currentApp", app);
            }, function getCurrentApp() {
                return this.getContext().getSession().get("currentApp") || {};
            }, [ [ Boolean ] ], ria.async.Future, function getDevApps(refresh_) {
                var apps = this.getContext().getSession().get("dev-apps") || [];
                return apps.length == 0 || refresh_ ? this.getPaginatedList("Application/List.json", chlk.models.apps.Application, {}).then(function(data) {
                    this.getContext().getSession().set("dev-apps", data.getItems());
                    return data.getItems();
                }, this) : new ria.async.DeferredData(apps);
            }, Array, function arrayToIds(obj) {
                return obj ? obj.map(function(item) {
                    return item.valueOf();
                }) : [];
            }, String, function arrayToCsv(obj) {
                return obj ? obj.map(function(item) {
                    return item.valueOf();
                }).join(",") : "";
            }, [ [ chlk.models.id.AppId ] ], ria.async.Future, function getInfo(appId_) {
                return appId_ ? this.get("Application/GetInfo.json", chlk.models.apps.Application, {
                    applicationId: appId_.valueOf()
                }) : ria.async.DeferredData(this.getCurrentApp());
            }, [ [ Object, Number, Number ] ], ria.async.Future, function uploadPicture(file, width_, height_) {
                return this.uploadFiles("Application/UploadPicture", file, chlk.models.id.PictureId, {
                    width: width_,
                    height: height_
                });
            }, [ [ chlk.models.id.AppId ] ], ria.async.Future, function approveApp(appId) {
                return this.post("Application/Approve.json", Boolean, {
                    applicationId: appId.valueOf()
                });
            }, [ [ chlk.models.id.AppId ] ], ria.async.Future, function goLive(appId) {
                return this.post("Application/GoLive.json", Boolean, {
                    applicationId: appId.valueOf()
                });
            }, [ [ chlk.models.id.AppId ] ], ria.async.Future, function unlist(appId) {
                return this.post("Application/Unlist.json", Boolean, {
                    applicationId: appId.valueOf()
                });
            }, [ [ chlk.models.id.AppId ] ], ria.async.Future, function declineApp(appId) {
                return this.post("Application/Decline.json", Boolean, {
                    applicationId: appId.valueOf()
                });
            }, [ [ chlk.models.id.SchoolPersonId, String ] ], ria.async.Future, function createApp(devId, name) {
                return this.post("Application/Create.json", chlk.models.apps.Application, {
                    developerId: devId.valueOf(),
                    name: name
                }).then(function(data) {
                    return this.getDevApps(true).then(function(items) {
                        this.devApplicationListChange.notify([ items ]);
                        this.switchApp(data);
                        return data;
                    }, this);
                }, this);
            }, [ [ chlk.models.id.AppId ] ], ria.async.Future, function deleteApp(appId) {
                return this.post("Application/Delete.json", Boolean, {
                    applicationId: appId.valueOf()
                }).then(function(data) {
                    return this.getDevApps(true).then(function(items) {
                        this.devApplicationListChange.notify([ items ]);
                        this.switchApp(items.length > 0 ? items[items.length - 1] : new chlk.models.apps.Application());
                        return data;
                    }, this);
                }, this);
            }, [ [ chlk.models.id.AppId, chlk.models.apps.ShortAppInfo, ria.__API.ArrayOf(chlk.models.id.AppPermissionId), chlk.models.apps.AppPrice, chlk.models.id.SchoolPersonId, chlk.models.apps.AppAccess, ria.__API.ArrayOf(chlk.models.id.AppCategoryId), ria.__API.ArrayOf(chlk.models.id.PictureId), ria.__API.ArrayOf(chlk.models.id.GradeLevelId), Boolean ] ], ria.async.Future, function updateApp(appId, shortAppInfo, permissionIds, appPricesInfo, devId, appAccess, categories, pictures_, gradeLevels, forSubmit) {
                return this.post("Application/Update.json", chlk.models.apps.Application, {
                    applicationId: appId.valueOf(),
                    shortApplicationInfo: shortAppInfo.getPostData(),
                    permissions: this.arrayToIds(permissionIds),
                    applicationPrices: appPricesInfo.getPostData(),
                    developerId: devId.valueOf(),
                    applicationAccessInfo: appAccess.getPostData(),
                    categories: this.arrayToIds(categories),
                    gradeLevels: this.arrayToIds(gradeLevels),
                    forSubmit: forSubmit
                });
            }, ria.__API.ArrayOf(chlk.models.apps.AppPermission), function getAppPermissions() {
                return [ new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(0), "User"), new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(1), "Message"), new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(2), "Grade"), new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(3), "Attendance"), new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(4), "Announcement"), new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(5), "Class"), new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(6), "Schedule"), new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(7), "Discipline") ];
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.apps.AppCategory";
    "chlk.models.id.AppCategoryId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.AppCategoryService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppCategoryService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getCategories(pageIndex_) {
                return this.getPaginatedList("Category/List.json", chlk.models.apps.AppCategory, {
                    start: pageIndex_ | 0
                });
            }, [ [ String, String ] ], ria.async.Future, function addCategory(name, description) {
                return this.post("Category/Add.json", chlk.models.apps.AppCategory, {
                    name: name,
                    description: description
                });
            }, [ [ chlk.models.id.AppCategoryId, String, String ] ], ria.async.Future, function updateCategory(id, name, description) {
                return this.post("Category/Update.json", chlk.models.apps.AppCategory, {
                    categoryId: id.valueOf(),
                    name: name,
                    description: description
                });
            }, [ [ chlk.models.id.AppCategoryId, String, String ] ], ria.async.Future, function saveCategory(id_, name, description) {
                if (id_ && id_.valueOf()) return this.updateCategory(id_, name, description);
                return this.addCategory(name, description);
            }, [ [ chlk.models.id.AppCategoryId ] ], ria.async.Future, function removeCategory(id) {
                return this.post("Category/Delete.json", chlk.models.apps.AppCategory, {
                    categoryId: id.valueOf()
                });
            }, [ [ chlk.models.id.AppCategoryId ] ], ria.async.Future, function getCategory(id) {
                return this.post("Category/GetInfo.json", chlk.models.apps.AppCategory, {
                    categoryId: id.valueOf()
                });
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "chlk.models.id.PictureId";
    "ria.async.Future";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.PictureService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("PictureService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.PictureId, Number, Number ] ], String, function getPictureUrl(id, width_, height_) {
                var prefix = this.getContext().getSession().get("azurePictureUrl");
                var suffix = "";
                if (width_) {
                    suffix = height_ ? "-" + width_ + "x" + height_ : "-" + width_ + "x" + width_;
                }
                return id.valueOf() ? prefix + id.valueOf() + suffix : "#";
            } ]);
        })();
    })();
    __ASSETS._xr6mwcruvscbx1or = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.AppManagementControls_mixin = function(id, state) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            if (state.getStateId() != chlk.models.apps.AppStateEnum.SUBMIT_FOR_APPROVE) {
                buf.push("<span>" + jade.escape(null == (jade.interp = state.toString()) ? "" : jade.interp) + "</span>");
            } else {
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Approve");
                    },
                    attributes: {
                        "class": "btn"
                    },
                    escaped: {}
                }, "apps", "approve", id);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Decline");
                    },
                    attributes: {
                        "class": "btn"
                    },
                    escaped: {}
                }, "apps", "decline", id);
            }
        };
        buf.push('<div class="apps-list">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th"></div><div class="th">Application Name</div><div class="th">Description</div><div class="th">State</div><div class="th narrow">isInternal</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            var pictureLink = item.getSmallPictureId() ? self.getPictureURL(item.getSmallPictureId(), 74) : "#";
                                            buf.push('<div class="td"><img' + jade.attrs({
                                                src: pictureLink,
                                                "class": "app-icon"
                                            }, {
                                                src: true
                                            }) + '/></div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getShortDescription()) ? "" : jade.interp) + '</div><div class="td app-state">');
                                            jade.globals.AppManagementControls_mixin.call({
                                                buf: buf
                                            }, item.getId(), item.getState());
                                            buf.push('</div><div class="td">');
                                            jade.globals.Checkbox_mixin.call({
                                                buf: buf
                                            }, "isInternal", item.isInternal());
                                            buf.push('</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "apps", "details", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "apps", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            var pictureLink = item.getSmallPictureId() ? self.getPictureURL(item.getSmallPictureId(), 74) : "#";
                                            buf.push('<div class="td"><img' + jade.attrs({
                                                src: pictureLink,
                                                "class": "app-icon"
                                            }, {
                                                src: true
                                            }) + '/></div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getShortDescription()) ? "" : jade.interp) + '</div><div class="td app-state">');
                                            jade.globals.AppManagementControls_mixin.call({
                                                buf: buf
                                            }, item.getId(), item.getState());
                                            buf.push('</div><div class="td">');
                                            jade.globals.Checkbox_mixin.call({
                                                buf: buf
                                            }, "isInternal", item.isInternal());
                                            buf.push('</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "apps", "details", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "apps", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "apps", "page", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.apps.Application";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).apps = chlk.templates.apps || {};
        (function() {
            chlk.templates.apps.Apps = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_xr6mwcruvscbx1or") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "Apps", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.apps.Application), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.apps.Apps";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).apps = chlk.activities.apps || {};
        (function() {
            chlk.activities.apps.AppsListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.apps.Apps) ], "AppsListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._ddkoury7x7xpqfr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Textbox_mixin = function(tName, val, isReadonly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            if (isReadonly) {
                attributes.disabled = isReadonly;
            }
            buf.push("<input" + jade.attrs(jade.merge({
                type: "text",
                name: tName,
                value: val,
                "class": "validate[required]" + " " + "text"
            }, attributes), jade.merge({
                type: true,
                name: true,
                value: true
            }, escaped, true)) + "/>");
        };
        jade.globals.TextArea_mixin = function(tName, val, isReadonly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            if (isReadonly) {
                attributes.disabled = isReadonly;
            }
            buf.push("<textarea" + jade.attrs(jade.merge({
                name: tName,
                "class": "validate[required]"
            }, attributes), jade.merge({
                name: true,
                "class": true
            }, escaped, true)) + ">" + jade.escape(null == (jade.interp = val) ? "" : jade.interp) + "</textarea>");
        };
        jade.globals.MyAppsPermission_mixin = function(prefix, app, suffix) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push('<div class="myapps-permission"><label>' + jade.escape(null == (jade.interp = prefix) ? "" : jade.interp) + '</label><span class="app-name">' + jade.escape(null == (jade.interp = app + " " + suffix) ? "" : jade.interp) + "</span></div>");
        };
        jade.globals.AppPricing_mixin = function(appPrice, isReadOnly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            var isFreeApp = (appPrice.getPrice() || appPrice.getPricePerSchool() || appPrice.getPricePerClass() || 0) == 0;
            var isSchoolFlatRateEnabled = appPrice.getPricePerSchool() > 0;
            var isClassFlatRateEnabled = appPrice.getPricePerClass() > 0;
            buf.push('<div class="app-pricing"><div class="elem"><label class="hint">Pricing:</label>');
            jade.globals.SlideCheckbox_mixin.call({
                buf: buf,
                attributes: {
                    "class": "price-checkbox"
                },
                escaped: {}
            }, "free", isFreeApp, isReadOnly);
            buf.push("</div><div" + jade.attrs({
                "class": "prices" + " " + (isFreeApp ? "x-hidden" : undefined)
            }, {
                "class": true
            }) + '><div class="elem cost-per-user"><label class="hint">Cost per user:</label>');
            jade.globals.Textbox_mixin.call({
                buf: buf,
                attributes: {
                    "class": "price-box"
                },
                escaped: {}
            }, "costPerUser", appPrice.getPrice());
            buf.push('</div><div class="elem"><label class="wide hint">School flat rate:</label><div class="price-elem"><span>Is there a flat rate for whole school purchases?</span>');
            jade.globals.SlideCheckbox_mixin.call({
                buf: buf
            }, "schoolFlatRateEnabled", isSchoolFlatRateEnabled, isReadOnly);
            buf.push("</div></div><div" + jade.attrs({
                "class": "elem" + " " + "school-flat-rate" + " " + (!isSchoolFlatRateEnabled ? "x-hidden" : undefined)
            }, {
                "class": true
            }) + '><label class="hint">School cost:</label>');
            jade.globals.Textbox_mixin.call({
                buf: buf,
                attributes: {
                    "class": "price-box"
                },
                escaped: {}
            }, "costPerSchool", appPrice.getPricePerSchool());
            buf.push('</div><div class="elem"><label class="wide hint">Class flat rate:</label><div class="price-elem"><span>Is there a flat rate for whole class purchases?</span>');
            jade.globals.SlideCheckbox_mixin.call({
                buf: buf
            }, "classFlatRateEnabled", isClassFlatRateEnabled, isReadOnly);
            buf.push("</div></div><div" + jade.attrs({
                "class": "elem" + " " + "class-flat-rate" + " " + (!isClassFlatRateEnabled ? "x-hidden" : undefined)
            }, {
                "class": true
            }) + '><label class="hint">Class cost:</label>');
            jade.globals.Textbox_mixin.call({
                buf: buf,
                attributes: {
                    "class": "price-box"
                },
                escaped: {}
            }, "costPerClass", appPrice.getPricePerClass());
            buf.push("</div></div></div>");
        };
        var app = self.app;
        var myAppsAccess = app.getAppAccess();
        var appPrice = app.getApplicationPrice();
        var selectedAppCategories = {};
        var selectedAppGradeLevels = {};
        var selectedAppPermissions = {};
        var appGradeLvls = [];
        var appCategories = [];
        var appPermissions = [];
        var actAppCategories = app.getCategories() || [];
        var actAppGradeLevels = app.getGradeLevels() || [];
        var actAppPermissions = app.getPermissions() || [];
        (function() {
            var $$obj = actAppGradeLevels;
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var item = $$obj[$index];
                    selectedAppGradeLevels[item.valueOf()] = true;
                    appGradeLvls.push(item.valueOf());
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var item = $$obj[$index];
                    selectedAppGradeLevels[item.valueOf()] = true;
                    appGradeLvls.push(item.valueOf());
                }
            }
        }).call(this);
        (function() {
            var $$obj = actAppCategories;
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var item = $$obj[$index];
                    selectedAppCategories[item.getId()] = true;
                    appCategories.push(item.getId().valueOf());
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var item = $$obj[$index];
                    selectedAppCategories[item.getId()] = true;
                    appCategories.push(item.getId().valueOf());
                }
            }
        }).call(this);
        (function() {
            var $$obj = actAppPermissions;
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var item = $$obj[$index];
                    selectedAppPermissions[item.getType()] = true;
                    appPermissions.push(item.getType());
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var item = $$obj[$index];
                    selectedAppPermissions[item.getType()] = true;
                    appPermissions.push(item.getType());
                }
            }
        }).call(this);
        var appGradeLvls = appGradeLvls.join(",");
        var appCategories = appCategories.join(",");
        var appPermissions = appPermissions.join(",");
        buf.push('<div class="app-info">');
        if (!self.isReadOnly()) {
            buf.push('<div class="action-bar not-transparent dev-app-info"><div class="container panel-bg"><label>API key:</label>');
            jade.globals.Textbox_mixin.call({
                buf: buf,
                attributes: {
                    "class": "api-key"
                },
                escaped: {}
            }, "apikey", app.getSecretKey());
            buf.push("<label>" + jade.escape(null == (jade.interp = "Status: " + app.getState().toString()) ? "" : jade.interp) + "</label></div></div>");
        }
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                if (!self.isReadOnly()) {
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push(" Submit App");
                        },
                        attributes: {
                            disabled: self.isDraft(),
                            name: "submit-btn",
                            "class": (self.isDraft() ? "x-item-disabled" : "") + " " + "blue-tb-right-button" + " " + "submit-btn"
                        },
                        escaped: {
                            disabled: true,
                            name: true
                        }
                    });
                }
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "draft", self.isDraft());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", app.getId());
                buf.push('<div class="section first"><div class="elem"><label class="hint">App Name:</label>');
                jade.globals.Textbox_mixin.call({
                    buf: buf
                }, "name", app.getName(), self.isReadOnly());
                buf.push('</div><div class="elem"><label class="hint">App URL:</label>');
                jade.globals.Textbox_mixin.call({
                    buf: buf
                }, "url", app.getUrl(), self.isReadOnly());
                buf.push('</div></div><div class="section"><div class="elem"><label class="top hint">My Apps:</label><div class="my-apps"><div class="permission">');
                jade.globals.MyAppsPermission_mixin.call({
                    buf: buf
                }, "Can Teachers Launch", app.getName(), "in My Apps?");
                jade.globals.SlideCheckbox_mixin.call({
                    buf: buf
                }, "hasTeacherMyApps", myAppsAccess.isTeacherMyAppsEnabled(), self.isReadOnly());
                buf.push('</div><div class="permission">');
                jade.globals.MyAppsPermission_mixin.call({
                    buf: buf
                }, "Can Students Launch", app.getName(), "in My Apps?");
                jade.globals.SlideCheckbox_mixin.call({
                    buf: buf
                }, "hasStudentMyApps", myAppsAccess.isStudentMyAppsEnabled(), self.isReadOnly());
                buf.push('</div><div class="permission">');
                jade.globals.MyAppsPermission_mixin.call({
                    buf: buf
                }, "Can Admins Launch", app.getName(), "in My Apps?");
                jade.globals.SlideCheckbox_mixin.call({
                    buf: buf
                }, "hasAdminMyApps", myAppsAccess.isAdminMyAppsEnabled(), self.isReadOnly());
                buf.push('</div><div class="permission">');
                jade.globals.MyAppsPermission_mixin.call({
                    buf: buf
                }, "Can Parents Launch", app.getName(), "in My Apps?");
                jade.globals.SlideCheckbox_mixin.call({
                    buf: buf
                }, "hasParentMyApps", myAppsAccess.isParentMyAppsEnabled(), self.isReadOnly());
                buf.push('</div></div></div></div><div class="section small"><div class="elem first"><label class="hint">New Item:</label><div class="my-apps"><div class="permission">');
                jade.globals.MyAppsPermission_mixin.call({
                    buf: buf
                }, "Can Teachers Attach", app.getName(), "in New Items?");
                jade.globals.SlideCheckbox_mixin.call({
                    buf: buf
                }, "canAttach", myAppsAccess.isAttachEnabled(), self.isReadOnly());
                buf.push('</div></div></div><div class="elem"><label class="hint">Grading View:</label><div class="my-apps"><div class="permission">');
                jade.globals.MyAppsPermission_mixin.call({
                    buf: buf
                }, "Will be Teachers", "", "viewing student output");
                jade.globals.SlideCheckbox_mixin.call({
                    buf: buf
                }, "showInGradingView", myAppsAccess.isVisibleInGradingView(), self.isReadOnly());
                buf.push('</div></div></div></div><div class="section"><div class="elem"><label class="hint">API Access:</label><div class="permissions">');
                jade.globals.CheckboxList_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.getPermissions();
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    isChecked = !!selectedAppPermissions[item.getId().valueOf()];
                                    jade.globals.LabeledCheckbox_mixin.call({
                                        buf: buf
                                    }, item.getName(), "app-permission" + item.getId(), isChecked, self.isReadOnly());
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    isChecked = !!selectedAppPermissions[item.getId().valueOf()];
                                    jade.globals.LabeledCheckbox_mixin.call({
                                        buf: buf
                                    }, item.getName(), "app-permission" + item.getId(), isChecked, self.isReadOnly());
                                }
                            }
                        }).call(this);
                    }
                }, "permissions", "app-permission", appPermissions);
                buf.push('</div></div></div><div class="section"><div class="elem"><label class="hint">Subjects:</label><div class="categories">');
                jade.globals.CheckboxList_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.getCategories();
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    isChecked = !!selectedAppCategories[item.getId()];
                                    jade.globals.LabeledCheckbox_mixin.call({
                                        buf: buf
                                    }, item.getName(), "app-category" + item.getId(), isChecked, self.isReadOnly());
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    isChecked = !!selectedAppCategories[item.getId()];
                                    jade.globals.LabeledCheckbox_mixin.call({
                                        buf: buf
                                    }, item.getName(), "app-category" + item.getId(), isChecked, self.isReadOnly());
                                }
                            }
                        }).call(this);
                    }
                }, "categories", "app-category", appCategories);
                buf.push('</div></div></div><div class="section"><div class="elem"><label class="hint">Grades:</label><div class="grade-levels">');
                jade.globals.CheckboxList_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.getGradeLevels();
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    isChecked = !!selectedAppGradeLevels[item.getId().valueOf()];
                                    jade.globals.LabeledCheckbox_mixin.call({
                                        buf: buf
                                    }, item.getName(), "app-grade-level" + item.getId(), isChecked, self.isReadOnly());
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    isChecked = !!selectedAppGradeLevels[item.getId().valueOf()];
                                    jade.globals.LabeledCheckbox_mixin.call({
                                        buf: buf
                                    }, item.getName(), "app-grade-level" + item.getId(), isChecked, self.isReadOnly());
                                }
                            }
                        }).call(this);
                    }
                }, "gradeLevels", "app-grade-level", appGradeLvls);
                buf.push('</div></div></div><div class="section"><div class="elem"><label class="wide hint">Short Description:</label>');
                jade.globals.Textbox_mixin.call({
                    buf: buf
                }, "shortDescription", app.getShortDescription(), self.isReadOnly());
                buf.push('</div><div class="elem"><label class="wide hint top-align">Long Description:</label>');
                jade.globals.TextArea_mixin.call({
                    buf: buf
                }, "longDescription", app.getDescription(), self.isReadOnly());
                buf.push('</div><div class="elem"><label class="wide hint">Video Demo:</label>');
                jade.globals.Textbox_mixin.call({
                    buf: buf
                }, "videoDemoUrl", app.getVideoDemoUrl(), self.isReadOnly());
                buf.push('</div></div><div class="section">');
                jade.globals.AppPricing_mixin.call({
                    buf: buf
                }, appPrice, self.isReadOnly());
                buf.push('</div><div class="section last"><div class="pictures"><div class="elem icons"><label class="hint">Icons:</label><div class="icons"><div class="icon">');
                jade.globals.RenderWith_mixin.call({
                    buf: buf
                }, app.getIconPicture(), chlk.templates.apps.AppPicture);
                buf.push('</div><div class="banner">');
                jade.globals.RenderWith_mixin.call({
                    buf: buf
                }, app.getBannerPicture(), chlk.templates.apps.AppPicture);
                buf.push('</div></div></div><div class="elem screenshots"><label class="hint">Screenshots:</label><div class="screenshots">');
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "appScreenShotIds", "");
                buf.push("<!--+RenderWith(app.(), chlk.templates.apps.AppPicture)--></div></div></div></div>");
                if (!self.isReadOnly()) {
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push("Update Draft");
                        },
                        attributes: {
                            disabled: !self.isDraft(),
                            name: "submit-draft-btn",
                            "class": (!self.isDraft() ? "x-item-disabled" : "") + " " + "submit-draft-btn" + " " + "special-button" + " " + "blue-button"
                        },
                        escaped: {
                            disabled: true,
                            name: true
                        }
                    });
                }
            },
            attributes: {
                "class": "app-info"
            },
            escaped: {}
        }, "apps", "update");
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.apps.Application";
    "chlk.models.apps.AppCategory";
    "chlk.models.apps.AppPermission";
    "chlk.models.apps.AppGradeLevel";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppInfoViewData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppInfoViewData", [ chlk.models.apps.Application, "app", ria.__API.ArrayOf(chlk.models.apps.AppCategory), "categories", ria.__API.ArrayOf(chlk.models.apps.AppGradeLevel), "gradeLevels", ria.__API.ArrayOf(chlk.models.apps.AppPermission), "permissions", Boolean, "empty", Boolean, "readOnly", Boolean, "draft", [ [ chlk.models.apps.Application, Boolean, ria.__API.ArrayOf(chlk.models.apps.AppCategory), ria.__API.ArrayOf(chlk.models.apps.AppGradeLevel), ria.__API.ArrayOf(chlk.models.apps.AppPermission), Boolean ] ], function $(app_, isReadonly, categories, gradeLevels, permissions, isDraft) {
                if (app_) this.setApp(app_);
                this.setEmpty(!!app_);
                this.setReadOnly(isReadonly);
                this.setCategories(categories);
                this.setGradeLevels(gradeLevels);
                this.setPermissions(permissions);
                this.setDraft(isDraft);
            } ]);
        })();
    })();
    "chlk.models.apps.Application";
    "chlk.models.apps.AppInfoViewData";
    "chlk.models.common.NameId";
    "chlk.models.id.AppGradeLevelId";
    "chlk.models.apps.AppGradeLevel";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).apps = chlk.templates.apps || {};
        (function() {
            chlk.templates.apps.AppInfo = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_ddkoury7x7xpqfr") ], [ ria.templates.ModelBind(chlk.models.apps.AppInfoViewData) ], "AppInfo", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.apps.Application, "app", [ ria.templates.ModelPropertyBind ], Boolean, "empty", [ ria.templates.ModelPropertyBind ], Boolean, "draft", [ ria.templates.ModelPropertyBind ], Boolean, "readOnly", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.apps.AppCategory), "categories", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.apps.AppGradeLevel), "gradeLevels", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.apps.AppPermission), "permissions" ]);
        })();
    })();
    __ASSETS._0urldvo7dvbtvs4i = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.Thumbnail_mixin = function(id, url, title, dimsX, dimsY, pictureCls, readOnly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push("<div" + jade.attrs({
                "class": "thumbnail" + " " + pictureCls
            }, {
                "class": true
            }) + ">");
            if (!readOnly) {
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    attributes: {
                        "class": "close-btn"
                    },
                    escaped: {}
                }, "apps", "deletePicture", [ title, dimsX, dimsY ]);
                jade.globals.LoadingImage_mixin.call({
                    buf: buf,
                    attributes: {
                        src: url
                    },
                    escaped: {
                        src: true
                    }
                });
            }
            buf.push("</div>");
        };
        jade.globals.IconBox_mixin = function(id, url, title, dimsX, dimsY, pictureCls, readOnly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            jade.globals.Hidden_mixin.call({
                buf: buf
            }, "app" + title + "Id", id.valueOf());
            if (id.valueOf()) {
                jade.globals.Thumbnail_mixin.call({
                    buf: buf
                }, id, url, title, dimsX, dimsY, pictureCls, readOnly);
            } else {
                buf.push('<div class="upload-img-block"><div class="top">' + jade.escape(null == (jade.interp = dimsX + "px x " + dimsY + "px") ? "" : jade.interp) + '</div><div class="bottom"><button class="simple-gray-button file">' + jade.escape(null == (jade.interp = title) ? "" : jade.interp));
                jade.globals.FileUpload_mixin.call({
                    buf: buf,
                    attributes: {
                        "class": "simple-gray-button"
                    },
                    escaped: {}
                }, "apps", "uploadPicture", [ dimsX, dimsY, title ]);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Download Attachment");
                    }
                }, "apps", "d", id);
                buf.push("</button></div></div>");
            }
        };
        jade.globals.IconBox_mixin.call({
            buf: buf
        }, self.pictureId, self.pictureUrl, self.title, self.width, self.height, self.pictureClass, !self.editable);
        return buf.join("");
    };
    "chlk.models.apps.AppPicture";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).apps = chlk.templates.apps || {};
        (function() {
            chlk.templates.apps.AppPicture = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_0urldvo7dvbtvs4i") ], [ ria.templates.ModelBind(chlk.models.apps.AppPicture) ], "AppPicture", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.PictureId, "pictureId", [ ria.templates.ModelPropertyBind ], String, "pictureUrl", [ ria.templates.ModelPropertyBind ], Number, "width", [ ria.templates.ModelPropertyBind ], Number, "height", [ ria.templates.ModelPropertyBind ], String, "title", [ ria.templates.ModelPropertyBind ], Boolean, "editable", [ ria.templates.ModelPropertyBind ], String, "pictureClass" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.apps.AppInfo";
    "chlk.templates.apps.AppPicture";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).apps = chlk.activities.apps || {};
        (function() {
            var HIDDEN_CLASS = "x-hidden";
            var DISABLED_CLASS = "x-item-disabled";
            chlk.activities.apps.AppInfoPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.apps.AppInfo) ], [ ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, "icon", ".icon", ria.mvc.PartialUpdateRuleActions.Replace) ], [ ria.mvc.PartialUpdateRule(chlk.templates.apps.AppPicture, "banner", ".banner", ria.mvc.PartialUpdateRuleActions.Replace) ], "AppInfoPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ function $() {
                BASE();
            }, [ ria.mvc.DomEventBind("click", "input.price-checkbox") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function toggleAppPaymentInfo(node, event) {
                var appPricing = this.dom.find(".prices");
                appPricing.toggleClass(HIDDEN_CLASS, node.checked());
            }, [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function onFormChange(node, event) {
                var submitBtnWrapper = this.dom.find(".submit-btn");
                submitBtnWrapper.addClass(DISABLED_CLASS);
                var submitBtn = submitBtnWrapper.find("button");
                submitBtn.setAttr("disabled", true);
                var isDraftHidden = this.dom.find("input[name=draft]");
                isDraftHidden.setValue("true");
                var updateDraftBtnWrapper = this.dom.find(".submit-draft-btn");
                updateDraftBtnWrapper.removeClass(DISABLED_CLASS);
                updateDraftBtnWrapper.removeClass("disabled");
                updateDraftBtnWrapper.removeAttr("disabled");
                var updateDraftBtn = updateDraftBtnWrapper.find("button");
                updateDraftBtn.removeAttr("disabled");
            }, [ ria.mvc.DomEventBind("click", ".close-btn") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function picturesChanged(node, event) {
                this.onFormChange(node, event);
            }, [ ria.mvc.DomEventBind("input", "input") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function inputsChanged(node, event) {
                this.onFormChange(node, event);
            }, [ ria.mvc.DomEventBind("change", "input[type=checkbox], input[type=file]") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function checkboxesChanged(node, event) {
                this.onFormChange(node, event);
            }, [ ria.mvc.DomEventBind("click", "input[name=schoolFlatRateEnabled]") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function toggleSchoolFlatRate(node, event) {
                var schoolFlatRate = this.dom.find(".school-flat-rate");
                schoolFlatRate.toggleClass(HIDDEN_CLASS, !node.checked());
            }, [ ria.mvc.DomEventBind("click", "input[name=classFlatRateEnabled]") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function toggleClassFlatRate(node, event) {
                var classFlatRate = this.dom.find(".class-flat-rate");
                classFlatRate.toggleClass(HIDDEN_CLASS, !node.checked());
            } ]);
        })();
    })();
    __ASSETS._98dbofgjxj38fr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.RenderAppState_mixin = function(id, state, hasLiveApp) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            buf.push('<div class="app-status">' + jade.escape(null == (jade.interp = state.toString(hasLiveApp)) ? "" : jade.interp) + "</div>");
            if (state == chlk.models.apps.AppStateEnum.APPROVED) {
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Live");
                    }
                }, "apps", "goLive", id);
            }
            if (state == chlk.models.apps.AppStateEnum.LIVE) {
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Unlist");
                    }
                }, "apps", "unlist", id);
            }
        };
        var app = self.getApp();
        buf.push('<div class="app-general-info"><div class="action-bar not-transparent dev-app-info"><div class="container panel-bg"></div></div><div class="app-header"><div class="app-icon"><img src="#" width="100" height="80"/></div><div class="app-description"><div class="title">' + jade.escape(null == (jade.interp = app.getName()) ? "" : jade.interp) + "</div>");
        jade.globals.RenderAppState_mixin.call({
            buf: buf
        }, app.getId(), app.getState(), app.getLiveAppId());
        buf.push("</div></div></div>");
        return buf.join("");
    };
    "chlk.models.apps.Application";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppGeneralInfoViewData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppGeneralInfoViewData", [ chlk.models.apps.Application, "app", String, "appThumbnail", [ [ chlk.models.apps.Application ] ], function $(app) {
                this.setApp(app);
            } ]);
        })();
    })();
    "chlk.models.apps.AppGeneralInfoViewData";
    "chlk.models.apps.Application";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).apps = chlk.templates.apps || {};
        (function() {
            chlk.templates.apps.AppGeneral = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_98dbofgjxj38fr") ], [ ria.templates.ModelBind(chlk.models.apps.AppGeneralInfoViewData) ], "AppGeneral", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.apps.Application, "app" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.apps.AppGeneral";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).apps = chlk.activities.apps || {};
        (function() {
            chlk.activities.apps.AppGeneralInfoPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.apps.AppGeneral) ], "AppGeneralInfoPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._vnpem9dg6uyds4i = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="dialog add-app">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<input type="text" name="name"/>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Add");
                    },
                    attributes: {
                        type: "submit",
                        "class": "special-button2" + " " + "blue" + " " + "add"
                    },
                    escaped: {
                        type: true
                    }
                });
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Cancel");
                    },
                    attributes: {
                        "class": "special-button" + " " + "special-button2" + " " + "blue" + " " + "cancel" + " " + "close"
                    },
                    escaped: {}
                });
            }
        }, "apps", "create");
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.apps.Application";
    "chlk.templates.JadeTemplate";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).apps = chlk.templates.apps || {};
        (function() {
            chlk.templates.apps.AddAppDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_vnpem9dg6uyds4i") ], [ ria.templates.ModelBind(chlk.models.apps.Application) ], "AddAppDialog", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], String, "name" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.apps.AddAppDialog";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).apps = chlk.activities.apps || {};
        (function() {
            var HIDDEN_CLASS = "x-hidden";
            var WIDE_CLASS = "wide";
            var TOP_CLASS = "top";
            chlk.activities.apps.AddAppDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.TemplateBind(chlk.templates.apps.AddAppDialog) ], "AddAppDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), [ function $() {
                BASE();
                this._appsCombo = new ria.dom.Dom("#dev-apps-list");
                this._demoFooter = new ria.dom.Dom("#demo-footer");
                this._devApps = new ria.dom.Dom("#dev-apps");
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onResume_() {
                BASE();
                this._appsCombo.addClass(HIDDEN_CLASS);
                this._devApps.addClass(WIDE_CLASS);
                this._demoFooter.removeClass(TOP_CLASS);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onPause_() {
                this._appsCombo.removeClass(HIDDEN_CLASS);
                this._devApps.removeClass(WIDE_CLASS);
                this._demoFooter.addClass(TOP_CLASS);
                BASE();
            } ]);
        })();
    })();
    "chlk.models.id.AppId";
    "chlk.models.id.PictureId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).apps = chlk.models.apps || {};
        (function() {
            "use strict";
            chlk.models.apps.AppPostData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppPostData", [ chlk.models.id.AppId, "id", Boolean, "draft", String, "gradeLevels", String, "permissions", String, "categories", String, "name", String, "url", String, "videoDemoUrl", String, "shortDescription", String, "longDescription", Boolean, "hasTeacherMyApps", Boolean, "hasAdminMyApps", Boolean, "hasStudentMyApps", Boolean, "hasParentMyApps", Boolean, "canAttach", Boolean, "showInGradingView", Number, "costPerUser", Number, "costPerSchool", Number, "costPerClass", Boolean, "free", Boolean, "classFlatRateEnabled", Boolean, "schoolFlatRateEnabled", chlk.models.id.PictureId, "appIconId", chlk.models.id.PictureId, "appBannerId", String, "appScreenShotIds" ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.ApplicationService";
    "chlk.services.AppCategoryService";
    "chlk.services.GradeLevelService";
    "chlk.services.PictureService";
    "chlk.activities.apps.AppsListPage";
    "chlk.activities.apps.AppInfoPage";
    "chlk.activities.apps.AppGeneralInfoPage";
    "chlk.activities.apps.AddAppDialog";
    "chlk.models.apps.Application";
    "chlk.models.apps.AppPostData";
    "chlk.models.apps.ShortAppInfo";
    "chlk.models.apps.AppAccess";
    "chlk.models.apps.AppPicture";
    "chlk.models.apps.AppState";
    "chlk.models.id.AppId";
    "chlk.models.id.AppPermissionId";
    "chlk.models.apps.AppGeneralInfoViewData";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.AppsController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppsController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.ApplicationService, "appsService", [ ria.mvc.Inject ], chlk.services.AppCategoryService, "categoryService", [ ria.mvc.Inject ], chlk.services.GradeLevelService, "gradeLevelService", [ ria.mvc.Inject ], chlk.services.PictureService, "pictureService", [ chlk.controllers.SidebarButton("apps") ], [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.appsService.getApps(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.apps.AppsListPage, result);
            }, [ [ Number ] ], function pageAction(pageIndex_) {
                var result = this.appsService.getApps(pageIndex_ | 0).attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.apps.AppsListPage, result);
            }, [ [ chlk.models.apps.Application, Boolean, Boolean ] ], ria.async.Future, function prepareAppInfo(app_, readOnly, isDraft) {
                var result = this.categoryService.getCategories().then(function(data) {
                    var cats = data.getItems();
                    var gradeLevels = this.gradeLevelService.getGradeLevels();
                    var permissions = this.appsService.getAppPermissions();
                    var appGradeLevels = app_.getGradeLevels();
                    if (!appGradeLevels) app_.setGradeLevels([]);
                    var appCategories = app_.getCategories();
                    if (!appCategories) app_.setCategories([]);
                    if (!app_.getState()) {
                        var appState = new chlk.models.apps.AppState();
                        appState.deserialize(chlk.models.apps.AppStateEnum.DRAFT);
                        app_.setState(appState);
                    }
                    if (!app_.getAppAccess()) app_.setAppAccess(new chlk.models.apps.AppAccess());
                    var appIconId = app_.getSmallPictureId() || new chlk.models.id.PictureId("");
                    var iconUrl = this.pictureService.getPictureUrl(appIconId, 74);
                    app_.setIconPicture(new chlk.models.apps.AppPicture(appIconId, iconUrl, 74, 74, "Icon", !readOnly));
                    var appBannerId = app_.getBigPictureId() || new chlk.models.id.PictureId("");
                    var bannerUrl = this.pictureService.getPictureUrl(appBannerId, 170, 110);
                    app_.setBannerPicture(new chlk.models.apps.AppPicture(appBannerId, bannerUrl, 170, 110, "Banner", !readOnly));
                    return new chlk.models.apps.AppInfoViewData(app_, readOnly, cats, gradeLevels, permissions, isDraft);
                }, this);
                return result;
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.id.AppId, Boolean ] ], function detailsDeveloperAction(appId_, isSubmit_) {
                var isReadonly = false;
                var isDraft = !!!isSubmit_;
                var app = this.appsService.getInfo(appId_).then(function(data) {
                    if (!data.getId()) {
                        return this.forward_("apps", "add", []);
                    } else return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(data, isReadonly, isDraft));
                }, this);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ Number, Number, String, Object ] ], function uploadPictureDeveloperAction(width, height, msg, file) {
                var result = this.appsService.uploadPicture(file, width, height).then(function(id) {
                    var pictureUrl = this.pictureService.getPictureUrl(id, width, height);
                    return new chlk.models.apps.AppPicture(id, pictureUrl, width, height, msg, true);
                }.bind(this));
                return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ String, Number, Number ] ], function deletePictureDeveloperAction(title, width, height) {
                var result = new ria.async.DeferredData(new chlk.models.apps.AppPicture(new chlk.models.id.PictureId(""), "#", width, height, title, true));
                return this.UpdateView(chlk.activities.apps.AppInfoPage, result, title.toLowerCase());
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.SYSADMIN ]) ], [ [ chlk.models.id.AppId ] ], function approveSysAdminAction(appId) {
                return this.appsService.approveApp(appId).then(function(data) {
                    return this.redirect_("apps", "list", []);
                }, this);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.SYSADMIN ]) ], [ [ chlk.models.id.AppId ] ], function declineSysAdminAction(appId) {
                return this.appsService.declineApp(appId).then(function(data) {
                    return this.redirect_("apps", "list", []);
                }, this);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.id.AppId ] ], function goLiveDeveloperAction(appId) {
                return this.appsService.goLive(appId).then(function(data) {
                    return this.redirect_("apps", "general", []);
                }, this);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.id.AppId ] ], function unlistDeveloperAction(appId) {
                return this.appsService.unlist(appId).then(function(data) {
                    return this.redirect_("apps", "general", []);
                }, this);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.SYSADMIN ]) ], [ [ chlk.models.id.AppId ] ], function detailsSysAdminAction(appId) {
                var isReadonly = true;
                var app = this.appsService.getInfo(appId).then(function(data) {
                    return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(data, isReadonly, true));
                }, this);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], function addDeveloperAction() {
                var app = new chlk.models.apps.Application();
                return this.PushView(chlk.activities.apps.AddAppDialog, new ria.async.DeferredData(app));
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.apps.Application ] ], function createDeveloperAction(model) {
                var devId = this.getCurrentPerson().getId();
                var result = this.appsService.createApp(devId, model.getName()).then(function() {
                    return this.forward_("apps", "details", []);
                }, this);
                return result;
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.id.AppId ] ], function deleteDeveloperAction(id) {
                return this.appsService.deleteApp(id).then(function() {
                    return this.forward_("apps", "details", []);
                }, this);
                return result;
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.apps.Application ] ], function updateApp(app) {
                this.getContext().getSession().set("currentApp", app);
                return this.forward_("apps", "details", [ app.getId().valueOf(), true ]);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.id.AppId ] ], function tryDeleteApplicationAction(id) {
                return this.ShowMsgBox("Are you sure you want to delete?", null, [ {
                    text: "Cancel",
                    color: chlk.models.common.ButtonColor.GREEN.valueOf()
                }, {
                    text: "Delete",
                    controller: "apps",
                    action: "delete",
                    params: [ id.valueOf() ],
                    color: chlk.models.common.ButtonColor.RED.valueOf()
                } ], "center");
            }, [ [ String, Function ] ], function getIdsList(ids, idClass) {
                var result = ids ? ids.split(",").map(function(item) {
                    return new idClass(item);
                }) : [];
                return result;
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.apps.AppPostData ] ], function updateDeveloperAction(model) {
                var shortAppData = new chlk.models.apps.ShortAppInfo(model.getName(), model.getUrl(), model.getVideoDemoUrl(), model.getShortDescription(), model.getLongDescription(), model.getAppIconId(), model.getAppBannerId());
                var appAccess = new chlk.models.apps.AppAccess(model.isHasStudentMyApps(), model.isHasTeacherMyApps(), model.isHasAdminMyApps(), model.isHasParentMyApps(), model.isCanAttach(), model.isShowInGradingView());
                var isFreeApp = model.isFree();
                var isSchoolFlatRateEnabled = model.isSchoolFlatRateEnabled();
                var isClassFlatRateEnabled = model.isSchoolFlatRateEnabled();
                var appPriceInfo = isFreeApp ? new chlk.models.apps.AppPrice() : new chlk.models.apps.AppPrice(model.getCostPerUser(), isSchoolFlatRateEnabled ? model.getCostPerClass() : null, isClassFlatRateEnabled ? model.getCostPerSchool() : null);
                var cats = this.getIdsList(model.getCategories(), chlk.models.id.AppCategoryId);
                var gradeLevels = this.getIdsList(model.getGradeLevels(), chlk.models.id.GradeLevelId);
                var appPermissions = this.getIdsList(model.getPermissions(), chlk.models.id.AppPermissionId);
                var appScreenShots = this.getIdsList(model.getAppScreenShotIds(), chlk.models.id.PictureId);
                var result = this.appsService.updateApp(model.getId(), shortAppData, appPermissions, appPriceInfo, this.getCurrentPerson().getId(), appAccess, cats, appScreenShots, gradeLevels, !model.isDraft()).then(function(newApp) {
                    return this.updateApp(newApp);
                }, this);
                return result;
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ [ chlk.models.id.AppId ] ], function generalDeveloperAction(appId_) {
                var app = this.appsService.getInfo(appId_).then(function(data) {
                    if (!data.getId()) {
                        return this.forward_("apps", "add", []);
                    } else {
                        return this.PushView(chlk.activities.apps.AppGeneralInfoPage, new ria.async.DeferredData(new chlk.models.apps.AppGeneralInfoViewData(data)));
                    }
                }, this);
            } ]);
        })();
    })();
    __ASSETS._ferwowd08iu23xr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="app-categories"><div class="action-bar not-transparent"><div class="container panel-bg">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Add application category");
            },
            attributes: {
                "class": "chlk-add" + " " + "opacity-button"
            },
            escaped: {}
        }, "appcategories", "add");
        buf.push("</div></div>");
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">Name</div><div class="th">Description</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getDescription()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "appcategories", "update", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "appcategories", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getDescription()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "appcategories", "update", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "appcategories", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "appcategories", "list", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.apps.AppCategory";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).apps = chlk.templates.apps || {};
        (function() {
            chlk.templates.apps.AppCategories = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_ferwowd08iu23xr") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "AppCategories", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.apps.AppCategory), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.apps.AppCategories";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).apps = chlk.activities.apps || {};
        (function() {
            chlk.activities.apps.AppCategoryListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.apps.AppCategories) ], "AppCategoryListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._u9nd9tc4vs7cik9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="dialog add-app-category gray"><div class="x-window-header"><span>Category</span></div><div class="x-window-body">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", self.id);
                buf.push('<div class="x-form-field"><label>Name:</label><input' + jade.attrs({
                    type: "text",
                    name: "name",
                    value: self.name
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>Description:</label><input' + jade.attrs({
                    type: "text",
                    name: "description",
                    value: self.description
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"></div>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Submit");
                    },
                    attributes: {
                        type: "submit",
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {
                        type: true
                    }
                });
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Cancel");
                    },
                    attributes: {
                        "class": "special-button" + " " + "red-button"
                    },
                    escaped: {}
                });
            }
        }, "appcategories", "save");
        buf.push("</div></div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.apps.AppCategory";
    "chlk.models.id.AppCategoryId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).apps = chlk.templates.apps || {};
        (function() {
            chlk.templates.apps.CategoryDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_u9nd9tc4vs7cik9") ], [ ria.templates.ModelBind(chlk.models.apps.AppCategory) ], "CategoryDialog", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.AppCategoryId, "id", [ ria.templates.ModelPropertyBind ], String, "name", [ ria.templates.ModelPropertyBind ], String, "description" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.apps.CategoryDialog";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).apps = chlk.activities.apps || {};
        (function() {
            chlk.activities.apps.CategoryDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.apps." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.ActivityGroup("CategoryDialog") ], [ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.TemplateBind(chlk.templates.apps.CategoryDialog) ], "CategoryDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.AppCategoryService";
    "chlk.activities.apps.AppCategoryListPage";
    "chlk.activities.apps.CategoryDialog";
    "chlk.models.id.AppCategoryId";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.AppCategoriesController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AppCategoriesController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.AppCategoryService, "appCategoryService", [ chlk.controllers.SidebarButton("settings") ], [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.appCategoryService.getCategories(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.apps.AppCategoryListPage, result);
            }, [ [ Number ] ], function pageAction(pageIndex) {
                var result = this.appCategoryService.getCategories(pageIndex).attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.apps.AppCategoryListPage, result);
            }, [ [ chlk.models.id.AppCategoryId ] ], function updateAction(id) {
                var result = this.appCategoryService.getCategory(id).attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.apps.CategoryDialog, result);
            }, function addAction() {
                var result = new ria.async.DeferredData(new chlk.models.apps.AppCategory());
                return this.ShadeView(chlk.activities.apps.CategoryDialog, result);
            }, [ [ chlk.models.apps.AppCategory ] ], function saveAction(model) {
                var result = this.appCategoryService.saveCategory(model.getId(), model.getName(), model.getDescription()).attach(this.validateResponse_()).then(function(data) {
                    this.view.getCurrent().close();
                    return this.appCategoryService.getCategories(0);
                }.bind(this));
                return this.UpdateView(chlk.activities.apps.AppCategoryListPage, result);
            }, [ [ chlk.models.id.AppCategoryId ] ], function deleteAction(id) {
                var result = this.appCategoryService.removeCategory(id).attach(this.validateResponse_()).then(function(data) {
                    return this.appCategoryService.getCategories(0);
                }, this);
                return this.UpdateView(chlk.activities.apps.AppCategoryListPage, result);
            } ]);
        })();
    })();
    __ASSETS._xpgpt9azy2rl766r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="settings"><div class="row"><div class="item">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Change password");
            }
        }, "account", "resetPassword");
        buf.push('</div><div class="item">');
        if (self.isPreferencesVisible()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("Preferences");
                }
            }, "settings", "preferences");
        }
        buf.push('</div><div class="item">');
        if (self.isAppCategoriesVisible()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("Application categories");
                }
            }, "appcategories", "list");
        }
        buf.push('</div></div><div class="row"><div class="item">');
        if (self.isDepartmentsVisible()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("Departments");
                }
            }, "departments", "list");
        }
        buf.push('</div><div class="item">');
        if (self.isStorageMonitorVisible()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("Storage monitor");
                }
            }, "storage", "list");
        }
        buf.push('</div><div class="item">');
        if (self.isBackgroundTaskMonitorVisible()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("Background task monitor");
                }
            }, "backgroundtask", "page");
        }
        buf.push('</div><div class="item">');
        if (self.isDbMaintenanceVisible()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("Db Maintenance");
                }
            }, "dbmaintenance", "listBackups");
        }
        buf.push("</div></div></div>");
        return buf.join("");
    };
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).settings = chlk.models.settings || {};
        (function() {
            "use strict";
            chlk.models.settings.Dashboard = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Dashboard", [ Boolean, "departmentsVisible", Boolean, "storageMonitorVisible", Boolean, "preferencesVisible", Boolean, "appCategoriesVisible", Boolean, "backgroundTaskMonitorVisible", Boolean, "dbMaintenanceVisible" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.settings.Dashboard";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).settings = chlk.templates.settings || {};
        (function() {
            chlk.templates.settings.Dashboard = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_xpgpt9azy2rl766r") ], [ ria.templates.ModelBind(chlk.models.settings.Dashboard) ], "Dashboard", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Boolean, "departmentsVisible", [ ria.templates.ModelPropertyBind ], Boolean, "appCategoriesVisible", [ ria.templates.ModelPropertyBind ], Boolean, "storageMonitorVisible", [ ria.templates.ModelPropertyBind ], Boolean, "preferencesVisible", [ ria.templates.ModelPropertyBind ], Boolean, "backgroundTaskMonitorVisible", [ ria.templates.ModelPropertyBind ], Boolean, "dbMaintenanceVisible" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.settings.Dashboard";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).settings = chlk.activities.settings || {};
        (function() {
            chlk.activities.settings.DashboardPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.settings.Dashboard) ], "DashboardPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._u50j4aeilfw53ik9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="settings"><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("General");
            },
            attributes: {
                "class": "pressed"
            },
            escaped: {}
        }, "settings", "generalTeacher");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Grading");
            }
        }, "settings", "grading");
        buf.push('</div><div class="right">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Profile");
            }
        }, "teachers", "info", currentChlkPerson.id);
        buf.push('</div></div></div><div class="general"><div class="block"><div class="header">SMS</div><div class="fields"><div class="field">');
        jade.globals.Checkbox_mixin.call({
            buf: buf
        }, "ch1", false);
        buf.push('<label>Announcements</label></div><div class="field">');
        jade.globals.Checkbox_mixin.call({
            buf: buf
        }, "ch1", false);
        buf.push('<label>Private messages</label></div><div class="field">');
        jade.globals.Checkbox_mixin.call({
            buf: buf
        }, "ch1", false);
        buf.push('<label>Notifications</label></div></div></div><div class="block"><div class="header">Email</div><div class="fields"><div class="field">');
        jade.globals.Checkbox_mixin.call({
            buf: buf
        }, "ch1", false);
        buf.push('<label>Announcements</label></div><div class="field">');
        jade.globals.Checkbox_mixin.call({
            buf: buf
        }, "ch1", false);
        buf.push('<label>Private messages</label></div><div class="field">');
        jade.globals.Checkbox_mixin.call({
            buf: buf
        }, "ch1", false);
        buf.push('<label>Notifications</label></div></div></div><div class="block"><div class="header">Password</div><div class="fields"><a id="changePasswordLink" href="javascript:">Reset Password</a></div><div id="changePasswordForm" class="x-hidden">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="title"><div class="name-block"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Old_Password) ? "" : jade.interp) + '</div><input name="oldPassword" type="password" class="validate[required]"/><div class="info-title">' + jade.escape(null == (jade.interp = Msg.New_Password) ? "" : jade.interp) + '</div><input name="newPassword" id="newPassword" type="password" class="validate[required]"/><div class="info-title">' + jade.escape(null == (jade.interp = Msg.New_Password_Confirm) ? "" : jade.interp) + '</div><input name="newPasswordConfirmation" id="newPasswordConfirmation" type="password" class="validate[required, equals[newPassword]]"/></div></div><br/><div class="section-buttons">');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        type: "submit",
                        id: "submit-pwd-button",
                        "class": "special-button" + " " + "blue-button"
                    },
                    escaped: {
                        type: true
                    }
                });
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        type: "button",
                        id: "cancell-edit-pwd-button",
                        "class": "special-button" + " " + "blue-button"
                    },
                    escaped: {
                        type: true
                    }
                });
                buf.push("</div>");
            },
            attributes: {
                id: "info-edit-form",
                "class": "with-ok"
            },
            escaped: {}
        }, "account", "teacherChangePassword");
        buf.push("</div></div></div></div>");
        return buf.join("");
    };
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).settings = chlk.models.settings || {};
        (function() {
            "use strict";
            chlk.models.settings.TeacherSettings = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TeacherSettings", [ Boolean, "annoucementNotificationsViaSms", Boolean, "messagesNotificationsViaSms", Boolean, "notificationsViaSms", Boolean, "annoucementNotificationsViaEmail", Boolean, "messagesNotificationsViaEmail", Boolean, "notificationsViaEmail" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.settings.TeacherSettings";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).settings = chlk.templates.settings || {};
        (function() {
            chlk.templates.settings.TeacherSettings = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_u50j4aeilfw53ik9") ], [ ria.templates.ModelBind(chlk.models.settings.TeacherSettings) ], "TeacherSettings", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Boolean, "annoucementNotificationsViaSms", [ ria.templates.ModelPropertyBind ], Boolean, "messagesNotificationsViaSms", [ ria.templates.ModelPropertyBind ], Boolean, "notificationsViaSms", [ ria.templates.ModelPropertyBind ], Boolean, "annoucementNotificationsViaEmail", [ ria.templates.ModelPropertyBind ], Boolean, "messagesNotificationsViaEmail", [ ria.templates.ModelPropertyBind ], Boolean, "notificationsViaEmail" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.settings.TeacherSettings";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).settings = chlk.activities.settings || {};
        (function() {
            chlk.activities.settings.TeacherPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.TemplateBind(chlk.templates.settings.TeacherSettings) ], "TeacherPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("click", "#changePasswordLink") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function resetPwdClick(node, event) {
                var link = this.dom.find("#changePasswordLink");
                var form = this.dom.find("#changePasswordForm");
                link.addClass("x-hidden");
                form.removeClass("x-hidden");
            }, [ ria.mvc.DomEventBind("click", "#cancell-edit-pwd-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function cancelResetPwdClick(node, event) {
                var link = this.dom.find("#changePasswordLink");
                var form = this.dom.find("#changePasswordForm");
                link.removeClass("x-hidden");
                form.addClass("x-hidden");
            } ]);
        })();
    })();
    __ASSETS._416lv305haes714i = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="settings"><h2>Preferences</h2><div class="preferences grid"><div class="loader"></div><div class="scroller"><div class="container"><div class="body"><div class="row header"><div class="col">Key</div><div class="col">Value</div><div class="col">Hint</div><div class="col">Is public</div><div class="col"></div></div>');
        (function() {
            var $$obj = self.items;
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var item = $$obj[$index];
                    jade.globals.ActionForm_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push('<div class="col">' + jade.escape(null == (jade.interp = item.getKey()) ? "" : jade.interp) + "</div><input" + jade.attrs({
                                name: "key",
                                type: "hidden",
                                value: item.getKey()
                            }, {
                                name: true,
                                type: true,
                                value: true
                            }) + '/><div class="col"><input' + jade.attrs({
                                name: "value",
                                value: item.getValue()
                            }, {
                                name: true,
                                value: true
                            }) + '/></div><div class="col hint">' + jade.escape(null == (jade.interp = item.getHint()) ? "" : jade.interp) + '</div><div class="col centered">');
                            jade.globals.Checkbox_mixin.call({
                                buf: buf
                            }, "ispublicpref", item.isPublicPreference());
                            buf.push('</div><div class="col">');
                            jade.globals.Button_mixin.call({
                                buf: buf,
                                block: function() {
                                    buf.push("Set");
                                }
                            });
                            buf.push("</div>");
                        },
                        attributes: {
                            "class": "row"
                        },
                        escaped: {}
                    }, "settings", "setPreference");
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var item = $$obj[$index];
                    jade.globals.ActionForm_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push('<div class="col">' + jade.escape(null == (jade.interp = item.getKey()) ? "" : jade.interp) + "</div><input" + jade.attrs({
                                name: "key",
                                type: "hidden",
                                value: item.getKey()
                            }, {
                                name: true,
                                type: true,
                                value: true
                            }) + '/><div class="col"><input' + jade.attrs({
                                name: "value",
                                value: item.getValue()
                            }, {
                                name: true,
                                value: true
                            }) + '/></div><div class="col hint">' + jade.escape(null == (jade.interp = item.getHint()) ? "" : jade.interp) + '</div><div class="col centered">');
                            jade.globals.Checkbox_mixin.call({
                                buf: buf
                            }, "ispublicpref", item.isPublicPreference());
                            buf.push('</div><div class="col">');
                            jade.globals.Button_mixin.call({
                                buf: buf,
                                block: function() {
                                    buf.push("Set");
                                }
                            });
                            buf.push("</div>");
                        },
                        attributes: {
                            "class": "row"
                        },
                        escaped: {}
                    }, "settings", "setPreference");
                }
            }
        }).call(this);
        buf.push("</div></div></div></div></div>");
        return buf.join("");
    };
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).settings = chlk.models.settings || {};
        (function() {
            "use strict";
            chlk.models.settings.PreferenceEnum =             function wrapper() {
                var values = {};
                function PreferenceEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(PreferenceEnum, "chlk.models.settings.PreferenceEnum");
                function PreferenceEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.settings.PreferenceEnum#" + value + "]";
                    };
                }
                ria.__API.extend(PreferenceEnumImpl, PreferenceEnum);
                values["videogetinginfoschalkable"] = PreferenceEnum.VIDEO_GETTING_INFO_CHALKABLE = new PreferenceEnumImpl("videogetinginfoschalkable");
                return PreferenceEnum;
            }();
            chlk.models.settings.Preference = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Preference", [ String, "key", String, "value", [ ria.serialize.SerializeProperty("ispublicpref") ], Boolean, "publicPreference", Number, "category", Number, "type", String, "hint" ]);
        })();
    })();
    "chlk.models.settings.Preference";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).settings = chlk.models.settings || {};
        (function() {
            "use strict";
            chlk.models.settings.PreferencesList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("PreferencesList", [ ria.__API.ArrayOf(chlk.models.settings.Preference), "items", [ [ ria.__API.ArrayOf(chlk.models.settings.Preference) ] ], function $(items) {
                this.setItems(items);
            } ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.settings.Preference";
    "chlk.models.settings.PreferencesList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).settings = chlk.templates.settings || {};
        (function() {
            chlk.templates.settings.Preferences = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_416lv305haes714i") ], [ ria.templates.ModelBind(chlk.models.settings.PreferencesList) ], "Preferences", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.settings.Preference), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.settings.Preferences";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).settings = chlk.activities.settings || {};
        (function() {
            chlk.activities.settings.PreferencesPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("settings") ], [ ria.mvc.TemplateBind(chlk.templates.settings.Preferences) ], "PreferencesPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._xu7g7mn21si9hpvi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="settings"><div class="developer"></div><div class="search-wrapper"><div id="search-bar"></div></div><div class="wrapper"><div class="row">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Profile");
            }
        }, "account", "profile", self.developerId.valueOf());
        buf.push('</div><div class="row">');
        id = self.currentAppId ? self.currentAppId.valueOf() : null;
        if (id) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push("Delete application");
                }
            }, "apps", "tryDeleteApplication", id);
        }
        buf.push("</div></div></div>");
        return buf.join("");
    };
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.AppId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).settings = chlk.models.settings || {};
        (function() {
            "use strict";
            chlk.models.settings.DeveloperSettings = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DeveloperSettings", [ chlk.models.id.SchoolPersonId, "developerId", chlk.models.id.AppId, "currentAppId" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.settings.DeveloperSettings";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).settings = chlk.templates.settings || {};
        (function() {
            chlk.templates.settings.DeveloperSettings = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_xu7g7mn21si9hpvi") ], [ ria.templates.ModelBind(chlk.models.settings.DeveloperSettings) ], "DeveloperSettings", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolPersonId, "developerId", [ ria.templates.ModelPropertyBind ], chlk.models.id.AppId, "currentAppId" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.settings.DeveloperSettings";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).settings = chlk.activities.settings || {};
        (function() {
            chlk.activities.settings.DeveloperPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.settings." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("settings") ], [ ria.mvc.TemplateBind(chlk.templates.settings.DeveloperSettings) ], "DeveloperPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.settings.Preference";
    "chlk.models.settings.PreferencesList";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.SettingsService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SettingsService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getPreferences(category_) {
                return this.getArray("Preference/List.json", chlk.models.settings.Preference, {
                    category: category_
                }).then(function(preferences) {
                    return new ria.async.DeferredData(new chlk.models.settings.PreferencesList(preferences));
                });
            }, [ [ String, String, Boolean ] ], ria.async.Future, function setPreference(key, value, isPublic) {
                return this.postArray("Preference/Set.json", chlk.models.settings.Preference, {
                    key: key,
                    value: value,
                    ispublic: isPublic
                }).then(function(preferences) {
                    return new ria.async.DeferredData(new chlk.models.settings.PreferencesList(preferences));
                });
            } ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.activities.settings.DashboardPage";
    "chlk.activities.settings.TeacherPage";
    "chlk.activities.settings.PreferencesPage";
    "chlk.activities.settings.DeveloperPage";
    "chlk.models.settings.Dashboard";
    "chlk.models.settings.Preference";
    "chlk.services.SettingsService";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.SettingsController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SettingsController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.SettingsService, "settingsService", [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.SYSADMIN ]) ], [ chlk.controllers.SidebarButton("settings") ], function dashboardAction() {
                var dashboard = new chlk.models.settings.Dashboard();
                dashboard.setDbMaintenanceVisible(true);
                dashboard.setBackgroundTaskMonitorVisible(true);
                dashboard.setStorageMonitorVisible(true);
                dashboard.setPreferencesVisible(true);
                dashboard.setAppCategoriesVisible(true);
                dashboard.setDepartmentsVisible(true);
                return this.PushView(chlk.activities.settings.DashboardPage, ria.async.DeferredData(dashboard));
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.SYSADMIN ]) ], [ chlk.controllers.SidebarButton("settings") ], function preferencesAction() {
                var result = this.settingsService.getPreferences();
                return this.PushView(chlk.activities.settings.PreferencesPage, result);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.SYSADMIN ]) ], [ chlk.controllers.SidebarButton("settings") ], [ [ chlk.models.settings.Preference ] ], function setPreferenceAction(model) {
                var result = this.settingsService.setPreference(model.getKey(), model.getValue(), model.isPublicPreference());
                return this.UpdateView(chlk.activities.settings.PreferencesPage, result);
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.TEACHER ]) ], [ chlk.controllers.SidebarButton("settings") ], function dashboardTeacherAction() {
                var teacherSettings = new chlk.models.settings.TeacherSettings();
                return this.PushView(chlk.activities.settings.TeacherPage, ria.async.DeferredData(teacherSettings));
            }, function getCurrentApp() {
                return this.getContext().getSession().get("currentApp");
            }, [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.DEVELOPER ]) ], [ chlk.controllers.SidebarButton("settings") ], function dashboardDeveloperAction() {
                var devSettings = new chlk.models.settings.DeveloperSettings();
                devSettings.setDeveloperId(this.getCurrentPerson().getId());
                var app = this.getCurrentApp();
                if (app) {
                    devSettings.setCurrentAppId(app.getId());
                }
                return this.PushView(chlk.activities.settings.DeveloperPage, ria.async.DeferredData(devSettings));
            } ]);
        })();
    })();
    "ria.serialize.IDeserializable";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).bgtasks = chlk.models.bgtasks || {};
        (function() {
            "use strict";
            chlk.models.bgtasks.BgTaskStateEnum =             function wrapper() {
                var values = {};
                function BgTaskStateEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(BgTaskStateEnum, "chlk.models.bgtasks.BgTaskStateEnum");
                function BgTaskStateEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.bgtasks.BgTaskStateEnum#" + value + "]";
                    };
                }
                ria.__API.extend(BgTaskStateEnumImpl, BgTaskStateEnum);
                values[0] = BgTaskStateEnum.CREATED = new BgTaskStateEnumImpl(0);
                values[1] = BgTaskStateEnum.PROCESSING = new BgTaskStateEnumImpl(1);
                values[2] = BgTaskStateEnum.PROCESSED = new BgTaskStateEnumImpl(2);
                values[3] = BgTaskStateEnum.CANCELED = new BgTaskStateEnumImpl(3);
                values[4] = BgTaskStateEnum.FAILED = new BgTaskStateEnumImpl(4);
                return BgTaskStateEnum;
            }();
            chlk.models.bgtasks.BgTaskState = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BgTaskState", ria.__SYNTAX.IMPLEMENTS(ria.serialize.IDeserializable), [ chlk.models.bgtasks.BgTaskStateEnum, "typeId", function $() {
                this._types = {};
                this._types[chlk.models.bgtasks.BgTaskStateEnum.CREATED] = "Created";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.PROCESSING] = "Processing";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.PROCESSED] = "Processed";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.CANCELED] = "Canceled";
                this._types[chlk.models.bgtasks.BgTaskStateEnum.FAILED] = "Failed";
            }, String, function toString() {
                return this._types[this.getTypeId()];
            }, ria.__SYNTAX.Modifiers.VOID, function deserialize(raw) {
                this.setTypeId(new chlk.models.bgtasks.BgTaskStateEnum(Number(raw)));
            } ]);
        })();
    })();
    "ria.serialize.IDeserializable";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).bgtasks = chlk.models.bgtasks || {};
        (function() {
            "use strict";
            chlk.models.bgtasks.BgTaskTypeEnum =             function wrapper() {
                var values = {};
                function BgTaskTypeEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(BgTaskTypeEnum, "chlk.models.bgtasks.BgTaskTypeEnum");
                function BgTaskTypeEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.bgtasks.BgTaskTypeEnum#" + value + "]";
                    };
                }
                ria.__API.extend(BgTaskTypeEnumImpl, BgTaskTypeEnum);
                values[0] = BgTaskTypeEnum.CREATE_EMPTY_SCHOOL = new BgTaskTypeEnumImpl(0);
                values[1] = BgTaskTypeEnum.SIS_DATA_IMPORT = new BgTaskTypeEnumImpl(1);
                values[2] = BgTaskTypeEnum.BACKUP_DATABASES = new BgTaskTypeEnumImpl(2);
                values[3] = BgTaskTypeEnum.RESTORE_DATABASES = new BgTaskTypeEnumImpl(3);
                values[4] = BgTaskTypeEnum.DATABASE_UPDATE = new BgTaskTypeEnumImpl(4);
                values[5] = BgTaskTypeEnum.CREATE_DEMO_SCHOOL = new BgTaskTypeEnumImpl(5);
                values[6] = BgTaskTypeEnum.DELETE_SCHOOL = new BgTaskTypeEnumImpl(6);
                return BgTaskTypeEnum;
            }();
            chlk.models.bgtasks.BgTaskType = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BgTaskType", ria.__SYNTAX.IMPLEMENTS(ria.serialize.IDeserializable), [ chlk.models.bgtasks.BgTaskTypeEnum, "typeId", function $() {
                this._types = {};
                this._types[chlk.models.bgtasks.BgTaskTypeEnum.CREATE_EMPTY_SCHOOL] = "Create empty school";
                this._types[chlk.models.bgtasks.BgTaskTypeEnum.SIS_DATA_IMPORT] = "SIS data import";
                this._types[chlk.models.bgtasks.BgTaskTypeEnum.BACKUP_DATABASES] = "Backup datatabases";
                this._types[chlk.models.bgtasks.BgTaskTypeEnum.RESTORE_DATABASES] = "Restore databases";
                this._types[chlk.models.bgtasks.BgTaskTypeEnum.DATABASE_UPDATE] = "Database Update";
                this._types[chlk.models.bgtasks.BgTaskTypeEnum.CREATE_DEMO_SCHOOL] = "Create Demo School";
                this._types[chlk.models.bgtasks.BgTaskTypeEnum.DELETE_SCHOOL] = "Delete School";
            }, String, function toString() {
                return this._types[this.getTypeId()];
            }, ria.__SYNTAX.Modifiers.VOID, function deserialize(raw) {
                this.setTypeId(new chlk.models.bgtasks.BgTaskTypeEnum(Number(raw)));
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.BgTaskId =             function wrapper() {
                var values = {};
                function BgTaskId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new BgTaskIdImpl(value);
                }
                ria.__API.identifier(BgTaskId, "chlk.models.id.BgTaskId");
                function BgTaskIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.BgTaskId#" + value + "]";
                    };
                }
                ria.__API.extend(BgTaskIdImpl, BgTaskId);
                return BgTaskId;
            }();
        })();
    })();
    "chlk.models.bgtasks.BgTaskState";
    "chlk.models.bgtasks.BgTaskType";
    "chlk.models.common.ChlkDate";
    "chlk.models.id.BgTaskId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).bgtasks = chlk.models.bgtasks || {};
        (function() {
            "use strict";
            chlk.models.bgtasks.BgTask = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BgTask", [ chlk.models.id.BgTaskId, "id", chlk.models.common.ChlkDate, "created", chlk.models.common.ChlkDate, "scheduled", chlk.models.common.ChlkDate, "completed", [ ria.serialize.SerializeProperty("taskstate") ], chlk.models.bgtasks.BgTaskState, "taskState", [ ria.serialize.SerializeProperty("tasktype") ], chlk.models.bgtasks.BgTaskType, "taskType", [ ria.serialize.SerializeProperty("schoolid") ], chlk.models.id.SchoolId, "schoolId" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            chlk.models.id.BgTaskLogId =             function wrapper() {
                var values = {};
                function BgTaskLogId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new BgTaskLogIdImpl(value);
                }
                ria.__API.identifier(BgTaskLogId, "chlk.models.id.BgTaskLogId");
                function BgTaskLogIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.BgTaskLogId#" + value + "]";
                    };
                }
                ria.__API.extend(BgTaskLogIdImpl, BgTaskLogId);
                return BgTaskLogId;
            }();
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.id.BgTaskLogId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).bgtasks = chlk.models.bgtasks || {};
        (function() {
            "use strict";
            chlk.models.bgtasks.BgTaskLog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BgTaskLog", [ chlk.models.id.BgTaskLogId, "id", [ ria.serialize.SerializeProperty("logtype") ], Number, "logType", String, "message", chlk.models.common.ChlkDate, "added" ]);
        })();
    })();
    "chlk.models.bgtasks.BgTaskLog";
    "chlk.models.bgtasks.BgTask";
    "chlk.models.id.BgTaskId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).bgtasks = chlk.models.bgtasks || {};
        (function() {
            "use strict";
            chlk.models.bgtasks.BgTasksLogListViewData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BgTasksLogListViewData", [ chlk.models.common.PaginatedList, "items", chlk.models.id.BgTaskId, "bgTaskId", [ [ chlk.models.id.BgTaskId, chlk.models.common.PaginatedList ] ], function $(bgTaskId, items) {
                this.setBgTaskId(bgTaskId);
                this.setItems(items);
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.bgtasks.BgTask";
    "chlk.models.bgtasks.BgTaskLog";
    "chlk.models.id.BgTaskId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.BgTaskService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BgTaskService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getTasks(pageIndex_) {
                return this.getPaginatedList("BackgroundTask/GetTasks.json", chlk.models.bgtasks.BgTask, {
                    start: pageIndex_
                });
            }, [ [ chlk.models.id.BgTaskId, Number ] ], ria.async.Future, function getLogs(id, start_) {
                return this.getPaginatedList("BackgroundTask/GetTaskLogs.json", chlk.models.bgtasks.BgTaskLog, {
                    taskId: id.valueOf(),
                    start: start_
                });
            }, [ [ chlk.models.id.BgTaskId ] ], ria.async.Future, function deleteTask(id) {
                return this.get("BackgroundTask/Delete.json", null, {
                    taskId: id.valueOf()
                });
            } ]);
        })();
    })();
    __ASSETS._cc35f2f2623p7gb9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="bg-tasks">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">Id</div><div class="th">School Id</div><div class="th">Created</div><div class="th">Scheduled</div><div class="th">Completed</div><div class="th">Task State</div><div class="th">Task Type</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getId().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolId() ? item.getSchoolId().valueOf() : "general") ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCreated().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getScheduled().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCompleted() ? item.getCompleted().toString("dd/m/yy hh:min:ss") : "") ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTaskState().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTaskType().valueOf()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "backgroundtask", "logsPage", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "backgroundtask", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getId().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolId() ? item.getSchoolId().valueOf() : "general") ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCreated().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getScheduled().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCompleted() ? item.getCompleted().toString("dd/m/yy hh:min:ss") : "") ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTaskState().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTaskType().valueOf()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "backgroundtask", "logsPage", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "backgroundtask", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "backgroundtask", "page", self, [ true ]);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.bgtasks.BgTask";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).bgtasks = chlk.templates.bgtasks || {};
        (function() {
            chlk.templates.bgtasks.BgTasks = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_cc35f2f2623p7gb9") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "BgTasks", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.bgtasks.BgTask), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.bgtasks.BgTasks";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).bgtasks = chlk.activities.bgtasks || {};
        (function() {
            chlk.activities.bgtasks.BgTasksListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.bgtasks.BgTasks) ], "BgTasksListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._8gghn31irafw29 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="bg-tasks">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">Type</div><div class="th">Message</div><div class="th">Added</div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items.getItems();
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getId().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getMessage()) ? "" : jade.interp) + "</div><div class=\"td\"></div><!--= item.getAdded().toString('d/m/y')-->");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getId().valueOf()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getMessage()) ? "" : jade.interp) + "</div><div class=\"td\"></div><!--= item.getAdded().toString('d/m/y')-->");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "backgroundtask", "logsPage", self.items, [ self.bgTaskId.valueOf(), true ]);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.bgtasks.BgTask";
    "chlk.models.bgtasks.BgTasksLogListViewData";
    "chlk.models.id.BgTaskId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).bgtasks = chlk.templates.bgtasks || {};
        (function() {
            chlk.templates.bgtasks.BgTaskLogs = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_8gghn31irafw29") ], [ ria.templates.ModelBind(chlk.models.bgtasks.BgTasksLogListViewData) ], "BgTaskLogs", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.common.PaginatedList, "items", [ ria.templates.ModelPropertyBind ], chlk.models.id.BgTaskId, "bgTaskId" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.bgtasks.BgTaskLogs";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).bgtasks = chlk.activities.bgtasks || {};
        (function() {
            chlk.activities.bgtasks.BgTaskLogListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.bgtasks." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.bgtasks.BgTaskLogs) ], "BgTaskLogListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.models.bgtasks.BgTask";
    "chlk.models.bgtasks.BgTasksLogListViewData";
    "chlk.services.BgTaskService";
    "chlk.activities.bgtasks.BgTasksListPage";
    "chlk.activities.bgtasks.BgTaskLogListPage";
    "chlk.models.id.BgTaskId";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.BackgroundTaskController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BackgroundTaskController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.BgTaskService, "bgTaskService", [ chlk.controllers.SidebarButton("settings") ], [ [ Boolean, Number ] ], function pageAction(postback_, pageIndex_) {
                var result = this.bgTaskService.getTasks(pageIndex_ | 0).attach(this.validateResponse_());
                return postback_ ? this.UpdateView(chlk.activities.bgtasks.BgTasksListPage, result) : this.PushView(chlk.activities.bgtasks.BgTasksListPage, result);
            }, [ chlk.controllers.SidebarButton("settings") ], [ [ chlk.models.id.BgTaskId, Boolean, Number ] ], function logsPageAction(id, postback_, start_) {
                var result = this.bgTaskService.getLogs(id, start_ || 0).attach(this.validateResponse_()).then(function(data) {
                    return new ria.async.DeferredData(new chlk.models.bgtasks.BgTasksLogListViewData(id, data));
                });
                return postback_ ? this.UpdateView(chlk.activities.bgtasks.BgTaskLogListPage, result) : this.PushView(chlk.activities.bgtasks.BgTaskLogListPage, result);
            }, [ chlk.controllers.SidebarButton("settings") ], [ [ chlk.models.id.BgTaskId ] ], function deleteAction(id) {
                this.bgTaskService.deleteTask(id).then(function(data) {
                    this.pageAction(true, 0);
                }.bind(this));
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.apps.Application";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.SignUpService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SignUpService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getSignUps(pageIndex_) {
                return this.getPaginatedList("chalkable2/app/data/signups.json", chlk.models.signup.SignUpInfo, {
                    start: pageIndex_
                });
            } ]);
        })();
    })();
    __ASSETS._l72ijfrpo8q6w29 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="signups">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">User Type</div><div class="th">Name</div><div class="th">Email</div><div class="th">Password</div><div class="th">IP Address</div><div class="th">TimeStamp</div><div class="th">School Name</div><div class="th">SIS URL</div><div class="th">Username</div><div class="th">Password</div><div class="th">TimeZone</div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getUserType()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getEmail()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPassword()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getIpAddress()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTimeStamp().toString("d/m/y")) ? "" : jade.interp) + '</div><div class="td"></div><!--= item.getSchoolName()--><div class="td">' + jade.escape(null == (jade.interp = item.getSisUrl()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisUserName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisPassword()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTimezone()) ? "" : jade.interp) + "</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getUserType()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getEmail()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPassword()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getIpAddress()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTimeStamp().toString("d/m/y")) ? "" : jade.interp) + '</div><div class="td"></div><!--= item.getSchoolName()--><div class="td">' + jade.escape(null == (jade.interp = item.getSisUrl()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisUserName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSisPassword()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTimezone()) ? "" : jade.interp) + "</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "signup", "list", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.common.ChlkDate";
    "chlk.models.id.SchoolId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).signup = chlk.models.signup || {};
        (function() {
            "use strict";
            chlk.models.signup.SignUpInfo = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.signup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SignUpInfo", [ chlk.models.id.SchoolId, "id", [ ria.serialize.SerializeProperty("schoolid") ], Number, "schoolId", [ ria.serialize.SerializeProperty("usertype") ], String, "userType", String, "email", String, "password", String, "name", [ ria.serialize.SerializeProperty("schoolname") ], String, "schoolName", [ ria.serialize.SerializeProperty("ipaddress") ], String, "ipAddress", [ ria.serialize.SerializeProperty("sisurl") ], String, "sisUrl", [ ria.serialize.SerializeProperty("sisusername") ], String, "sisUserName", [ ria.serialize.SerializeProperty("sispassword") ], String, "sisPassword", [ ria.serialize.SerializeProperty("systemtype") ], String, "systemType", [ ria.serialize.SerializeProperty("candelete") ], String, "canDelete", [ ria.serialize.SerializeProperty("timestamp") ], chlk.models.common.ChlkDate, "timeStamp", [ ria.serialize.SerializeProperty("timezone") ], String, "timezone" ]);
        })();
    })();
    "chlk.templates.PaginatedList";
    "chlk.models.signup.SignUpInfo";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).signup = chlk.templates.signup || {};
        (function() {
            chlk.templates.signup.SignUpList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.signup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_l72ijfrpo8q6w29") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "SignUpList", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.signup.SignUpInfo), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.signup.SignUpList";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).signup = chlk.activities.signup || {};
        (function() {
            chlk.activities.signup.SignUpListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.signup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.signup.SignUpList) ], "SignUpListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.SignUpService";
    "chlk.activities.signup.SignUpListPage";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.SignUpController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SignUpController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.SignUpService, "signUpService", [ chlk.controllers.SidebarButton("signups") ], [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.signUpService.getSignUps(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.signup.SignUpListPage, result);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).storage = chlk.models.storage || {};
        (function() {
            "use strict";
            chlk.models.storage.Storage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Storage", [ String, "name", String, "uri" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).storage = chlk.models.storage || {};
        (function() {
            "use strict";
            chlk.models.storage.Blob = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Blob", [ String, "name", String, "uri", Number, "size" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.storage.Storage";
    "chlk.models.storage.Blob";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.StorageService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StorageService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getStorages(pageIndex_) {
                return this.getPaginatedList("StorageMonitor/ListBlobContainers.json", chlk.models.storage.Storage, {
                    start: pageIndex_
                });
            }, [ [ String, Number ] ], ria.async.Future, function getBlobs(uri, pageIndex_) {
                return this.getPaginatedList("StorageMonitor/ListBlobs.json", chlk.models.storage.Blob, {
                    containeraddress: uri,
                    start: pageIndex_
                });
            }, ria.async.Future, function deleteBlob(uri) {
                return this.getPaginatedList("StorageMonitor/Delete.json", chlk.models.storage.Blob, {
                    blobAddress: uri
                });
            } ]);
        })();
    })();
    "chlk.models.storage.Blob";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).storage = chlk.models.storage || {};
        (function() {
            "use strict";
            chlk.models.storage.BlobsListViewData = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("BlobsListViewData", [ chlk.models.common.PaginatedList, "items", String, "containerAddress" ]);
        })();
    })();
    __ASSETS._huqx4yrd75oy9zfr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="storages">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">Name</div><div class="th">Uri</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getUri()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "storage", "blobs", item.getUri());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getUri()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "storage", "blobs", item.getUri());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "storage", "page", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.storage.Storage";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).storage = chlk.templates.storage || {};
        (function() {
            chlk.templates.storage.Storages = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_huqx4yrd75oy9zfr") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "Storages", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.storage.Storage), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.storage.Storages";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).storage = chlk.activities.storage || {};
        (function() {
            chlk.activities.storage.StorageListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.storage.Storages) ], "StorageListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._hagdlkyreutprpb9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="blobs">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">Name</div><div class="th">Uri</div><div class="th">Size</div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items.getItems();
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getUri()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSize()) ? "" : jade.interp) + "</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getUri()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSize()) ? "" : jade.interp) + "</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "storage", "blobsPage", self.items, [ self.containerAddress ]);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.storage.Blob";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).storage = chlk.templates.storage || {};
        (function() {
            chlk.templates.storage.Blobs = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_hagdlkyreutprpb9") ], [ ria.templates.ModelBind(chlk.models.storage.BlobsListViewData) ], "Blobs", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.common.PaginatedList, "items", [ ria.templates.ModelPropertyBind ], String, "containerAddress" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.storage.Blobs";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).storage = chlk.activities.storage || {};
        (function() {
            chlk.activities.storage.StorageBlobsPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.storage.Blobs) ], "StorageBlobsPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.StorageService";
    "chlk.models.storage.BlobsListViewData";
    "chlk.activities.storage.StorageListPage";
    "chlk.activities.storage.StorageBlobsPage";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.StorageController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StorageController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.StorageService, "storageService", [ chlk.controllers.SidebarButton("settings") ], [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.storageService.getStorages(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.storage.StorageListPage, result);
            }, [ [ Number ] ], function pageAction(pageIndex_) {
                var result = this.storageService.getStorages(pageIndex_ | 0).attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.storage.StorageListPage, result);
            }, [ [ String, Number ] ], function blobsAction(uri, pageIndex_) {
                var result = this.storageService.getBlobs(uri, pageIndex_ | 0).attach(this.validateResponse_()).then(function(data) {
                    var blobsListViewData = new chlk.models.storage.BlobsListViewData();
                    blobsListViewData.setItems(data);
                    blobsListViewData.setContainerAddress(uri);
                    return new ria.async.DeferredData(blobsListViewData);
                });
                return this.PushView(chlk.activities.storage.StorageBlobsPage, result);
            }, [ [ String, Number ] ], function blobsPageAction(uri, pageIndex_) {
                var result = this.storageService.getBlobs(uri, pageIndex_ | 0).attach(this.validateResponse_()).then(function(data) {
                    var blobsListViewData = new chlk.models.storage.BlobsListViewData();
                    blobsListViewData.setItems(data);
                    blobsListViewData.setContainerAddress(uri);
                    return new ria.async.DeferredData(blobsListViewData);
                });
                return this.UpdateView(chlk.activities.storage.StorageBlobsPage, result);
            } ]);
        })();
    })();
    "chlk.models.common.ChlkDate";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).funds = chlk.models.funds || {};
        (function() {
            "use strict";
            chlk.models.funds.Fund = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.funds." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Fund", [ [ ria.serialize.SerializeProperty("requestid") ], Number, "id", [ ria.serialize.SerializeProperty("schoolid") ], Number, "schoolId", [ ria.serialize.SerializeProperty("schoolname") ], String, "schoolName", [ ria.serialize.SerializeProperty("ponumber") ], Number, "poNumber", Number, "amount", [ ria.serialize.SerializeProperty("statetype") ], Number, "stateType", String, "name", String, "terms", [ ria.serialize.SerializeProperty("displayname") ], String, "displayName", chlk.models.common.ChlkDate, "created" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.funds.Fund";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.FundsService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("FundsService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getFunds(pageIndex_) {
                return this.getPaginatedList("chalkable2/app/data/funds.json", chlk.models.funds.Fund, {
                    start: pageIndex_
                });
            } ]);
        })();
    })();
    __ASSETS._8p95mcnzeiv0a4i = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="funds">');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">School</div><div class="th">PO#</div><div class="th">Created By</div><div class="th">Created On</div><div class="th">Amount</div><div class="th">Terms</div><div class="th"></div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getSchoolName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPoNumber()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getDisplayName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCreated().toString("d/m/y")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getAmount()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTerms()) ? "" : jade.interp) + '</div><div class="td"></div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "funds", "accept", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "funds", "reject", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf
                                            }, "funds", "export", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getSchoolName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getPoNumber()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getDisplayName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getCreated().toString("d/m/y")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getAmount()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTerms()) ? "" : jade.interp) + '</div><div class="td"></div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "funds", "accept", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "funds", "reject", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf
                                            }, "funds", "export", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "funds", "list", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.funds.Fund";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).funds = chlk.templates.funds || {};
        (function() {
            chlk.templates.funds.Funds = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.funds." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_8p95mcnzeiv0a4i") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "Funds", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.funds.Fund), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.funds.Funds";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).funds = chlk.activities.funds || {};
        (function() {
            chlk.activities.funds.FundsListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.funds." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.funds.Funds) ], "FundsListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.FundsService";
    "chlk.activities.funds.FundsListPage";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.FundsController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("FundsController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.FundsService, "fundsService", [ chlk.controllers.SidebarButton("statistic") ], [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.fundsService.getFunds(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.funds.FundsListPage, result);
            }, [ [ Number ] ], function acceptAction(id) {}, [ [ Number ] ], function rejectAction(id) {}, [ [ Number ] ], function exportAction(id) {} ]);
        })();
    })();
    "chlk.models.id.DepartmentId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).departments = chlk.models.departments || {};
        (function() {
            "use strict";
            chlk.models.departments.Department = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.departments." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Department", [ chlk.models.id.DepartmentId, "id", String, "name", String, "keywords", Object, "icon" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.departments.Department";
    "chlk.models.id.DepartmentId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.DepartmentsService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DepartmentsService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getDepartments(pageIndex_) {
                return this.getPaginatedList("ChalkableDepartment/List.json", chlk.models.departments.Department, {
                    start: pageIndex_ | 0
                });
            }, [ [ String, String, FileList ] ], ria.async.Future, function addDepartment(name, keywords, files) {
                return this.uploadFiles("ChalkableDepartment/create.json", files, chlk.models.departments.Department, {
                    name: name,
                    keywords: keywords
                });
            }, [ [ chlk.models.id.DepartmentId, String, String, FileList ] ], ria.async.Future, function updateDepartment(id, name, keywords, files) {
                return this.uploadFiles("ChalkableDepartment/create.json", files, chlk.models.departments.Department, {
                    chalkableDepartmentId: id.valueOf(),
                    name: name,
                    keywords: keywords
                });
            }, [ [ chlk.models.id.DepartmentId, String, String, FileList ] ], ria.async.Future, function saveDepartment(id_, name, keywords, files) {
                if (id_ && id_.valueOf()) return this.updateDepartment(id_, name, keywords, files);
                return this.addDepartment(name, keywords, files);
            }, [ [ chlk.models.id.DepartmentId ] ], ria.async.Future, function removeDepartment(id) {
                return this.post("ChalkableDepartment/Delete.json", chlk.models.departments.Department, {
                    chalkableDepartmentId: id.valueOf()
                });
            }, [ [ chlk.models.id.DepartmentId ] ], ria.async.Future, function getDepartment(id) {
                return this.post("ChalkableDepartment/Info.json", chlk.models.departments.Department, {
                    chalkableDepartmentId: id.valueOf()
                });
            } ]);
        })();
    })();
    __ASSETS._59de4kefpjo9wwmi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="departments"><div class="action-bar not-transparent"><div class="container panel-bg">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Add Department");
            },
            attributes: {
                "class": "chlk-add" + " " + "opacity-button"
            },
            escaped: {}
        }, "departments", "add");
        buf.push("</div></div>");
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th"></div><div class="th">Name</div><div class="th">keywords</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td"><img' + jade.attrs({
                                                src: self.getPictureURL(item.getId(), 46, 26),
                                                "class": "avatar" + " " + "avatar47"
                                            }, {
                                                src: true
                                            }) + '/></div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getKeywords()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "departments", "update", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "departments", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td"><img' + jade.attrs({
                                                src: self.getPictureURL(item.getId(), 46, 26),
                                                "class": "avatar" + " " + "avatar47"
                                            }, {
                                                src: true
                                            }) + '/></div><div class="td">' + jade.escape(null == (jade.interp = item.getName()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getKeywords()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "profile"
                                                },
                                                escaped: {}
                                            }, "departments", "update", item.getId());
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "remove"
                                                },
                                                escaped: {}
                                            }, "departments", "delete", item.getId());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "departments", "page", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.departments.Department";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).departments = chlk.templates.departments || {};
        (function() {
            chlk.templates.departments.Departments = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.departments." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_59de4kefpjo9wwmi") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "Departments", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.departments.Department), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.departments.Departments";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).departments = chlk.activities.departments || {};
        (function() {
            chlk.activities.departments.DepartmentsListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.departments." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.departments.Departments) ], "DepartmentsListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._bfxxmruf6v9rudi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="dialog add-department gray"><div class="x-window-header"><span>Department</span></div><div class="x-window-body">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", self.id);
                buf.push('<div class="x-form-field"><label>File:</label>');
                jade.globals.FileUpload_mixin.call({
                    buf: buf,
                    attributes: {
                        name: "icon",
                        id: "icon",
                        "class": "simple-gray-button"
                    },
                    escaped: {
                        name: true
                    }
                });
                buf.push('</div><div class="x-form-field"><label>Name:</label><input' + jade.attrs({
                    type: "text",
                    name: "name",
                    value: self.name
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>Keywords:</label><input' + jade.attrs({
                    type: "text",
                    name: "keywords",
                    value: self.keywords
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + "/></div>");
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Submit");
                    },
                    attributes: {
                        type: "submit",
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {
                        type: true
                    }
                });
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Cancel");
                    },
                    attributes: {
                        "class": "special-button" + " " + "red-button" + " " + "close"
                    },
                    escaped: {}
                });
            }
        }, "departments", "save");
        buf.push("</div></div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.departments.Department";
    "chlk.models.id.DepartmentId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).departments = chlk.templates.departments || {};
        (function() {
            chlk.templates.departments.DepartmentDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.departments." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_bfxxmruf6v9rudi") ], [ ria.templates.ModelBind(chlk.models.departments.Department) ], "DepartmentDialog", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.DepartmentId, "id", [ ria.templates.ModelPropertyBind ], String, "name", [ ria.templates.ModelPropertyBind ], String, "keywords" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.departments.DepartmentDialog";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).departments = chlk.activities.departments || {};
        (function() {
            chlk.activities.departments.DepartmentDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.departments." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.ActivityGroup("DepartmentDialog") ], [ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.TemplateBind(chlk.templates.departments.DepartmentDialog) ], "DepartmentDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.DepartmentsService";
    "chlk.activities.departments.DepartmentsListPage";
    "chlk.activities.departments.DepartmentDialog";
    "chlk.models.departments.Department";
    "chlk.models.id.DepartmentId";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.DepartmentsController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DepartmentsController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.DepartmentsService, "departmentService", [ chlk.controllers.SidebarButton("settings") ], [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.departmentService.getDepartments(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.departments.DepartmentsListPage, result);
            }, [ [ Number ] ], function pageAction(pageIndex) {
                var result = this.departmentService.getDepartments(pageIndex).attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.departments.DepartmentsListPage, result);
            }, [ [ chlk.models.id.DepartmentId ] ], function updateAction(id) {
                var result = this.departmentService.getDepartment(id).attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.departments.DepartmentDialog, result);
            }, function addAction() {
                var result = new ria.async.DeferredData(new chlk.models.departments.Department());
                return this.ShadeView(chlk.activities.departments.DepartmentDialog, result);
            }, [ [ chlk.models.departments.Department ] ], function saveAction(model) {
                var result = this.departmentService.saveDepartment(model.getId(), model.getName(), model.getKeywords(), model.getIcon()).attach(this.validateResponse_()).then(function(data) {
                    this.view.getCurrent().close();
                    return this.departmentService.getDepartments(0);
                }.bind(this));
                return this.UpdateView(chlk.activities.departments.DepartmentsListPage, result);
            }, [ [ chlk.models.id.DepartmentId ] ], function deleteAction(id) {
                var result = this.departmentService.removeDepartment(id).attach(this.validateResponse_()).then(function(data) {
                    return this.departmentService.getDepartments(0);
                }, this);
                return this.UpdateView(chlk.activities.departments.DepartmentsListPage, result);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).dbmaintenance = chlk.models.dbmaintenance || {};
        (function() {
            "use strict";
            chlk.models.dbmaintenance.DbBackup = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.dbmaintenance." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DbBackup", [ String, "ticks", chlk.models.common.ChlkDate, "created", [ ria.serialize.SerializeProperty("hasmaster") ], Boolean, "hasMaster", [ ria.serialize.SerializeProperty("hasschooltemplate") ], Boolean, "hasSchoolTemplate", [ ria.serialize.SerializeProperty("schoolcount") ], Number, "schoolCount" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.departments.Department";
    "chlk.models.dbmaintenance.DbBackup";
    "chlk.models.Success";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.DbMaintenanceService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DbMaintenanceService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ ria.async.Future, function backup() {
                return this.get("dbmaintenance/Backup.json", chlk.models.Success, {});
            }, [ [ String ] ], ria.async.Future, function restore(ticks) {
                return this.get("dbmaintenance/Restore.json", chlk.models.Success, {
                    time: ticks
                });
            }, [ [ String, String ] ], ria.async.Future, function databaseUpdate(masterSql, schoolSql) {
                return this.post("dbmaintenance/DatabaseUpdate.json", chlk.models.Success, {
                    masterSql: masterSql,
                    schoolSql: schoolSql
                });
            }, [ [ Number, Number ] ], ria.async.Future, function getBackups(start_, count_) {
                return this.getPaginatedList("dbmaintenance/ListBackups.json", chlk.models.dbmaintenance.DbBackup, {
                    start: start_,
                    count: count_
                });
            } ]);
        })();
    })();
    __ASSETS._hd9zz0gojzg58kt9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="db-maintenance"><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Backups");
            },
            attributes: {
                "class": "pressed"
            },
            escaped: {}
        }, "dbmaintenance", "listBackups");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Update");
            }
        }, "dbmaintenance", "updateDb");
        buf.push("</div></div></div>");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("BackUp");
                    },
                    attributes: {
                        "class": "special-button2" + " " + "green"
                    },
                    escaped: {}
                });
            },
            attributes: {
                "class": "i-b" + " " + "t-d" + " " + "not-blue"
            },
            escaped: {}
        }, "dbmaintenance", "backup");
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridHead_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push('<div class="th">Date</div><div class="th">Ticks</div><div class="th">Has Master</div><div class="th">Has S. Template</div><div class="th">School Count</div><div class="th"></div>');
                    }
                });
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getCreated().toString("dd/mm/yy hh/min/ss")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTicks()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.isHasMaster()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.isHasSchoolTemplate()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolCount()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "plugin"
                                                },
                                                escaped: {}
                                            }, "dbmaintenance", "restore", item.getTicks());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    jade.globals.GridRow_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="td">' + jade.escape(null == (jade.interp = item.getCreated().toString("dd/mm/yy hh/min/ss")) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getTicks()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.isHasMaster()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.isHasSchoolTemplate()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSchoolCount()) ? "" : jade.interp) + '</div><div class="td action-links">');
                                            jade.globals.ActionLink_mixin.call({
                                                buf: buf,
                                                attributes: {
                                                    "class": "plugin"
                                                },
                                                escaped: {}
                                            }, "dbmaintenance", "restore", item.getTicks());
                                            buf.push("</div>");
                                        }
                                    });
                                }
                            }
                        }).call(this);
                    }
                });
            },
            attributes: {
                "class": "not-transparent"
            },
            escaped: {}
        }, "dbmaintenance", "listBackups", self);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.dbmaintenance.DbBackup";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).storage = chlk.templates.storage || {};
        (function() {
            chlk.templates.storage.DbMaintenance = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_hd9zz0gojzg58kt9") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "DbMaintenance", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.dbmaintenance.DbBackup), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.storage.DbMaintenance";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).storage = chlk.activities.storage || {};
        (function() {
            chlk.activities.storage.DbMaintenancePage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.storage.DbMaintenance) ], [ ria.mvc.PartialUpdateRule(chlk.templates.storage.DbMaintenance, "", null, ria.mvc.PartialUpdateRuleActions.Replace) ], "DbMaintenancePage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._re6oe60rm1ymygb9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="db-maintenance"><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Backups");
            }
        }, "dbmaintenance", "listBackups");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Update");
            },
            attributes: {
                "class": "pressed"
            },
            escaped: {}
        }, "dbmaintenance", "updateDb");
        buf.push("</div></div></div>");
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="x-form-field"><label>Master Update</label><textarea name="masterSql"></textarea></div><div class="x-form-field"><label>School Update</label><textarea name="schoolSql"></textarea></div>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("RUN");
                    },
                    attributes: {
                        type: "submit",
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {
                        type: true
                    }
                });
            },
            attributes: {
                "class": "db-update-form"
            },
            escaped: {}
        }, "dbmaintenance", "runSql");
        buf.push("</div>");
        return buf.join("");
    };
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).storage = chlk.models.storage || {};
        (function() {
            "use strict";
            chlk.models.storage.DatabaseUpdate = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DatabaseUpdate", [ String, "masterSql", String, "schoolSql" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.storage.DatabaseUpdate";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).storage = chlk.templates.storage || {};
        (function() {
            chlk.templates.storage.DatabaseUpdate = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_re6oe60rm1ymygb9") ], [ ria.templates.ModelBind(chlk.models.storage.DatabaseUpdate) ], "DatabaseUpdate", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], String, "masterSql", [ ria.templates.ModelPropertyBind ], String, "schoolSql" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.storage.DatabaseUpdate";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).storage = chlk.activities.storage || {};
        (function() {
            chlk.activities.storage.DatabaseUpdatePage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.storage." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.storage.DatabaseUpdate) ], "DatabaseUpdatePage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.DbMaintenanceService";
    "chlk.activities.storage.DbMaintenancePage";
    "chlk.activities.storage.DatabaseUpdatePage";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.DbMaintenanceController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DbMaintenanceController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.DbMaintenanceService, "dbMaintenanceService", [ [ Number ] ], function listBackupsAction(start_) {
                var result = this.dbMaintenanceService.getBackups(start_ || 0, 10).attach(this.validateResponse_()).then(function(model) {
                    this.getContext().getSession().set("listBackups", model);
                    return model;
                }.bind(this));
                return this.PushView(chlk.activities.storage.DbMaintenancePage, result);
            }, [ [ chlk.models.storage.DatabaseUpdate ] ], function updateDbAction(model_) {
                model_ = model_ || new chlk.models.storage.DatabaseUpdate();
                return this.PushView(chlk.activities.storage.DatabaseUpdatePage, new ria.async.DeferredData(model_));
            }, [ [ chlk.models.storage.DatabaseUpdate ] ], function runSqlAction(model) {
                this.dbMaintenanceService.databaseUpdate(model.getMasterSql(), model.getSchoolSql());
            }, function backupAction() {
                var result = this.dbMaintenanceService.backup().then(function(success) {
                    return this.getContext().getSession().get("listBackups");
                }.bind(this));
                return this.UpdateView(chlk.activities.storage.DbMaintenancePage, result);
            }, [ [ String ] ], function restoreAction(ticks) {
                var result = this.dbMaintenanceService.restore(ticks).then(function(success) {
                    return this.getContext().getSession().get("listBackups");
                }.bind(this));
                return this.UpdateView(chlk.activities.storage.DbMaintenancePage, result);
            } ]);
        })();
    })();
    "chlk.BaseApp";
    "chlk.controllers.SchoolsController";
    "chlk.controllers.DistrictController";
    "chlk.controllers.AccountController";
    "chlk.controllers.AppsController";
    "chlk.controllers.AppCategoriesController";
    "chlk.controllers.SettingsController";
    "chlk.controllers.BackgroundTaskController";
    "chlk.controllers.SignUpController";
    "chlk.controllers.StorageController";
    "chlk.controllers.FundsController";
    "chlk.controllers.DepartmentsController";
    "chlk.controllers.DbMaintenanceController";
    (function() {
        chlk = chlk || {};
        (function() {
            chlk.SysAdminApp = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SysAdminApp", ria.__SYNTAX.EXTENDS(chlk.BaseApp), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();
                dispatcher.setDefaultControllerId("district");
                dispatcher.setDefaultControllerAction("list");
                return dispatcher;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set("role", new chlk.models.common.Role(chlk.models.common.RoleEnum.SYSADMIN, "SysAdmin"));
                return session;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.async.Future, function onStart_() {
                return BASE().then(function(data) {
                    new ria.dom.Dom().fromHTML(ASSET("_mfwd7z7vjagaatt9")()).appendTo("#sidebar");
                    return data;
                });
            } ]);
        })();
    })();
    (function() {
        new chlk.SysAdminApp().session(ria.__CFG["#mvc"].settings || {}).run();
    })();
})(jQuery, jade);