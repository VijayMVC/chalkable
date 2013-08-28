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
    __ASSETS._ihgenbsa8l6ry66r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                id: "home-logo"
            },
            escaped: {}
        }, "feed", "list");
        buf.push('<div id="sidebar-controls"><div class="search-wrapper"><div id="search-bar">');
        jade.globals.SearchBox_mixin.call({
            buf: buf,
            attributes: {
                id: "siteSearch",
                name: "siteSearch"
            },
            escaped: {
                name: true
            }
        }, chlk.services.SearchService, "search", chlk.templates.search.SiteSearch);
        buf.push('</div></div><div class="wrapper">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "fullwidth-button" + " " + "add-new"
            },
            escaped: {}
        }, "announcement", "add");
        buf.push('<div class="buttons-row">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Calendar");
            },
            attributes: {
                title: "View Calendar",
                "class": "one-third-button" + " " + "calendar"
            },
            escaped: {
                title: true
            }
        }, "calendar", "month");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Messages");
            },
            attributes: {
                title: "View Messages",
                "class": "one-third-button" + " " + "messages"
            },
            escaped: {
                title: true
            }
        }, "message", "page");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("People");
            },
            attributes: {
                title: "View People",
                "class": "one-third-button" + " " + "people"
            },
            escaped: {
                title: true
            }
        }, "students", "all");
        buf.push('</div><div class="buttons-row">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Grades");
            },
            attributes: {
                title: "View Grades",
                "class": "one-third-button" + " " + "statistic"
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
                title: "View Apps",
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
                title: "View Settings",
                "class": "one-third-button" + " " + "settings"
            },
            escaped: {
                title: true
            }
        }, "settings", "dashboardTeacher");
        buf.push('</div><div class="buttons-row">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Attendance");
            },
            attributes: {
                title: "View Attendance",
                "class": "one-third-button" + " " + "statistic"
            },
            escaped: {
                title: true
            }
        }, "attendance", "list");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Discipline");
            },
            attributes: {
                title: "View Discipline",
                "class": "one-third-button" + " " + "discipline"
            },
            escaped: {
                title: true
            }
        }, "discipline", "list");
        buf.push("</div></div></div>");
        return buf.join("");
    };
    __ASSETS._y24qgi22nfiozuxr = function anonymous(locals) {
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
    __ASSETS._khpycj2hu9q0vn29 = function anonymous(locals) {
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
            buf.push("<input" + jade.attrs({
                type: "hidden",
                name: hidName,
                value: hidValue
            }, {
                type: true,
                name: true,
                value: true
            }) + "/>");
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
            var r20 = /%20/g, rbracket = /\[\]$/, rCRLF = /\r?\n/g, rreturn = /\r/g, rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i, manipulation_rcheckableType = /^(?:checkbox|radio)$/i, rsubmittable = /^(?:input|select|textarea|keygen)/i;
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
                ASSET("_khpycj2hu9q0vn29")(this);
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
                var formSelector = attributes.id ? "#" + attributes.id : attributes.class ? "." + attributes.class : "form";
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
    __ASSETS._nrpc6mehvog30udi = function anonymous(locals) {
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
                ASSET("_nrpc6mehvog30udi")(this);
            }, [ [ ria.dom.Dom, ria.dom.Event ] ], Boolean, function changed_($target, event) {} ]);
        })();
    })();
    __ASSETS._86rjiaon48q41jor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.SlideCheckbox_mixin = function(name, val, isReadOnly) {
            var block = this.block, attributes = this.attributes || {}, escaped = this.escaped || {}, buf = this.buf;
            console.log("readonly", isReadOnly);
            buf.push("<div" + jade.attrs(jade.merge({
                "class": "slide-checkbox"
            }, attributes), jade.merge({}, escaped, true)) + '><div class="wrapper">');
            jade.globals.Hidden_mixin.call({
                buf: buf
            }, name, false);
            attributes.checked = val;
            buf.push("<input" + jade.attrs(jade.merge({
                id: name,
                name: name,
                type: "checkbox"
            }, attributes), jade.merge({
                id: true,
                name: true,
                type: true
            }, escaped, true)) + "/>");
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
                ASSET("_86rjiaon48q41jor")(this);
            } ]);
        })();
    })();
    __ASSETS._7tewvcgzbhkyy14i = function anonymous(locals) {
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
                ASSET("_7tewvcgzbhkyy14i")(this);
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
    __ASSETS._v6p97ap0l6tdfgvi = function anonymous(locals) {
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
                ASSET("_v6p97ap0l6tdfgvi")(this);
            } ]);
        })();
    })();
    __ASSETS._01ntmdla1zrjatt9 = function anonymous(locals) {
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
                ASSET("_01ntmdla1zrjatt9")(this);
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
    __ASSETS._oq7zlte8qgogk3xr = function anonymous(locals) {
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
                ASSET("_oq7zlte8qgogk3xr")(this);
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
    __ASSETS._9nii3njovw75jyvi = function anonymous(locals) {
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
                ASSET("_9nii3njovw75jyvi")(this);
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
    __ASSETS._83uaiywbpltvgqfr = function anonymous(locals) {
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
                ASSET("_83uaiywbpltvgqfr")(this);
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
    __ASSETS._rsrxw3swvbvb1emi = function anonymous(locals) {
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
                ASSET("_rsrxw3swvbvb1emi")(this);
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
    __ASSETS._6z8jddsl1ve7b9 = function anonymous(locals) {
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
                ASSET("_6z8jddsl1ve7b9")(this);
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
    __ASSETS._u3d2q20yfucba9k9 = function anonymous(locals) {
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
                ASSET("_u3d2q20yfucba9k9")(this);
            }, [ [ Object ] ], Object, function prepareData(attrs) {
                attrs.id = attrs.id || ria.dom.NewGID();
                var that = this, params = attrs["data-params"], controller = attrs["data-controller"], action = attrs["data-action"];
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
                return attrs;
            } ]);
        })();
    })();
    __ASSETS._7dnma29zy9cnmi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.GlanceBox_mixin = function(controller, action, data) {
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
                    "class": "value" + " " + self.getValueClass(data.value)
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
                                buf.push("<div>" + jade.escape((jade.interp = self.getShortText(item.value, item.title)) == null ? "" : jade.interp) + "</div>");
                            }
                        } else {
                            var $$l = 0;
                            for (var i in $$obj) {
                                $$l++;
                                var item = $$obj[i];
                                buf.push("<div>" + jade.escape((jade.interp = self.getShortText(item.value, item.title)) == null ? "" : jade.interp) + "</div>");
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
                ASSET("_7dnma29zy9cnmi")(this);
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
    __ASSETS._lr02fx9pyqr529 = function anonymous(locals) {
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
                ASSET("_lr02fx9pyqr529")(this);
            } ]);
        })();
    })();
    __ASSETS._g4uro2zi3jfko6r = function anonymous(locals) {
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
                ASSET("_g4uro2zi3jfko6r")(this);
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
    __ASSETS._e5n0calyz4zpvi = function anonymous(locals) {
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
                ASSET("_e5n0calyz4zpvi")(this);
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
    __ASSETS._g015l4je3mu0udi = function anonymous(locals) {
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
                ASSET("_g015l4je3mu0udi")(this);
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
    __ASSETS._idojvb3optzw7b9 = function anonymous(locals) {
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
                ASSET("_idojvb3optzw7b9")(this);
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
    __ASSETS._dfewdmdklizwu3di = function anonymous(locals) {
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
                ASSET("_dfewdmdklizwu3di")(this);
            } ]);
        })();
    })();
    __ASSETS._43vwycavu6czyqfr = function anonymous(locals) {
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
                ASSET("_43vwycavu6czyqfr")(this);
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
    __ASSETS._dr4cxh1mcjotuik9 = function anonymous(locals) {
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
                ASSET("_dr4cxh1mcjotuik9")(this);
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
    __ASSETS._zwuctmaxqvjthuxr = function anonymous(locals) {
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
                ASSET("_zwuctmaxqvjthuxr")(this);
            }, [ [ Object ] ], Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                this.context.getDefaultView().onActivityRefreshed(function(activity, model) {
                    jQuery("textarea#" + attributes.id).autosize();
                }.bind(this));
                return attributes;
            } ]);
        })();
    })();
    __ASSETS._br51m69u23xr = function anonymous(locals) {
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
                ASSET("_br51m69u23xr")(this);
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
                    if (this.getCurrentPerson()) new ria.dom.Dom().fromHTML(ASSET("_y24qgi22nfiozuxr")(this)).appendTo("#logout-block");
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
    __ASSETS._469u3bivb863l3di = function anonymous(locals) {
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
    __ASSETS._ge22iah97ug9o1or = function anonymous(locals) {
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
            ASSET("_ge22iah97ug9o1or")();
            chlk.templates.JadeTemplate = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("JadeTemplate", ria.__SYNTAX.EXTENDS(ria.templates.CompiledTemplate), [ Function, "block", [ [ Object, Number ] ], String, function getPictureURL(id, size_) {
                var url = window.azurePictureUrl + id.valueOf();
                return size_ ? url + "-" + size_ + "x" + size_ : url;
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
            }([ ria.templates.TemplateBind("_469u3bivb863l3di") ], [ ria.templates.ModelBind(chlk.models.common.InfoMsg) ], "InfoMsg", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], String, "text", [ ria.templates.ModelPropertyBind ], String, "header", [ ria.templates.ModelPropertyBind ], String, "clazz", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.common.Button), "buttons" ]);
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
                    button.setClose(true);
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
    "chlk.models.class.Class";
    "chlk.models.id.ClassId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).class = chlk.models.class || {};
        (function() {
            "use strict";
            chlk.models.class.ClassesForTopBar = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.class." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ClassesForTopBar", [ ria.__API.ArrayOf(chlk.models.class.Class), "topItems", chlk.models.id.ClassId, "selectedItemId", Boolean, "disabled" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.AnnouncementTypeEnum =             function wrapper() {
                var values = {};
                function AnnouncementTypeEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(AnnouncementTypeEnum, "chlk.models.announcement.AnnouncementTypeEnum");
                function AnnouncementTypeEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.announcement.AnnouncementTypeEnum#" + value + "]";
                    };
                }
                ria.__API.extend(AnnouncementTypeEnumImpl, AnnouncementTypeEnum);
                values[0] = AnnouncementTypeEnum.CUSTOM = new AnnouncementTypeEnumImpl(0);
                values[1] = AnnouncementTypeEnum.ANNOUNCEMENT = new AnnouncementTypeEnumImpl(1);
                values[2] = AnnouncementTypeEnum.HW = new AnnouncementTypeEnumImpl(2);
                values[3] = AnnouncementTypeEnum.ESSAY = new AnnouncementTypeEnumImpl(3);
                values[4] = AnnouncementTypeEnum.QUIZ = new AnnouncementTypeEnumImpl(4);
                values[5] = AnnouncementTypeEnum.TEST = new AnnouncementTypeEnumImpl(5);
                values[6] = AnnouncementTypeEnum.PROJECT = new AnnouncementTypeEnumImpl(6);
                values[7] = AnnouncementTypeEnum.FINAL = new AnnouncementTypeEnumImpl(7);
                values[8] = AnnouncementTypeEnum.MIDTERM = new AnnouncementTypeEnumImpl(8);
                values[9] = AnnouncementTypeEnum.BOOK_REPORT = new AnnouncementTypeEnumImpl(9);
                values[10] = AnnouncementTypeEnum.TERM_PAPER = new AnnouncementTypeEnumImpl(10);
                values[11] = AnnouncementTypeEnum.ADMIN = new AnnouncementTypeEnumImpl(11);
                return AnnouncementTypeEnum;
            }();
            chlk.models.announcement.AnnouncementType = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementType", [ [ ria.serialize.SerializeProperty("cancreate") ], Boolean, "canCreate", String, "description", Number, "id", [ ria.serialize.SerializeProperty("issystem") ], Boolean, "isSystem", String, "name" ]);
        })();
    })();
    "chlk.models.announcement.AnnouncementType";
    "chlk.models.id.ClassId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).class = chlk.models.class || {};
        (function() {
            "use strict";
            chlk.models.class.ClassForWeekMask = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.class." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ClassForWeekMask", [ [ ria.serialize.SerializeProperty("classid") ], chlk.models.id.ClassId, "classId", ria.__API.ArrayOf(Number), "mask", ria.__API.ArrayOf(chlk.models.announcement.AnnouncementType), "typesByClass" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AnnouncementAttachmentId =             function wrapper() {
                var values = {};
                function AnnouncementAttachmentId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AnnouncementAttachmentIdImpl(value);
                }
                ria.__API.identifier(AnnouncementAttachmentId, "chlk.models.id.AnnouncementAttachmentId");
                function AnnouncementAttachmentIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AnnouncementAttachmentId#" + value + "]";
                    };
                }
                ria.__API.extend(AnnouncementAttachmentIdImpl, AnnouncementAttachmentId);
                return AnnouncementAttachmentId;
            }();
        })();
    })();
    "chlk.models.id.AnnouncementAttachmentId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).attachment = chlk.models.attachment || {};
        (function() {
            "use strict";
            chlk.models.attachment.Attachment = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.attachment." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Attachment", [ chlk.models.id.AnnouncementAttachmentId, "id", [ ria.serialize.SerializeProperty("isowner") ], Boolean, "isOwner", String, "name", Number, "order", [ ria.serialize.SerializeProperty("thumbnailurl") ], String, "thumbnailUrl", Number, "type", String, "url" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).grading = chlk.models.grading || {};
        (function() {
            "use strict";
            chlk.models.grading.Mapping = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.grading." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Mapping", [ [ ria.serialize.SerializeProperty("gradingabcf") ], ria.__API.ArrayOf(Number), "gradingAbcf", [ ria.serialize.SerializeProperty("gradingcomplete") ], ria.__API.ArrayOf(Number), "gradingComplete", [ ria.serialize.SerializeProperty("gradingcheck") ], ria.__API.ArrayOf(Number), "gradingCheck" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AnnouncementId =             function wrapper() {
                var values = {};
                function AnnouncementId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AnnouncementIdImpl(value);
                }
                ria.__API.identifier(AnnouncementId, "chlk.models.id.AnnouncementId");
                function AnnouncementIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AnnouncementId#" + value + "]";
                    };
                }
                ria.__API.extend(AnnouncementIdImpl, AnnouncementId);
                return AnnouncementId;
            }();
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.StudentAnnouncementId =             function wrapper() {
                var values = {};
                function StudentAnnouncementId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new StudentAnnouncementIdImpl(value);
                }
                ria.__API.identifier(StudentAnnouncementId, "chlk.models.id.StudentAnnouncementId");
                function StudentAnnouncementIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.StudentAnnouncementId#" + value + "]";
                    };
                }
                ria.__API.extend(StudentAnnouncementIdImpl, StudentAnnouncementId);
                return StudentAnnouncementId;
            }();
        })();
    })();
    "chlk.models.people.User";
    "chlk.models.attachment.Attachment";
    "chlk.models.id.AnnouncementId";
    "chlk.models.id.StudentAnnouncementId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.StudentAnnouncement = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StudentAnnouncement", [ [ ria.serialize.SerializeProperty("announcementid") ], chlk.models.id.AnnouncementId, "announcementId", ria.__API.ArrayOf(chlk.models.attachment.Attachment), "attachments", String, "comment", Boolean, "dropped", [ ria.serialize.SerializeProperty("gradevalue") ], Number, "gradeValue", chlk.models.id.StudentAnnouncementId, "id", Number, "state", [ ria.serialize.SerializeProperty("studentinfo") ], chlk.models.people.User, "studentInfo" ]);
        })();
    })();
    "chlk.models.class.Class";
    "chlk.models.grading.Mapping";
    "chlk.models.announcement.StudentAnnouncement";
    "chlk.models.id.CourseId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.StudentAnnouncements = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StudentAnnouncements", [ [ ria.serialize.SerializeProperty("announcementtypeid") ], Number, "announcementTypeId", [ ria.serialize.SerializeProperty("announcmenttitel") ], String, "announcementTitle", [ ria.serialize.SerializeProperty("classavg") ], Number, "classAvg", [ ria.serialize.SerializeProperty("classname") ], String, "className", [ ria.serialize.SerializeProperty("courseid") ], chlk.models.id.CourseId, "courseId", [ ria.serialize.SerializeProperty("gradedstudentcount") ], Number, "gradedStudentCount", [ ria.serialize.SerializeProperty("gradingstyle") ], Number, "gradingStyle", ria.__API.ArrayOf(chlk.models.announcement.StudentAnnouncement), "items", [ ria.serialize.SerializeProperty("gradingstylemapper") ], chlk.models.grading.Mapping, "mapping", Number, "selectedIndex" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.ReminderId =             function wrapper() {
                var values = {};
                function ReminderId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new ReminderIdImpl(value);
                }
                ria.__API.identifier(ReminderId, "chlk.models.id.ReminderId");
                function ReminderIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.ReminderId#" + value + "]";
                    };
                }
                ria.__API.extend(ReminderIdImpl, ReminderId);
                return ReminderId;
            }();
        })();
    })();
    "chlk.models.id.ReminderId";
    "chlk.models.id.AnnouncementId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.Reminder = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Reminder", [ Number, "before", chlk.models.id.ReminderId, "id", chlk.models.id.AnnouncementId, "announcementId", [ ria.serialize.SerializeProperty("isowner") ], Boolean, "isOwner", [ ria.serialize.SerializeProperty("reminddate") ], chlk.models.common.ChlkDate, "remindDate", Boolean, "duplicate" ]);
        })();
    })();
    "chlk.models.people.User";
    "chlk.models.common.ChlkDate";
    "chlk.models.class.Class";
    "chlk.models.attachment.Attachment";
    "chlk.models.people.User";
    "chlk.models.announcement.StudentAnnouncements";
    "chlk.models.id.AnnouncementId";
    "chlk.models.id.ClassId";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.StudentAnnouncementId";
    "chlk.models.id.MarkingPeriodId";
    "chlk.models.announcement.Reminder";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.StateEnum =             function wrapper() {
                var values = {};
                function StateEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(StateEnum, "chlk.models.announcement.StateEnum");
                function StateEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.announcement.StateEnum#" + value + "]";
                    };
                }
                ria.__API.extend(StateEnumImpl, StateEnum);
                values[0] = StateEnum.CREATED = new StateEnumImpl(0);
                values[1] = StateEnum.SUBMITTED = new StateEnumImpl(1);
                return StateEnum;
            }();
            chlk.models.announcement.Announcement = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Announcement", [ chlk.models.id.AnnouncementId, "id", [ ria.serialize.SerializeProperty("announcementattachments") ], ria.__API.ArrayOf(chlk.models.attachment.Attachment), "announcementAttachments", [ ria.serialize.SerializeProperty("announcementreminders") ], ria.__API.ArrayOf(chlk.models.announcement.Reminder), "announcementReminders", [ ria.serialize.SerializeProperty("announcementtypeid") ], Number, "announcementTypeId", [ ria.serialize.SerializeProperty("announcementtypename") ], String, "announcementTypeName", Array, "applications", [ ria.serialize.SerializeProperty("applicationname") ], String, "applicationName", [ ria.serialize.SerializeProperty("applicationscount") ], Number, "applicationsCount", String, "attachments", [ ria.serialize.SerializeProperty("attachmentscount") ], Number, "attachmentsCount", [ ria.serialize.SerializeProperty("attachmentsummary") ], Number, "attachmentsSummary", [ ria.serialize.SerializeProperty("autogradeapps") ], ria.__API.ArrayOf(String), "autoGradeApps", Number, "avg", [ ria.serialize.SerializeProperty("avgnumeric") ], Number, "avgNumeric", [ ria.serialize.SerializeProperty("class") ], Object, "clazz", [ ria.serialize.SerializeProperty("classname") ], String, "className", String, "comment", String, "content", chlk.models.common.ChlkDate, "created", Number, "dropped", [ ria.serialize.SerializeProperty("expiresdate") ], chlk.models.common.ChlkDate, "expiresDate", String, "expiresDateColor", String, "expiresDateText", [ ria.serialize.SerializeProperty("classid") ], chlk.models.id.ClassId, "classId", Boolean, "gradable", Number, "grade", [ ria.serialize.SerializeProperty("gradesummary") ], Number, "gradesSummary", [ ria.serialize.SerializeProperty("gradingstudentscount") ], Number, "gradingStudentsCount", [ ria.serialize.SerializeProperty("gradingstyle") ], Number, "gradingStyle", [ ria.serialize.SerializeProperty("isowner") ], Boolean, "isOwner", [ ria.serialize.SerializeProperty("nongradingstudentscount") ], Number, "nonGradingStudentsCount", Number, "order", [ ria.serialize.SerializeProperty("ownerattachmentscount") ], Number, "ownerAttachmentsCount", chlk.models.people.User, "owner", [ ria.serialize.SerializeProperty("qnacount") ], Number, "qnaCount", [ ria.serialize.SerializeProperty("recipientid") ], chlk.models.id.ClassId, "recipientId", [ ria.serialize.SerializeProperty("schoolpersongender") ], String, "schoolPersonGender", [ ria.serialize.SerializeProperty("schoolpersonname") ], String, "schoolPersonName", [ ria.serialize.SerializeProperty("personid") ], chlk.models.id.SchoolPersonId, "personId", [ ria.serialize.SerializeProperty("personname") ], String, "personName", [ ria.serialize.SerializeProperty("shortcontent") ], String, "shortContent", [ ria.serialize.SerializeProperty("showgradingicon") ], Boolean, "showGradingIcon", Boolean, "starred", Number, "state", [ ria.serialize.SerializeProperty("statetyped") ], Number, "stateTyped", [ ria.serialize.SerializeProperty("studentannouncementid") ], chlk.models.id.StudentAnnouncementId, "studentAnnouncementId", [ ria.serialize.SerializeProperty("studentannouncements") ], chlk.models.announcement.StudentAnnouncements, "studentAnnouncements", [ ria.serialize.SerializeProperty("studentscount") ], Number, "studentsCount", [ ria.serialize.SerializeProperty("studentscountwithattachments") ], Number, "studentsWithAttachmentsCount", [ ria.serialize.SerializeProperty("studentscountwithoutattachments") ], Number, "studentsWithoutAttachmentsCount", String, "subject", [ ria.serialize.SerializeProperty("systemtype") ], Number, "systemType", String, "title", [ ria.serialize.SerializeProperty("wasannouncementtypegraded") ], Boolean, "wasAnnouncementTypeGraded", [ ria.serialize.SerializeProperty("wassubmittedtoadmin") ], Boolean, "wasSubmittedToAdmin", chlk.models.id.MarkingPeriodId, "markingPeriodId", String, "submitType" ]);
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.announcement.Announcement";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.AnnouncementCreate = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementCreate", [ chlk.models.announcement.Announcement, "announcement", [ ria.serialize.SerializeProperty("isdraft") ], Boolean, "isDraft", Array, "recipients" ]);
        })();
    })();
    "chlk.models.class.ClassesForTopBar";
    "chlk.models.class.ClassForWeekMask";
    "chlk.models.announcement.AnnouncementCreate";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.AnnouncementForm = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementForm", ria.__SYNTAX.EXTENDS(chlk.models.announcement.AnnouncementCreate), [ chlk.models.class.ClassesForTopBar, "topData", chlk.models.class.ClassForWeekMask, "classInfo", Number, "selectedTypeId", Array, "reminders" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.ScheduleSectionId =             function wrapper() {
                var values = {};
                function ScheduleSectionId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new ScheduleSectionIdImpl(value);
                }
                ria.__API.identifier(ScheduleSectionId, "chlk.models.id.ScheduleSectionId");
                function ScheduleSectionIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.ScheduleSectionId#" + value + "]";
                    };
                }
                ria.__API.extend(ScheduleSectionIdImpl, ScheduleSectionId);
                return ScheduleSectionId;
            }();
        })();
    })();
    "chlk.models.id.ScheduleSectionId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).schoolYear = chlk.models.schoolYear || {};
        (function() {
            "use strict";
            chlk.models.schoolYear.ScheduleSection = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.schoolYear." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ScheduleSection", [ chlk.models.id.ScheduleSectionId, "id", String, "description", [ ria.serialize.SerializeProperty("startdate") ], String, "startDate", [ ria.serialize.SerializeProperty("enddate") ], String, "endDate", Number, "weekdays", String, "name" ]);
        })();
    })();
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
    "chlk.models.common.ChlkDate";
    "chlk.models.schoolYear.ScheduleSection";
    "chlk.models.announcement.Announcement";
    "chlk.models.Popup";
    (function() {
        (((chlk = chlk || {}).models = chlk.models || {}).calendar = chlk.models.calendar || {}).announcement = chlk.models.calendar.announcement || {};
        (function() {
            "use strict";
            chlk.models.calendar.announcement.MonthItem = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("MonthItem", ria.__SYNTAX.EXTENDS(chlk.models.Popup), [ Number, "day", [ ria.serialize.SerializeProperty("iscurrentmonth") ], Boolean, "currentMonth", [ ria.serialize.SerializeProperty("issunday") ], Boolean, "sunday", chlk.models.common.ChlkDate, "date", [ ria.serialize.SerializeProperty("schedulesection") ], chlk.models.schoolYear.ScheduleSection, "scheduleSection", ria.__API.ArrayOf(chlk.models.announcement.Announcement), "announcements", ria.__API.ArrayOf(chlk.models.announcement.Announcement), "items", String, "todayClassName", String, "role", Number, "annLimit", String, "className", Array, "itemsArray" ]);
        })();
    })();
    "chlk.models.calendar.announcement.MonthItem";
    "chlk.models.class.ClassesForTopBar";
    "chlk.models.common.ChlkDate";
    (function() {
        (((chlk = chlk || {}).models = chlk.models || {}).calendar = chlk.models.calendar || {}).announcement = chlk.models.calendar.announcement || {};
        (function() {
            "use strict";
            chlk.models.calendar.announcement.Month = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Month", [ ria.__API.ArrayOf(chlk.models.calendar.announcement.MonthItem), "items", chlk.models.class.ClassesForTopBar, "topData", Number, "selectedTypeId", String, "currentTitle", chlk.models.common.ChlkDate, "nextDate", chlk.models.common.ChlkDate, "prevDate", chlk.models.common.ChlkDate, "currentDate" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AttachmentId =             function wrapper() {
                var values = {};
                function AttachmentId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AttachmentIdImpl(value);
                }
                ria.__API.identifier(AttachmentId, "chlk.models.id.AttachmentId");
                function AttachmentIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AttachmentId#" + value + "]";
                    };
                }
                ria.__API.extend(AttachmentIdImpl, AttachmentId);
                return AttachmentId;
            }();
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.announcement.AnnouncementForm";
    "chlk.models.attachment.Attachment";
    "chlk.models.class.ClassesForTopBar";
    "chlk.models.common.ChlkDate";
    "chlk.models.calendar.announcement.Month";
    "chlk.models.id.AnnouncementId";
    "chlk.models.id.AttachmentId";
    "chlk.models.id.ClassId";
    "chlk.models.id.MarkingPeriodId";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.AnnouncementAttachmentId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.AnnouncementService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number ] ], ria.async.Future, function getAnnouncements(pageIndex_) {
                return this.getPaginatedList("Feed/List.json", chlk.models.announcement.Announcement, {
                    start: pageIndex_ | 0
                });
            }, [ [ chlk.models.id.AnnouncementId, Object ] ], ria.async.Future, function uploadAttachment(announcementId, files) {
                return this.uploadFiles("AnnouncementAttachment/AddAttachment", files, chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf()
                });
            }, [ [ chlk.models.id.AnnouncementAttachmentId, Boolean, Number, Number ] ], String, function getAttachmentUri(announcementAttachmentId, needsDownload, width, heigh) {
                return this.getUrl("AnnouncementAttachment/DownloadAttachment", {
                    announcementAttachmentId: announcementAttachmentId.valueOf(),
                    needsDownload: needsDownload,
                    width: width,
                    heigh: heigh
                });
            }, [ [ chlk.models.id.AttachmentId, Object ] ], ria.async.Future, function deleteAttachment(attachmentId) {
                return this.get("AnnouncementAttachment/DeleteAttachment.json", chlk.models.announcement.Announcement, {
                    announcementAttachmentId: attachmentId.valueOf()
                });
            }, [ [ chlk.models.id.ClassId, Number ] ], ria.async.Future, function addAnnouncement(classId_, announcementTypeId_) {
                return this.get("Announcement/Create.json", chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    announcementTypeRef: announcementTypeId_
                });
            }, [ [ chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId ] ], ria.async.Future, function saveAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get("Announcement/SaveAnnouncement.json", chlk.models.announcement.AnnouncementForm, {
                    announcementId: id.valueOf(),
                    announcementTypeId: announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingPeriodId: markingPeriodId_ ? markingPeriodId_.valueOf() : null,
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            }, [ [ chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String, chlk.models.common.ChlkDate, String, String, chlk.models.id.MarkingPeriodId ] ], ria.async.Future, function submitAnnouncement(id, classId_, announcementTypeId_, subject_, content_, expiresdate_, attachments_, applications_, markingPeriodId_) {
                return this.get("Announcement/SubmitAnnouncement.json", chlk.models.announcement.AnnouncementForm, {
                    announcementId: id.valueOf(),
                    announcementTypeId: announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    markingPeriodId: markingPeriodId_ ? markingPeriodId_.valueOf() : null,
                    subject: subject_,
                    content: content_,
                    attachments: attachments_,
                    applications: applications_,
                    expiresdate: expiresdate_
                });
            }, [ [ chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId ] ], ria.async.Future, function listLast(classId, announcementTypeId, schoolPersonId) {
                return this.get("Announcement/ListLast.json", ria.__API.ArrayOf(String), {
                    classId: classId.valueOf(),
                    announcementTypeId: announcementTypeId,
                    personId: schoolPersonId.valueOf()
                });
            }, [ [ chlk.models.id.AnnouncementId ] ], ria.async.Future, function editAnnouncement(id) {
                return this.get("Announcement/Edit.json", chlk.models.announcement.AnnouncementForm, {
                    announcementId: id.valueOf()
                });
            }, [ [ chlk.models.id.AnnouncementId ] ], ria.async.Future, function deleteAnnouncement(id) {
                return this.post("Announcement/Delete.json", chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            }, [ [ chlk.models.id.SchoolPersonId ] ], ria.async.Future, function deleteDrafts(id) {
                return this.post("Announcement/DeleteDrafts.json", chlk.models.announcement.Announcement, {
                    personId: id.valueOf()
                });
            }, [ [ chlk.models.id.AnnouncementId ] ], ria.async.Future, function getAnnouncement(id) {
                return this.get("Announcement/Read.json", chlk.models.announcement.Announcement, {
                    announcementId: id.valueOf()
                });
            } ]);
        })();
    })();
    "chlk.models.class.Class";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).class = chlk.models.class || {};
        (function() {
            "use strict";
            chlk.models.class.ClassForTopBar = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.class." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ClassForTopBar", ria.__SYNTAX.EXTENDS(chlk.models.class.Class), [ String, "controller", String, "action", Array, "params", Boolean, "pressed", Number, "index", Boolean, "disabled" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.class.ClassForTopBar";
    "chlk.models.class.ClassForWeekMask";
    "chlk.models.id.ClassId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.ClassService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ClassService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ ria.__API.ArrayOf(chlk.models.class.ClassForTopBar), "classesToFilter", ria.__API.ArrayOf(chlk.models.class.ClassForTopBar), "classesToFilterWithAll", [ [ Boolean ] ], Array, function getClassesForTopBar(withAll_) {
                var res = this.getClassesToFilter(), res1 = this.getClassesToFilterWithAll();
                if (res) return withAll_ ? res1 : res;
                res = new ria.serialize.JsonSerializer().deserialize(window.classesToFilter, ria.__API.ArrayOf(chlk.models.class.ClassForTopBar));
                this.setClassesToFilter(res);
                var classesToFilterWithAll = window.classesToFilter.slice();
                classesToFilterWithAll.unshift({
                    name: "All",
                    description: "All",
                    id: ""
                });
                res1 = new ria.serialize.JsonSerializer().deserialize(classesToFilterWithAll, ria.__API.ArrayOf(chlk.models.class.ClassForTopBar));
                this.setClassesToFilterWithAll(res1);
                return withAll_ ? res1 : res;
            }, [ [ chlk.models.id.ClassId ] ], chlk.models.class.ClassForWeekMask, function getClassAnnouncementInfo(id) {
                var res = window.classesInfo[id.valueOf()];
                res = new ria.serialize.JsonSerializer().deserialize(res, chlk.models.class.ClassForWeekMask);
                return res;
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.people.User";
    "chlk.models.id.SchoolPersonId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.PersonService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("PersonService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.SchoolPersonId, Object ] ], ria.async.Future, function uploadPicture(personId, files) {
                return this.uploadFiles("Person/UploadPicture", files, Boolean, {
                    personId: personId.valueOf()
                });
            }, [ [ chlk.models.id.SchoolPersonId, String ] ], ria.async.Future, function changePassword(personId, password) {
                return this.get("Person/ReChangePassword", Boolean, {
                    id: personId.valueOf(),
                    newPassword: password
                });
            }, [ [ chlk.models.id.SchoolPersonId, Number ] ], String, function getPictureURL(personId, size_) {
                var url = window.azurePictureUrl + personId.valueOf();
                return size_ ? url + "-" + size_ + "x" + size_ : url;
            }, [ [ String ] ], ria.async.Future, function getPersons(filter_) {
                return this.getPaginatedList("Person/GetPersons.json", chlk.models.people.User, {
                    filter: filter_
                }).then(function(model) {
                    return model.getItems();
                });
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.announcement.StudentAnnouncement";
    "chlk.models.id.StudentAnnouncementId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.GradingService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("GradingService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.StudentAnnouncementId, Number, String, Boolean ] ], ria.async.Future, function updateItem(studentAnnouncementId, gradeValue, comment, dropped) {
                return this.get("Grading/UpdateItem", chlk.models.announcement.StudentAnnouncement, {
                    studentAnnouncementId: studentAnnouncementId.valueOf(),
                    gradeValue: gradeValue,
                    comment: comment,
                    dropped: dropped
                });
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.id.AnnouncementId";
    "chlk.models.id.ReminderId";
    "chlk.models.announcement.Announcement";
    "chlk.models.announcement.Reminder";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.AnnouncementReminderService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementReminderService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.AnnouncementId, Number ] ], ria.async.Future, function addReminder(announcementId, before) {
                return this.get("AnnouncementReminder/AddReminder.json", chlk.models.announcement.Announcement, {
                    announcementId: announcementId.valueOf(),
                    before: before
                });
            }, [ [ chlk.models.id.ReminderId, Number ] ], ria.async.Future, function editReminder(announcementReminderId, before) {
                return this.get("AnnouncementReminder/EditReminder.json", chlk.models.announcement.Reminder, {
                    announcementReminderId: announcementReminderId.valueOf(),
                    before: before
                });
            }, [ [ chlk.models.id.ReminderId ] ], ria.async.Future, function deleteReminder(announcementReminderId) {
                return this.get("AnnouncementReminder/DeleteReminder.json", chlk.models.announcement.Announcement, {
                    announcementReminderId: announcementReminderId.valueOf()
                });
            } ]);
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
    __ASSETS._lygjxl0m1ymygb9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="announcement-form loader-container">');
        topData = self.getTopData();
        announcement = self.getAnnouncement();
        topItems = topData.getTopItems();
        classId = announcement.getClassId() ? announcement.getClassId().valueOf() : null;
        typeId = self.getSelectedTypeId();
        attachments = [];
        announcement.getAnnouncementAttachments().forEach(function(attachment) {
            attachments.push(attachment.__PROTECTED);
        });
        classes = self.getClassInfo() ? self.getClassInfo().getTypesByClass() : [];
        buf.push('<div class="loader"></div>');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.LeftRightToolbar_mixin.call({
                    buf: buf,
                    attributes: {
                        selectedItemId: topData.getSelectedItemId(),
                        pressAfterClick: !announcement.getState(),
                        "class": "classes-bar"
                    },
                    escaped: {
                        selectedItemId: true,
                        pressAfterClick: true
                    }
                }, topItems, chlk.templates.class.TopBar);
                console.info(classId, "class = " + (classId ? "" : "x-hidden"));
                buf.push("<div" + jade.attrs({
                    "class": "action-bar" + " " + "not-transparent" + " " + "buttons" + " " + (classId ? "" : "x-hidden")
                }, {
                    "class": true
                }) + '><div class="container panel-bg">');
                (function() {
                    var $$obj = classes;
                    if ("number" == typeof $$obj.length) {
                        for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                            var item = $$obj[$index];
                            buf.push("<BUTTON" + jade.attrs({
                                typeId: item.getId(),
                                typeName: item.getName(),
                                type: announcement.getState() ? "button" : "submit",
                                name: "submitType",
                                value: "save",
                                "class": "validate-skip " + (typeId == item.getId() ? "pressed" : "") + " " + "action-button"
                            }, {
                                typeId: true,
                                typeName: true,
                                type: true,
                                name: true,
                                value: true
                            }) + ">" + jade.escape(null == (jade.interp = item.getDescription()) ? "" : jade.interp) + "</BUTTON>");
                        }
                    } else {
                        var $$l = 0;
                        for (var $index in $$obj) {
                            $$l++;
                            var item = $$obj[$index];
                            buf.push("<BUTTON" + jade.attrs({
                                typeId: item.getId(),
                                typeName: item.getName(),
                                type: announcement.getState() ? "button" : "submit",
                                name: "submitType",
                                value: "save",
                                "class": "validate-skip " + (typeId == item.getId() ? "pressed" : "") + " " + "action-button"
                            }, {
                                typeId: true,
                                typeName: true,
                                type: true,
                                name: true,
                                value: true
                            }) + ">" + jade.escape(null == (jade.interp = item.getDescription()) ? "" : jade.interp) + "</BUTTON>");
                        }
                    }
                }).call(this);
                buf.push("</div></div><div" + jade.attrs({
                    "class": "ann-form-container" + " " + (classId && typeId ? "" : "x-hidden")
                }, {
                    "class": true
                }) + ">");
                typesEnum = chlk.models.announcement.AnnouncementTypeEnum;
                isAnnouncement = announcement.getAnnouncementTypeId() == typesEnum.ANNOUNCEMENT.valueOf();
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", announcement.getId().valueOf());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "announcementtypeid", announcement.getAnnouncementTypeId());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "statetyped", announcement.getStateTyped());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "systemtype", announcement.getSystemType());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "personid", announcement.getPersonId().valueOf());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "gradingstyle", announcement.getGradingStyle());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "attachments", announcement.getAttachments());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "markingperiodid", announcement.getCreated());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "state", announcement.getState());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "classid", classId);
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "announcementtypename", announcement.getAnnouncementTypeName());
                buf.push('<div class="left-block">');
                if (isAnnouncement) {
                    buf.push('<TEXTAREA id="subject" name="subject" placeholder="Subject">' + jade.escape(null == (jade.interp = announcement.getSubject()) ? "" : jade.interp) + "</TEXTAREA>");
                }
                jade.globals.TextArea_mixin.call({
                    buf: buf,
                    attributes: {
                        id: "content",
                        name: "content",
                        placeholder: isAnnouncement ? "Message" : "Assignment"
                    },
                    escaped: {
                        id: true,
                        name: true,
                        placeholder: true
                    }
                });
                buf.push('<div class="drop-down-container"></div><input type="submit" id="list-last-button" value="listLast" name="submitType" class="x-hidden validate-skip"/><input type="submit" id="save-form-button" value="saveNoUpdate" name="submitType" class="x-hidden validate-skip"/></div><div class="right-block"><div class="date-picker-container">');
                jade.globals.DatePicker_mixin.call({
                    buf: buf,
                    attributes: {
                        "data-options": {
                            showOtherMonths: true,
                            selectOtherMonths: true
                        },
                        id: "expiresdate",
                        "class": "validate[required]"
                    },
                    escaped: {
                        "data-options": true,
                        "class": true
                    }
                }, "expiresdate", announcement.getExpiresDate());
                buf.push('</div><SPAN id="reminders-button" class="small-gray-link">' + jade.escape(null == (jade.interp = Msg.Reminder(true)) ? "" : jade.interp) + '</SPAN><div class="reminders-container x-hidden"><div class="reminders">');
                var reminders = announcement.getAnnouncementReminders() || [];
                (function() {
                    var $$obj = reminders;
                    if ("number" == typeof $$obj.length) {
                        for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                            var reminder = $$obj[$index];
                            jade.globals.RenderWith_mixin.call({
                                buf: buf
                            }, reminder, chlk.templates.announcement.AnnouncementReminder);
                        }
                    } else {
                        var $$l = 0;
                        for (var $index in $$obj) {
                            $$l++;
                            var reminder = $$obj[$index];
                            jade.globals.RenderWith_mixin.call({
                                buf: buf
                            }, reminder, chlk.templates.announcement.AnnouncementReminder);
                        }
                    }
                }).call(this);
                buf.push('</div><div class="reminder new-reminder x-hidden"><input class="reminder-input"/>');
                jade.globals.Select_mixin.call({
                    buf: buf,
                    block: function() {
                        jade.globals.Option_mixin.call({
                            buf: buf
                        }, 7, Msg.Week(true));
                        jade.globals.Option_mixin.call({
                            buf: buf
                        }, 1, Msg.Day(true), true);
                    },
                    attributes: {
                        "class": "reminder-select"
                    },
                    escaped: {}
                });
                buf.push('<SPAN class="remove-btn remove-reminder"></SPAN></div><div id="add-reminder" class="chlk-add opacity-button">' + jade.escape(null == (jade.interp = Msg.Add_reminder) ? "" : jade.interp) + '</div></div></div><div class="attachments-and-applications">');
                jade.globals.RenderWith_mixin.call({
                    buf: buf,
                    attributes: {
                        needButtons: true,
                        needDeleteButton: true
                    },
                    escaped: {
                        needButtons: true,
                        needDeleteButton: true
                    }
                }, announcement, chlk.templates.announcement.Announcement);
                buf.push('</div><div class="bottom-block">');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push(jade.escape(null == (jade.interp = announcement.getState() ? Msg.Save : Msg.Submit) ? "" : jade.interp));
                    },
                    attributes: {
                        type: "submit",
                        name: "submitType",
                        value: "submit",
                        "class": "special-button" + " " + "blue-button" + " " + "submit-btn"
                    },
                    escaped: {
                        type: true,
                        name: true,
                        value: true
                    }
                });
                if (announcement.getState()) {
                    jade.globals.ActionLink_mixin.call({
                        buf: buf,
                        block: function() {
                            jade.globals.Button_mixin.call({
                                buf: buf,
                                attributes: {
                                    type: "submit",
                                    name: "submitType",
                                    value: "submit",
                                    "class": "special-button" + " " + "red-button" + " " + "submit-btn"
                                },
                                escaped: {
                                    type: true,
                                    name: true,
                                    value: true
                                }
                            });
                        }
                    }, "announcement", "delete", announcement.getId().valueOf());
                } else {
                    jade.globals.ActionLink_mixin.call({
                        buf: buf,
                        attributes: {
                            "class": "grey-link" + " " + "not-blue"
                        },
                        escaped: {}
                    }, "announcement", "discard", announcement.getPersonId().valueOf());
                }
                buf.push("</div></div><div" + jade.attrs({
                    "class": "choose-type" + " " + (classId && !typeId ? "" : "x-hidden")
                }, {
                    "class": true
                }) + "></div><div" + jade.attrs({
                    "class": "choose-class" + " " + (classId ? "x-hidden" : "")
                }, {
                    "class": true
                }) + "></div>");
            },
            attributes: {
                onlySubmitValidate: true
            },
            escaped: {
                onlySubmitValidate: true
            }
        }, "announcement", "save");
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "announcementId", announcement.getId());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "before");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "duplicate", false);
            },
            attributes: {
                "data-reminders": self.getReminders(),
                id: "add-edit-reminder"
            },
            escaped: {
                "data-reminders": true
            }
        }, "announcement", "editAddReminder");
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.announcement.AnnouncementForm";
    "chlk.models.class.ClassesForTopBar";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).announcement = chlk.templates.announcement || {};
        (function() {
            chlk.templates.announcement.AnnouncementForm = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_lygjxl0m1ymygb9") ], [ ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm) ], "AnnouncementForm", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.announcement.Announcement, "announcement", [ ria.templates.ModelPropertyBind ], chlk.models.class.ClassesForTopBar, "topData", [ ria.templates.ModelPropertyBind ], chlk.models.class.ClassForWeekMask, "classInfo", [ ria.templates.ModelPropertyBind ], Number, "selectedTypeId", [ ria.templates.ModelPropertyBind ], Boolean, "isDraft", [ ria.templates.ModelPropertyBind ], Array, "recipients", [ ria.templates.ModelPropertyBind ], Array, "reminders" ]);
        })();
    })();
    __ASSETS._vtpl925nr48cl3di = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        attachments = self.getAnnouncementAttachments() || [];
        if (self.needButtons) {
            buf.push('<div id="attach-file-area" class="announcement-item attach-file-area"><button id="add-application" class="simple-gray-button">Attach App</button><button class="simple-gray-button file">Attach File');
            jade.globals.FileUpload_mixin.call({
                buf: buf,
                attributes: {
                    id: "add-attachment",
                    "class": "simple-gray-button"
                },
                escaped: {}
            }, "announcement", "uploadAttachment", [ self.getId().valueOf() ]);
            buf.push("</button></div>");
        }
        (function() {
            var $$obj = attachments;
            if ("number" == typeof $$obj.length) {
                for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                    var attachment = $$obj[i];
                    buf.push("<div" + jade.attrs({
                        "data-id": attachment.getId(),
                        "data-url": "viewurl",
                        "class": "announcement-item" + " " + "attachment"
                    }, {
                        "data-id": true,
                        "data-url": true
                    }) + ">");
                    if (self.needDeleteButton) {
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            attributes: {
                                "class": "close-btn"
                            },
                            escaped: {}
                        }, "announcement", "deleteAttachment", attachment.getId().valueOf());
                    }
                    jade.globals.LoadingImage_mixin.call({
                        buf: buf,
                        attributes: {
                            src: attachment.getThumbnailUrl(),
                            id: "loading-image-" + i
                        },
                        escaped: {
                            src: true,
                            id: true
                        }
                    });
                    buf.push('<div class="title"><div>' + jade.escape(null == (jade.interp = attachment.getName()) ? "" : jade.interp) + "</div></div></div>");
                }
            } else {
                var $$l = 0;
                for (var i in $$obj) {
                    $$l++;
                    var attachment = $$obj[i];
                    buf.push("<div" + jade.attrs({
                        "data-id": attachment.getId(),
                        "data-url": "viewurl",
                        "class": "announcement-item" + " " + "attachment"
                    }, {
                        "data-id": true,
                        "data-url": true
                    }) + ">");
                    if (self.needDeleteButton) {
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            attributes: {
                                "class": "close-btn"
                            },
                            escaped: {}
                        }, "announcement", "deleteAttachment", attachment.getId().valueOf());
                    }
                    jade.globals.LoadingImage_mixin.call({
                        buf: buf,
                        attributes: {
                            src: attachment.getThumbnailUrl(),
                            id: "loading-image-" + i
                        },
                        escaped: {
                            src: true,
                            id: true
                        }
                    });
                    buf.push('<div class="title"><div>' + jade.escape(null == (jade.interp = attachment.getName()) ? "" : jade.interp) + "</div></div></div>");
                }
            }
        }).call(this);
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.announcement.Announcement";
    "chlk.models.id.AnnouncementId";
    "chlk.models.id.ClassId";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.StudentAnnouncementId";
    "chlk.models.id.MarkingPeriodId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).announcement = chlk.templates.announcement || {};
        (function() {
            chlk.templates.announcement.Announcement = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_vtpl925nr48cl3di") ], [ ria.templates.ModelBind(chlk.models.announcement.Announcement) ], "Announcement", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.AnnouncementId, "id", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "created", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.attachment.Attachment), "announcementAttachments", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.announcement.Reminder), "announcementReminders", [ ria.templates.ModelPropertyBind ], Number, "announcementTypeId", [ ria.templates.ModelPropertyBind ], String, "announcementTypeName", [ ria.templates.ModelPropertyBind ], Array, "applications", [ ria.templates.ModelPropertyBind ], String, "applicationName", [ ria.templates.ModelPropertyBind ], Number, "applicationsCount", [ ria.templates.ModelPropertyBind ], String, "attachments", [ ria.templates.ModelPropertyBind ], Number, "attachmentsCount", [ ria.templates.ModelPropertyBind ], Number, "attachmentsSummary", [ ria.templates.ModelPropertyBind ], Number, "avg", [ ria.templates.ModelPropertyBind ], Number, "avgNumeric", [ ria.templates.ModelPropertyBind ], Object, "clazz", [ ria.templates.ModelPropertyBind ], String, "comment", [ ria.templates.ModelPropertyBind ], String, "content", [ ria.templates.ModelPropertyBind ], chlk.models.id.ClassId, "classId", [ ria.templates.ModelPropertyBind ], Number, "dropped", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "expiresDate", [ ria.templates.ModelPropertyBind ], String, "expiresDateColor", [ ria.templates.ModelPropertyBind ], String, "expiresDateText", [ ria.templates.ModelPropertyBind ], Boolean, "gradable", [ ria.templates.ModelPropertyBind ], Number, "grade", [ ria.templates.ModelPropertyBind ], Number, "gradesSummary", [ ria.templates.ModelPropertyBind ], Number, "gradingStudentsCount", [ ria.templates.ModelPropertyBind ], Number, "gradingStyle", [ ria.templates.ModelPropertyBind ], Boolean, "isOwner", [ ria.templates.ModelPropertyBind ], Number, "nonGradingStudentsCount", [ ria.templates.ModelPropertyBind ], Number, "order", [ ria.templates.ModelPropertyBind ], chlk.models.people.User, "owner", [ ria.templates.ModelPropertyBind ], Number, "ownerAttachmentsCount", [ ria.templates.ModelPropertyBind ], Number, "qnaCount", [ ria.templates.ModelPropertyBind ], chlk.models.id.ClassId, "recipientId", [ ria.templates.ModelPropertyBind ], String, "schoolPersonGender", [ ria.templates.ModelPropertyBind ], String, "schoolPersonName", [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolPersonId, "personId", [ ria.templates.ModelPropertyBind ], String, "shortContent", [ ria.templates.ModelPropertyBind ], Boolean, "showGradingIcon", [ ria.templates.ModelPropertyBind ], Boolean, "starred", [ ria.templates.ModelPropertyBind ], Number, "state", [ ria.templates.ModelPropertyBind ], Number, "stateTyped", [ ria.templates.ModelPropertyBind ], chlk.models.id.StudentAnnouncementId, "studentAnnouncementId", [ ria.templates.ModelPropertyBind ], chlk.models.announcement.StudentAnnouncements, "studentAnnouncements", [ ria.templates.ModelPropertyBind ], Number, "studentsCount", [ ria.templates.ModelPropertyBind ], Number, "studentsWithAttachmentsCount", [ ria.templates.ModelPropertyBind ], Number, "studentsWithoutAttachmentsCount", [ ria.templates.ModelPropertyBind ], String, "subject", [ ria.templates.ModelPropertyBind ], Number, "systemType", [ ria.templates.ModelPropertyBind ], String, "title", [ ria.templates.ModelPropertyBind ], Boolean, "wasAnnouncementTypeGraded", [ ria.templates.ModelPropertyBind ], String, "submitType", Boolean, "needButtons", Boolean, "needDeleteButton" ]);
        })();
    })();
    __ASSETS._vcnl4b78rfk9y66r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        var before = self.getBefore();
        var isWeek = before && before % 7 == 0;
        var inputValue = isWeek ? before / 7 : before;
        buf.push("<div" + jade.attrs({
            "data-id": self.getId().valueOf(),
            "data-value": before,
            "class": "reminder"
        }, {
            "data-id": true,
            "data-value": true
        }) + "><input" + jade.attrs({
            value: inputValue,
            "class": "reminder-input"
        }, {
            value: true
        }) + "/>");
        jade.globals.Select_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Option_mixin.call({
                    buf: buf
                }, 7, Msg.Week(true), isWeek);
                jade.globals.Option_mixin.call({
                    buf: buf
                }, 1, Msg.Day(true), !isWeek);
            },
            attributes: {
                "class": "reminder-select"
            },
            escaped: {}
        });
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "remove-btn" + " " + "remove-reminder"
            },
            escaped: {}
        }, "announcement", "removeReminder", self.getId());
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.announcement.Reminder";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).announcement = chlk.templates.announcement || {};
        (function() {
            chlk.templates.announcement.AnnouncementReminder = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_vcnl4b78rfk9y66r") ], [ ria.templates.ModelBind(chlk.models.announcement.Reminder) ], "AnnouncementReminder", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Number, "before", [ ria.templates.ModelPropertyBind ], chlk.models.id.ReminderId, "id", [ ria.templates.ModelPropertyBind ], chlk.models.id.AnnouncementId, "announcementId", [ ria.templates.ModelPropertyBind ], Boolean, "isOwner", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "remindDate", [ ria.templates.ModelPropertyBind ], Boolean, "duplicate" ]);
        })();
    })();
    __ASSETS._wjoozfm6ejulq5mi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        length = self.getItems().length;
        if (length > 1) {
            buf.push("<div" + jade.attrs({
                "data-length": length,
                "class": "new-item-dropdown"
            }, {
                "data-length": true
            }) + "><h1>Recent " + jade.escape((jade.interp = self.getAnnouncementTypeName()) == null ? "" : jade.interp) + " Assignments</h1>");
            (function() {
                var $$obj = self.getItems();
                if ("number" == typeof $$obj.length) {
                    for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                        var item = $$obj[i];
                        buf.push("<div" + jade.attrs({
                            "data-value": item,
                            "data-index": i,
                            "class": (i ? "" : "current") + " " + "autofill-item"
                        }, {
                            "data-value": true,
                            "data-index": true
                        }) + ">" + jade.escape(null == (jade.interp = item) ? "" : jade.interp) + "</div>");
                    }
                } else {
                    var $$l = 0;
                    for (var i in $$obj) {
                        $$l++;
                        var item = $$obj[i];
                        buf.push("<div" + jade.attrs({
                            "data-value": item,
                            "data-index": i,
                            "class": (i ? "" : "current") + " " + "autofill-item"
                        }, {
                            "data-value": true,
                            "data-index": true
                        }) + ">" + jade.escape(null == (jade.interp = item) ? "" : jade.interp) + "</div>");
                    }
                }
            }).call(this);
            buf.push("</div>");
        }
        return buf.join("");
    };
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.Array = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Array", [ Array, "items" ]);
        })();
    })();
    "chlk.models.common.Array";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.LastMessages = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("LastMessages", ria.__SYNTAX.EXTENDS(chlk.models.common.Array), [ String, "announcementTypeName" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.announcement.LastMessages";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).announcement = chlk.templates.announcement || {};
        (function() {
            chlk.templates.announcement.LastMessages = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_wjoozfm6ejulq5mi") ], [ ria.templates.ModelBind(chlk.models.announcement.LastMessages) ], "LastMessages", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Array, "items", [ ria.templates.ModelPropertyBind ], String, "announcementTypeName" ]);
        })();
    })();
    __ASSETS._tvzw3pax94fgvi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        if (self.getController()) {
            var params = self.getParams();
            buf.push("<!--params = (params && params.length ? params.join('/') + '/' : '');-->");
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                block: function() {
                    buf.push('<div class="img-wrapper"><img href="http://local.chalkable.com:8080/chalkable/Course/GetIcon?courseInfoId=10"/></div><SPAN class="text-container">' + jade.escape(null == (jade.interp = self.getName()) ? "" : jade.interp) + "</SPAN>");
                },
                attributes: {
                    "class": (self.isPressed() ? "pressed " : "") + " " + "item" + " " + "class-button" + " " + "button-link"
                },
                escaped: {}
            }, self.getController(), self.getAction(), params, self.getId());
        } else {
            buf.push("<BUTTON" + jade.attrs({
                classId: self.getId(),
                index: self.getIndex(),
                type: self.isDisabled() ? "button" : "submit",
                name: "submitType",
                value: "save",
                "class": (self.isPressed() ? "pressed " : "") + " " + "item" + " " + "class-button" + " " + "validate-skip"
            }, {
                classId: true,
                index: true,
                type: true,
                name: true,
                value: true
            }) + '><div class="img-wrapper"><img href="http://local.chalkable.com:8080/chalkable/Course/GetIcon?courseInfoId=10"/></div><SPAN class="text-container">' + jade.escape(null == (jade.interp = self.getName()) ? "" : jade.interp) + "</SPAN></BUTTON>");
        }
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.class.ClassForTopBar";
    "chlk.models.id.ClassId";
    "chlk.models.id.CourseId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).class = chlk.templates.class || {};
        (function() {
            chlk.templates.class.TopBar = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.class." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_tvzw3pax94fgvi") ], [ ria.templates.ModelBind(chlk.models.class.ClassForTopBar) ], "TopBar", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.ClassId, "id", [ ria.templates.ModelPropertyBind ], String, "name", [ ria.templates.ModelPropertyBind ], chlk.models.course.Course, "course", [ ria.templates.ModelPropertyBind ], Boolean, "pressed", [ ria.templates.ModelPropertyBind ], Boolean, "disabled", [ ria.templates.ModelPropertyBind ], Number, "index", [ ria.templates.ModelPropertyBind ], String, "controller", [ ria.templates.ModelPropertyBind ], String, "action", [ ria.templates.ModelPropertyBind ], Array, "params" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.announcement.AnnouncementForm";
    "chlk.templates.announcement.Announcement";
    "chlk.templates.announcement.AnnouncementReminder";
    "chlk.templates.announcement.LastMessages";
    "chlk.templates.class.TopBar";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).announcement = chlk.activities.announcement || {};
        (function() {
            var handler;
            chlk.activities.announcement.AnnouncementFormPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementForm) ], [ ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementReminder, "", ".reminders", ria.mvc.PartialUpdateRuleActions.Append) ], [ ria.mvc.PartialUpdateRule(chlk.templates.announcement.Announcement, "", ".attachments-and-applications", ria.mvc.PartialUpdateRuleActions.Replace) ], [ ria.mvc.PartialUpdateRule(chlk.templates.announcement.AnnouncementForm, "", null, ria.mvc.PartialUpdateRuleActions.Replace) ], [ ria.mvc.PartialUpdateRule(chlk.templates.announcement.LastMessages, "", ".drop-down-container", ria.mvc.PartialUpdateRuleActions.Replace) ], [ chlk.activities.lib.PageClass("new-item") ], "AnnouncementFormPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("click", ".class-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function classClick(node, event) {
                var classId = node.getAttr("classId");
                this.dom.find("input[name=classid]").setValue(classId);
            }, [ ria.mvc.DomEventBind("click", "#add-reminder") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function addReminderClick(node, event) {
                this.dom.find(".new-reminder").removeClass("x-hidden");
            }, [ ria.mvc.DomEventBind("click", "#reminders-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function showRemindersClick(node, event) {
                this.dom.find(".reminders-container").toggleClass("x-hidden");
            }, [ ria.mvc.DomEventBind("click", ".action-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function typeClick(node, event) {
                var typeId = node.getAttr("typeId");
                var typeName = node.getAttr("typeName");
                this.dom.find("input[name=announcementtypeid]").setValue(typeId);
                this.dom.find("input[name=announcementtypename]").setValue(typeName);
            }, [ ria.mvc.DomEventBind("click", "form") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function formClick(node, event) {}, [ ria.mvc.DomEventBind("click", ".remove-reminder") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function removeReminderClick(node, event) {
                setTimeout(function() {
                    node.parent(".reminder").remove();
                }, 10);
            }, [ ria.mvc.DomEventBind("change", ".reminder-input") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function inputChange(node, event) {
                this.addEditReminder(node);
            }, [ ria.mvc.DomEventBind("focus keydown keyup", "#content") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function showDropDown(node, event) {
                if (this.dom.find("[name=announcementtypeid]").getValue() != chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf()) {
                    var dropDown = this.dom.find(".new-item-dropdown");
                    if (!node.getValue() && !dropDown.is(":visible")) {
                        this.dom.find("#list-last-button").triggerEvent("click");
                    } else {
                        if (event.type == "keydown") {
                            var isUp = event.keyCode == ria.dom.Keys.UP.valueOf(), isDown = event.keyCode == ria.dom.Keys.DOWN.valueOf();
                            if (dropDown.is(":visible") && (isUp || isDown)) {
                                var current = dropDown.find(".current");
                                var currentIndex = parseInt(current.getData("index"), 10), nextItem;
                                var moveUp = currentIndex > 0 && isUp;
                                var moveDown = currentIndex < dropDown.getData("length") - 1 && isDown;
                                if (moveUp || moveDown) {
                                    current.removeClass("current");
                                    if (moveUp) {
                                        currentIndex--;
                                    } else {
                                        currentIndex++;
                                    }
                                    nextItem = dropDown.find('.autofill-item[data-index="' + currentIndex + '"]').addClass("current");
                                    node.setValue(nextItem.getData("value"));
                                }
                            } else {
                                dropDown.addClass("x-hidden");
                                if (event.keyCode == ria.dom.Keys.ENTER.valueOf()) return false;
                            }
                        }
                    }
                }
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                if (model.getClass() == chlk.models.announcement.LastMessages) {
                    this.dom.find("#content").triggerEvent("focus");
                }
                if (model.getClass() == chlk.models.announcement.Reminder) {
                    var parent = this.dom.find(".new-reminder");
                    var input = parent.find(".reminder-input");
                    var select = parent.find(".reminder-select");
                    input.setAttr("disabled", false).setValue("");
                    select.setAttr("disabled", false).find("option[value=1]").setAttr("selected", true).find("option[value=7]").setAttr("selected", false);
                    parent.find(".remove-reminder").removeClass("x-hidden");
                    jQuery(select.valueOf()).trigger("liszt:updated");
                    parent.addClass("x-hidden");
                }
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onStart_() {
                BASE();
                var that = this;
                handler = function(event, node) {
                    var target = new ria.dom.Dom(event.target);
                    if (!target.parent(".drop-down-container").exists() && target.getAttr("name") != "content") that.dom.find(".new-item-dropdown").addClass("x-hidden");
                };
                jQuery(this.dom.valueOf()).on("change", ".reminder-select", function() {
                    that.addEditReminder(new ria.dom.Dom(this));
                });
                new ria.dom.Dom().on("click", handler);
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onStop_() {
                new ria.dom.Dom("#save-form-button").triggerEvent("click");
                new ria.dom.Dom().off("click", handler);
                BASE();
            }, [ [ ria.dom.Dom ] ], ria.__SYNTAX.Modifiers.VOID, function addEditReminder(node) {
                var parent = node.parent(".reminder");
                var input = parent.find(".reminder-input");
                var inputValue = input.getValue();
                if (inputValue) {
                    var select = parent.find(".reminder-select");
                    var id = parent.getData("id");
                    var form = this.dom.find("#add-edit-reminder");
                    var reminders = form.getData("reminders"), isDuplicate = false;
                    var before = parseInt(select.getValue(), 10) * parseInt(inputValue, 10);
                    isDuplicate = reminders.indexOf(before) > -1;
                    if (!isDuplicate) {
                        reminders.push(before);
                        form.setData("reminders", reminders);
                        if (!id) {
                            input.setAttr("disabled", true);
                            select.setAttr("disabled", true);
                            parent.find(".remove-reminder").addClass("x-hidden");
                            jQuery(select.valueOf()).trigger("liszt:updated");
                        }
                    }
                    form.find("[name=duplicate]").setValue(isDuplicate);
                    form.find("[name=id]").setValue(id || "");
                    form.find("[name=before]").setValue(before);
                    form.triggerEvent("submit");
                }
            } ]);
        })();
    })();
    __ASSETS._9fyvtcq9r2fkcsor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="announcement-view not-transparent">');
        var isNotStudent = true;
        var isReadonly = false;
        var studentAnnouncements = false;
        var studentAnnouncementsInfo = self.getStudentAnnouncements();
        var selectedIndex = studentAnnouncementsInfo ? studentAnnouncementsInfo.getSelectedIndex() : 0;
        var applications = self.getApplications();
        var applicationsInGradeView = applications.filter(function(item) {
            return item.applicationviewdata.showingradeview;
        });
        if (studentAnnouncementsInfo) {
            var studentAnnouncements = studentAnnouncementsInfo.getItems();
            var gradingStyle = studentAnnouncementsInfo.getGradingStyle();
            var gradingMapping = studentAnnouncementsInfo.getMapping();
            var getGrade = function(value) {
                return GradingStyler.getLetterByGrade(value, gradingMapping, gradingStyle);
            };
            var getGradeNumber = function(value) {
                return GradingStyler.getGradeNumberValue(value, gradingMapping, gradingStyle);
            };
        }
        buf.push('<div class="feed-item">');
        if (isNotStudent) {
            buf.push('<div class="grades"></div>');
        }
        buf.push('<div class="bullet">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "star"
            },
            escaped: {}
        }, "announcement", "star", self.getId());
        buf.push('<div class="item-type"><div class="attachment"><div' + jade.attrs({
            "class": "announcements-type-" + self.getAnnouncementTypeId() + " " + "icon" + " " + "blue" + " " + "icon"
        }, {}) + '></div><div class="icon black"></div><h2 class="animated-header">' + jade.escape(null == (jade.interp = self.getSubject()) ? "" : jade.interp) + '</h2><p class="animated">' + jade.escape(null == (jade.interp = self.getShortContent()) ? "" : jade.interp) + '</p><div class="animated">' + jade.escape(null == (jade.interp = self.getExpiresDate().toString()) ? "" : jade.interp) + '</div><div class="attachment-icon"></div><div class="apps-icon"></div></div></div></div></div><div class="panel-bg"><div class="action-bar buttons"><div class="container"><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "back-button"
            },
            escaped: {}
        }, "feed", "list");
        buf.push('</div><div class="right">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Edit");
            },
            attributes: {
                "class": "edit-button"
            },
            escaped: {}
        }, "announcement", "edit", self.getId());
        buf.push('</div></div></div><div class="announcement-details-panel"><div class="details-container silver-panel"><div class="left-block">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("<img" + jade.attrs({
                    src: self.getOwner().getPictureUrl(),
                    "class": "avatar" + " " + "avatar-47"
                }, {
                    src: true
                }) + "/>");
            },
            attributes: {
                "class": "image-container"
            },
            escaped: {}
        }, "feed", "list");
        buf.push('</div><div class="right-block"><h2' + jade.attrs({
            "class": self.getExpiresDateColor()
        }, {
            "class": true
        }) + ">" + jade.escape(null == (jade.interp = self.getExpiresDateText()) ? "" : jade.interp) + "</h2><p>" + jade.escape(null == (jade.interp = self.getContent()) ? "" : jade.interp) + '</p><div class="attachments-and-applications">');
        jade.globals.RenderWith_mixin.call({
            buf: buf
        }, self._model, chlk.templates.announcement.Announcement);
        buf.push("</div></div></div>");
        if (studentAnnouncements && studentAnnouncements.length) {
            buf.push('<div class="silver-panel"><div class="auto-grade-buttons right">');
            jade.globals.Button_mixin.call({
                buf: buf,
                block: function() {
                    buf.push(jade.escape(null == (jade.interp = Msg.Auto_grade) ? "" : jade.interp));
                },
                attributes: {
                    disabled: true,
                    "class": "special-button2" + " " + "green"
                },
                escaped: {
                    disabled: true
                }
            });
            jade.globals.Button_mixin.call({
                buf: buf,
                block: function() {
                    buf.push(jade.escape(null == (jade.interp = Msg.Grade_manually) ? "" : jade.interp));
                },
                attributes: {
                    disabled: true,
                    "class": "special-button2" + " " + "red"
                },
                escaped: {
                    disabled: true
                }
            });
            buf.push('</div><div class="grades-info right"><TABLE><TR><TD><span id="graded-count">' + jade.escape(null == (jade.interp = studentAnnouncementsInfo.getGradedStudentCount()) ? "" : jade.interp) + "</span>" + jade.escape(null == (jade.interp = "/" + studentAnnouncements.length) ? "" : jade.interp) + "</TD><TD>" + jade.escape(null == (jade.interp = getGrade(studentAnnouncementsInfo.getClassAvg())) ? "" : jade.interp) + '</TD></TR><TR><TD class="small">' + jade.escape(null == (jade.interp = Msg.Students_graded(studentAnnouncements.length)) ? "" : jade.interp) + '</TD><TD class="small">' + jade.escape(null == (jade.interp = Msg.Class_average) ? "" : jade.interp) + '</TD></TR></TABLE></div><div class="clear-right"></div></div>');
            jade.globals.ListView_mixin.call({
                buf: buf,
                block: function() {
                    (function() {
                        var $$obj = studentAnnouncements;
                        if ("number" == typeof $$obj.length) {
                            for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                var item = $$obj[i];
                                buf.push("<div" + jade.attrs({
                                    index: i,
                                    "class": "row" + " " + (i == selectedIndex ? "selected" : "")
                                }, {
                                    "class": true,
                                    index: true
                                }) + ">");
                                var ownerPictureUrl = self.getOwner().getPictureUrl();
                                var notAnnouncement = self.getAnnouncementTypeId() != chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf();
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "ownerPictureUrl", ownerPictureUrl);
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "notAnnouncement", notAnnouncement);
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "readonly", isReadonly);
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "gradingStyle", gradingStyle);
                                buf.push("<div" + jade.attrs({
                                    id: "top-content-" + item.getId(),
                                    "class": "top-content"
                                }, {
                                    id: true
                                }) + ">");
                                jade.globals.RenderWith_mixin.call({
                                    buf: buf,
                                    attributes: {
                                        ownerPictureUrl: ownerPictureUrl,
                                        notAnnouncement: notAnnouncement,
                                        readonly: isReadonly,
                                        applicationsInGradeView: applications.filter(function(item) {
                                            return item.applicationviewdata.showingradeview;
                                        }),
                                        gradingStyle: gradingStyle,
                                        gradingMapping: studentAnnouncementsInfo.getMapping()
                                    },
                                    escaped: {
                                        ownerPictureUrl: true,
                                        notAnnouncement: true,
                                        readonly: true,
                                        applicationsInGradeView: true,
                                        gradingStyle: true,
                                        gradingMapping: true
                                    }
                                }, item, chlk.templates.announcement.StudentAnnouncement);
                                buf.push("</div></div>");
                            }
                        } else {
                            var $$l = 0;
                            for (var i in $$obj) {
                                $$l++;
                                var item = $$obj[i];
                                buf.push("<div" + jade.attrs({
                                    index: i,
                                    "class": "row" + " " + (i == selectedIndex ? "selected" : "")
                                }, {
                                    "class": true,
                                    index: true
                                }) + ">");
                                var ownerPictureUrl = self.getOwner().getPictureUrl();
                                var notAnnouncement = self.getAnnouncementTypeId() != chlk.models.announcement.AnnouncementTypeEnum.ANNOUNCEMENT.valueOf();
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "ownerPictureUrl", ownerPictureUrl);
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "notAnnouncement", notAnnouncement);
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "readonly", isReadonly);
                                jade.globals.Hidden_mixin.call({
                                    buf: buf
                                }, "gradingStyle", gradingStyle);
                                buf.push("<div" + jade.attrs({
                                    id: "top-content-" + item.getId(),
                                    "class": "top-content"
                                }, {
                                    id: true
                                }) + ">");
                                jade.globals.RenderWith_mixin.call({
                                    buf: buf,
                                    attributes: {
                                        ownerPictureUrl: ownerPictureUrl,
                                        notAnnouncement: notAnnouncement,
                                        readonly: isReadonly,
                                        applicationsInGradeView: applications.filter(function(item) {
                                            return item.applicationviewdata.showingradeview;
                                        }),
                                        gradingStyle: gradingStyle,
                                        gradingMapping: studentAnnouncementsInfo.getMapping()
                                    },
                                    escaped: {
                                        ownerPictureUrl: true,
                                        notAnnouncement: true,
                                        readonly: true,
                                        applicationsInGradeView: true,
                                        gradingStyle: true,
                                        gradingMapping: true
                                    }
                                }, item, chlk.templates.announcement.StudentAnnouncement);
                                buf.push("</div></div>");
                            }
                        }
                    }).call(this);
                },
                attributes: {
                    selectedIndex: selectedIndex,
                    "class": "grades-individual" + " " + "people-list"
                },
                escaped: {
                    selectedIndex: true
                }
            }, studentAnnouncementsInfo);
        }
        buf.push("</div></div></div>");
        return buf.join("");
    };
    "chlk.templates.announcement.Announcement";
    "chlk.models.announcement.Announcement";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).announcement = chlk.templates.announcement || {};
        (function() {
            chlk.templates.announcement.AnnouncementView = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_9fyvtcq9r2fkcsor") ], [ ria.templates.ModelBind(chlk.models.announcement.Announcement) ], "AnnouncementView", ria.__SYNTAX.EXTENDS(chlk.templates.announcement.Announcement), []);
        })();
    })();
    __ASSETS._ondzxeazudte29 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        var getGrade = function(value) {
            return GradingStyler.getLetterByGrade(value, self.getGradingMapping(), self.getGradingStyle());
        };
        var getGradeNumber = function(value) {
            return GradingStyler.getGradeNumberValue(value, self.getGradingMapping(), self.getGradingStyle());
        };
        var value = self.getGradeValue();
        var normalValue = value >= 0 ? value : "";
        var isDrop = self.isDropped();
        var res = self.getGradingStyle() ? !isNaN(value) ? getGrade(value) : "" : value >= 0 ? value : "";
        var lbl = "A - F";
        var itemCls = isDrop ? "dropped" : "";
        var selectedCls = !self.isReadonly() ? "not-selected" : "";
        var student = self.getStudentInfo();
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<input type="submit" class="x-hidden"/>');
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", self.getId());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "dropped", isDrop);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("<img" + jade.attrs({
                            src: student.getPictureUrl(),
                            "class": "avatar" + " " + "avatar-47"
                        }, {
                            src: true
                        }) + "/>");
                    },
                    attributes: {
                        "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                    },
                    escaped: {}
                }, "students", "info", student.getId());
                buf.push('<div class="i-b name-block"><p>' + jade.escape(null == (jade.interp = student.getFullName()) ? "" : jade.interp) + "</p>");
                if (!self.isReadonly()) {
                    buf.push('<SPAN class="container"><SPAN class="comment-grade">' + jade.escape(null == (jade.interp = self.getComment() ? Msg.Commented : Msg.Comment) ? "" : jade.interp) + '<div class="small-pop-up x-hidden"><div class="container"><div class="triangle"></div><div><div><img' + jade.attrs({
                        src: self.getOwnerPictureUrl(),
                        "class": "avatar-24"
                    }, {
                        src: true
                    }) + "/></div><div>");
                    jade.globals.TextArea_mixin.call({
                        buf: buf,
                        attributes: {
                            name: "comment",
                            "class": "comment-input"
                        },
                        escaped: {
                            name: true
                        }
                    });
                    buf.push('<span class="enter">' + jade.escape(null == (jade.interp = Msg.Enter_when_done) ? "" : jade.interp) + "</span></div></div></div></div></SPAN>");
                    if (self.isNotAnnouncement() && !self.isReadonly()) {
                        buf.push("&nbsp;<A" + jade.attrs({
                            id: "drop-" + self.getId(),
                            "class": "drop-link " + (isDrop ? "undrop" : "")
                        }, {
                            id: true,
                            "class": true
                        }) + ">" + jade.escape(null == (jade.interp = isDrop ? Msg.Undrop : Msg.Drop) ? "" : jade.interp) + "</A>");
                    }
                    buf.push("</SPAN>");
                }
                buf.push('</div><div class="i-b grade-block">');
                if (self.isNotAnnouncement()) {
                    if (self.getApplicationsInGradeView().length || self.getAttachments().length) {
                        buf.push('<div class="grade-triangle"></div>');
                    }
                    buf.push("<div" + jade.attrs({
                        "class": selectedCls + " " + itemCls + " " + "i-b"
                    }, {}) + ">" + jade.escape(null == (jade.interp = isDrop ? Msg.Drop : res) ? "" : jade.interp) + "</div><div" + jade.attrs({
                        rowIndex: i,
                        recId: self.getId(),
                        "class": "selected " + itemCls + " " + "i-b"
                    }, {
                        rowIndex: true,
                        recId: true
                    }) + ">");
                    if (self.getGradingStyle()) {
                        buf.push('<div class="select-container"><div class="select-container2">');
                        jade.globals.Select_mixin.call({
                            buf: buf,
                            block: function() {
                                (function() {
                                    var $$obj = GradingStyler.getGradeLetters();
                                    if ("number" == typeof $$obj.length) {
                                        for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                            var item = $$obj[$index];
                                            currentValue = getGradeNumber(item);
                                            if (currentValue != -1) {
                                                jade.globals.Option_mixin.call({
                                                    buf: buf
                                                }, currentValue, item, res == item);
                                            }
                                        }
                                    } else {
                                        var $$l = 0;
                                        for (var $index in $$obj) {
                                            $$l++;
                                            var item = $$obj[$index];
                                            currentValue = getGradeNumber(item);
                                            if (currentValue != -1) {
                                                jade.globals.Option_mixin.call({
                                                    buf: buf
                                                }, currentValue, item, res == item);
                                            }
                                        }
                                    }
                                }).call(this);
                            },
                            attributes: {
                                id: "grade-select-" + self.getId(),
                                "data-placeholder": " ",
                                "class": "grade-select"
                            },
                            escaped: {
                                id: true,
                                "data-placeholder": true
                            }
                        }, "gradeLetter");
                        buf.push('</div><div class="letters">' + jade.escape(null == (jade.interp = lbl) ? "" : jade.interp) + "</div></div>");
                    }
                    buf.push('<div class="input-container"><INPUT' + jade.attrs({
                        type: "text",
                        autofill: "off",
                        value: normalValue,
                        name: "gradevalue",
                        "class": "no-padding" + " " + "grade-input" + " " + "with-grid-focus"
                    }, {
                        type: true,
                        autofill: true,
                        value: true,
                        name: true
                    }) + '></INPUT><div class="letters">100 - 0</div></div></div>');
                }
                buf.push("</div>");
            }
        }, "announcement", "updateAnnouncementGrade");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.announcement.StudentAnnouncement";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).announcement = chlk.templates.announcement || {};
        (function() {
            chlk.templates.announcement.StudentAnnouncement = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_ondzxeazudte29") ], [ ria.templates.ModelBind(chlk.models.announcement.StudentAnnouncement) ], "StudentAnnouncement", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.AnnouncementId, "announcementId", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.attachment.Attachment), "attachments", [ ria.templates.ModelPropertyBind ], String, "comment", [ ria.templates.ModelPropertyBind ], Boolean, "dropped", [ ria.templates.ModelPropertyBind ], Number, "gradeValue", [ ria.templates.ModelPropertyBind ], chlk.models.id.StudentAnnouncementId, "id", [ ria.templates.ModelPropertyBind ], Number, "state", [ ria.templates.ModelPropertyBind ], chlk.models.people.User, "studentInfo", String, "ownerPictureUrl", Boolean, "notAnnouncement", Boolean, "readonly", Array, "applicationsInGradeView", Number, "gradingStyle", chlk.models.grading.Mapping, "gradingMapping" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.announcement.AnnouncementView";
    "chlk.templates.announcement.StudentAnnouncement";
    "chlk.templates.class.TopBar";
    "chlk.models.grading.Mapping";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).announcement = chlk.activities.announcement || {};
        (function() {
            chlk.activities.announcement.AnnouncementViewPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementView) ], "AnnouncementViewPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ chlk.models.grading.Mapping, "mapping", Array, "applicationsInGradeView", [ ria.mvc.DomEventBind("keypress", ".grade-input") ], [ [ ria.dom.Dom, ria.dom.Event ] ], function inputClick(node, event) {
                if (event.keyCode == ria.dom.Keys.ENTER) {
                    var value = node.getValue();
                    value = parseInt(value, 10);
                    if (!value && value != 0) value = "";
                    if (value) {
                        if (value > 100) value = 100;
                        if (value < 0) value = 0;
                    }
                    node.setValue(value);
                    this.updateItem(node, true);
                }
            }, [ ria.mvc.DomEventBind("keypress", ".comment-input") ], [ [ ria.dom.Dom, ria.dom.Event ] ], function commentPress(node, event) {
                if (event.keyCode == ria.dom.Keys.ENTER) {
                    this.updateItem(node, false);
                    node.parent(".small-pop-up").addClass("x-hidden");
                }
            }, [ ria.mvc.DomEventBind("click", ".drop-link") ], [ [ ria.dom.Dom, ria.dom.Event ] ], function dropClick(node, event) {
                var row = node.parent(".row");
                if (node.hasClass("undrop")) {
                    row.find("[name=dropped]").setValue(false);
                    this.updateItem(node, false);
                } else {
                    row.find("[name=dropped]").setValue(true);
                    row.find(".grade-input").setValue("");
                    this.updateItem(node, true);
                }
            }, [ ria.mvc.DomEventBind("click", ".comment-grade") ], [ [ ria.dom.Dom, ria.dom.Event ] ], function commentClick(node, event) {
                var popUp = node.find(".small-pop-up");
                popUp.removeClass("x-hidden");
                setTimeout(function() {
                    jQuery(popUp.find("textarea").valueOf()).focus();
                }, 10);
            }, [ ria.mvc.DomEventBind("click") ], [ [ ria.dom.Dom, ria.dom.Event ] ], function wholeDomClick(node, event) {
                var target = new ria.dom.Dom(event.target);
                if (!target.hasClass("comment-grade") && !target.parent(".comment-grade").exists()) this.dom.find(".small-pop-up:visible").addClass("x-hidden");
            }, [ [ ria.dom.Dom, Boolean ] ], ria.__SYNTAX.Modifiers.VOID, function updateItem(node, selectNext_) {
                var row = node.parent(".row");
                var container = row.find(".top-content");
                container.addClass("loading");
                var form = row.find("form");
                form.triggerEvent("submit");
                if (selectNext_) {
                    setTimeout(function() {
                        var next = row.next();
                        if (next.exists()) {
                            row.removeClass("selected");
                            next.addClass("selected");
                            jQuery(next.find(".grade-input").valueOf()).focus();
                        }
                    }, 1);
                }
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onRender_(model) {
                BASE(model);
                model.getStudentAnnouncements() && this.setMapping(model.getStudentAnnouncements().getMapping());
                this.setApplicationsInGradeView(model.getApplications().filter(function(item) {
                    return item.applicationviewdata.showingradeview;
                }));
                var that = this;
                jQuery(this.dom.valueOf()).on("change", ".grade-select", function() {
                    var node = new ria.dom.Dom(this);
                    var row = node.parent(".row");
                    row.find(".grade-input").setValue(node.getValue());
                    that.updateItem(node, true);
                });
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onStop_() {
                BASE();
            }, [ ria.mvc.PartialUpdateRule(chlk.templates.announcement.StudentAnnouncement) ], ria.__SYNTAX.Modifiers.VOID, function doUpdateItem(tpl, model, msg_) {
                tpl.options({
                    gradingMapping: this.getMapping(),
                    applicationsInGradeView: this.getApplicationsInGradeView(),
                    ownerPictureUrl: this.dom.find("[name=ownerPictureUrl]").getValue(),
                    notAnnouncement: !!this.dom.find("[name=notAnnouncement]").getValue(),
                    readonly: !!this.dom.find("[name=readonly]").getValue(),
                    gradingStyle: parseInt(this.dom.find("[name=gradingStyle]").getValue(), 10)
                });
                var container = this.dom.find("#top-content-" + model.getId().valueOf());
                container.empty();
                tpl.renderTo(container.removeClass("loading"));
                var gradedCount = this.dom.find(".grade-input[value]").count();
                this.dom.find("#graded-count").setHTML(gradedCount.toString());
            } ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.AnnouncementService";
    "chlk.services.ClassService";
    "chlk.services.PersonService";
    "chlk.services.GradingService";
    "chlk.services.AnnouncementReminderService";
    "chlk.activities.announcement.AnnouncementFormPage";
    "chlk.activities.announcement.AnnouncementViewPage";
    "chlk.models.announcement.AnnouncementForm";
    "chlk.models.announcement.Reminder";
    "chlk.models.announcement.LastMessages";
    "chlk.models.attachment.Attachment";
    "chlk.models.announcement.StudentAnnouncement";
    "chlk.models.id.ClassId";
    "chlk.models.id.AnnouncementId";
    "chlk.models.id.ReminderId";
    "chlk.models.id.AttachmentId";
    "chlk.models.id.MarkingPeriodId";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            var announcementAttachments;
            chlk.controllers.AttachmentTypeEnum =             function wrapper() {
                var values = {};
                function AttachmentTypeEnum(value) {
                    return values.hasOwnProperty(value) ? values[value] : undefined;
                }
                ria.__API.identifier(AttachmentTypeEnum, "chlk.controllers.AttachmentTypeEnum");
                function AttachmentTypeEnumImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.controllers.AttachmentTypeEnum#" + value + "]";
                    };
                }
                ria.__API.extend(AttachmentTypeEnumImpl, AttachmentTypeEnum);
                values[0] = AttachmentTypeEnum.DOCUMENT = new AttachmentTypeEnumImpl(0);
                values[1] = AttachmentTypeEnum.PICTURE = new AttachmentTypeEnumImpl(1);
                return AttachmentTypeEnum;
            }();
            chlk.controllers.AnnouncementController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.AnnouncementService, "announcementService", [ ria.mvc.Inject ], chlk.services.AnnouncementReminderService, "announcementReminderService", [ ria.mvc.Inject ], chlk.services.ClassService, "classService", [ ria.mvc.Inject ], chlk.services.PersonService, "personService", [ ria.mvc.Inject ], chlk.services.GradingService, "gradingService", ria.__API.ArrayOf(chlk.models.attachment.Attachment), "announcementAttachments", [ [ chlk.models.announcement.Reminder ] ], function editAddReminderAction(model) {
                if (model.isDuplicate()) {
                    this.ShowMsgBox("This reminder was added before!.", "fyi.", [ {
                        text: Msg.GOT_IT.toUpperCase()
                    } ]);
                } else {
                    var result, before = model.getBefore();
                    if (model.getId().valueOf()) {
                        this.announcementReminderService.editReminder(model.getId(), before);
                    } else {
                        result = this.announcementReminderService.addReminder(model.getAnnouncementId(), before).then(function(announcement) {
                            var reminders = announcement.getAnnouncementReminders(), res;
                            res = reminders.filter(function(item) {
                                return item.getBefore() == before;
                            });
                            return res[0];
                        });
                        return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result, window.noLoadingMsg);
                    }
                }
            }, [ [ chlk.models.id.ReminderId ] ], function removeReminderAction(announcementReminderId) {
                this.announcementReminderService.deleteReminder(announcementReminderId);
            }, [ [ ria.__API.ArrayOf(chlk.models.attachment.Attachment) ] ], function prepareAttachments(attachments) {
                attachments.forEach(function(item) {
                    if (item.getType() == chlk.controllers.AttachmentTypeEnum.PICTURE.valueOf()) item.setThumbnailUrl(this.announcementService.getAttachmentUri(item.getId(), false, 170, 110));
                }.bind(this));
            }, [ [ chlk.models.announcement.StudentAnnouncement ] ], function updateAnnouncementGradeAction(model) {
                var result = this.gradingService.updateItem(model.getId(), model.getGradeValue(), model.getComment(), model.isDropped());
                return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, result);
            }, [ [ chlk.models.announcement.AnnouncementForm, Boolean ] ], function addEditAction(model, isEdit) {
                var classes = this.classService.getClassesForTopBar();
                var topModel = new chlk.models.class.ClassesForTopBar();
                var announcement = model.getAnnouncement();
                var reminders = announcement.getAnnouncementReminders(), remindersArr = [];
                reminders && reminders.forEach(function(item) {
                    remindersArr.push(item.getBefore());
                });
                model.setReminders(remindersArr);
                var attachments = announcement.getAnnouncementAttachments();
                this.prepareAttachments(attachments);
                this.getContext().getSession().set("AnnouncementAttachments", attachments);
                announcement.setAttachments(attachments.map(function(item) {
                    return item.id;
                }).join(","));
                topModel.setTopItems(classes);
                topModel.setDisabled(isEdit);
                var classId_ = announcement.getClassId();
                if (classId_) {
                    topModel.setSelectedItemId(classId_);
                    var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                    model.setClassInfo(classInfo);
                }
                var announcementTypeId_ = announcement.getAnnouncementTypeId();
                if (announcementTypeId_) {
                    if (classId_ && classInfo) {
                        var types = classInfo.getTypesByClass(), typeId = null;
                        types.forEach(function(item) {
                            if (item.getId() == announcementTypeId_) typeId = announcementTypeId_;
                        });
                        typeId && model.setSelectedTypeId(typeId);
                    }
                }
                model.setTopData(topModel);
                return new ria.async.DeferredData(model);
            }, [ chlk.controllers.SidebarButton("add-new") ], [ [ chlk.models.id.ClassId, Number ] ], function addAction(classId_, announcementTypeId_) {
                this.getView().reset();
                var result = this.announcementService.addAnnouncement(classId_, announcementTypeId_).attach(this.validateResponse_()).then(function(model) {
                    return this.addEditAction(model, false);
                }.bind(this));
                return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
            }, [ [ chlk.models.id.AnnouncementId ] ], function editAction(announcementId) {
                var result = this.announcementService.editAnnouncement(announcementId).attach(this.validateResponse_()).then(function(model) {
                    return this.addEditAction(model, true);
                }.bind(this));
                return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
            }, [ [ chlk.models.id.AnnouncementId ] ], function viewAction(announcementId) {
                this.getView().reset();
                var result = this.announcementService.getAnnouncement(announcementId).attach(this.validateResponse_()).then(function(model) {
                    var now = getDate(), days;
                    var expires = model.getExpiresDate();
                    var expiresDate = expires.getDate();
                    var date = expires.format("(D m/d)");
                    var attachments = model.getAnnouncementAttachments();
                    this.prepareAttachments(attachments);
                    model.setExpiresDateColor("blue");
                    model.getOwner().setPictureUrl(this.personService.getPictureURL(model.getOwner().getId(), 24));
                    model.getStudentAnnouncements() && model.getStudentAnnouncements().getItems().forEach(function(item) {
                        var student = item.getStudentInfo();
                        student.setPictureUrl(this.personService.getPictureURL(student.getId(), 24));
                    }.bind(this));
                    if (formatDate(now, "dd-mm-yy") == expires.format("dd-mm-yy")) {
                        model.setExpiresDateColor("blue");
                        model.setExpiresDateText(Msg.Due_today);
                    } else {
                        if (now > expires.getDate()) {
                            model.setExpiresDateColor("red");
                            days = Math.ceil((now - expiresDate) / 1e3 / 3600 / 24);
                            if (days == 1) {
                                model.setExpiresDateText(Msg.Due_yesterday + " " + date);
                            } else {
                                model.setExpiresDateText(Msg.Due_days_ago(days) + " " + date);
                            }
                        } else {
                            days = Math.ceil((expiresDate - now) / 1e3 / 3600 / 24);
                            if (days == 1) {
                                model.setExpiresDateText(Msg.Due_tomorrow + " " + date);
                            } else {
                                model.setExpiresDateText(Msg.Due_in_days(days) + " " + date);
                            }
                        }
                    }
                    return new ria.async.DeferredData(model);
                }.bind(this));
                return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
            }, [ [ chlk.models.id.AnnouncementId, Object ] ], function uploadAttachmentAction(announcementId, files) {
                var result = this.announcementService.uploadAttachment(announcementId, files).then(function(model) {
                    var attachments = model.getAnnouncementAttachments();
                    this.prepareAttachments(attachments);
                    return model;
                }.bind(this));
                return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
            }, [ [ chlk.models.id.AnnouncementId ] ], function deleteAction(announcementId) {
                this.getContext().getSession().set("noSave", true);
                this.announcementService.deleteAnnouncement(announcementId).attach(this.validateResponse_()).then(function(model) {
                    return this.redirect_("feed", "list", []);
                }.bind(this));
            }, [ [ chlk.models.id.SchoolPersonId ] ], function discardAction(schoolPersonId) {
                this.getContext().getSession().set("noSave", true);
                this.announcementService.deleteDrafts(schoolPersonId).attach(this.validateResponse_()).then(function(model) {
                    return this.redirect_("feed", "list", []);
                }.bind(this));
            }, [ [ chlk.models.id.AttachmentId ] ], function deleteAttachmentAction(attachmentId) {
                var result = this.announcementService.deleteAttachment(attachmentId).attach(this.validateResponse_()).then(function(model) {
                    var announcementForm = new chlk.models.announcement.AnnouncementForm();
                    announcementForm.setAnnouncement(model);
                    return this.addEditAction(announcementForm, true);
                }.bind(this));
                return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
            }, [ [ chlk.models.announcement.Announcement ] ], function saveAction(model) {
                if (!this.getContext().getSession().get("noSave", false)) {
                    this.getContext().getSession().set("noSave", false);
                    var session = this.getContext().getSession();
                    var result;
                    var submitType = model.getSubmitType();
                    var schoolPersonId = model.getPersonId();
                    var announcementTypeId = model.getAnnouncementTypeId();
                    var announcementTypeName = model.getAnnouncementTypeName();
                    var classId = model.getClassId();
                    model.setMarkingPeriodId(session.get("markingPeriod").getId());
                    if (submitType == "listLast") {
                        result = this.announcementService.listLast(classId, announcementTypeId, schoolPersonId).attach(this.validateResponse_()).then(function(data) {
                            var model = new chlk.models.announcement.LastMessages();
                            model.setItems(data);
                            model.setAnnouncementTypeName(announcementTypeName);
                            return new ria.async.DeferredData(model);
                        }.bind(this));
                        return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result, window.noLoadingMsg);
                    } else {
                        if (submitType == "save") {
                            model.setAnnouncementAttachments(this.getContext().getSession().get("AnnouncementAttachments"));
                            var announcementForm = new chlk.models.announcement.AnnouncementForm();
                            announcementForm.setAnnouncement(model);
                            result = this.addEditAction(announcementForm, false);
                            this.saveAnnouncement(model);
                            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
                        } else {
                            if (submitType == "saveNoUpdate") {
                                this.saveAnnouncement(model);
                            } else {
                                if (!this.userInRole(chlk.models.common.RoleEnum.ADMINEDIT) && !this.userInRole(chlk.models.common.RoleEnum.ADMINVIEW) && session.get("finalizedClassesIds").indexOf(classId.valueOf()) > -1) {
                                    var nextMp = model.setMarkingPeriodId(session.get("nextMarkingPeriod"));
                                    if (nextMp) {
                                        this.submitAnnouncement(model);
                                        this.StartLoading(chlk.activities.announcement.AnnouncementFormPage);
                                    }
                                } else {
                                    this.submitAnnouncement(model);
                                    this.StartLoading(chlk.activities.announcement.AnnouncementFormPage);
                                }
                            }
                        }
                    }
                }
            }, [ [ chlk.models.announcement.Announcement ] ], ria.__SYNTAX.Modifiers.VOID, function saveAnnouncement(model) {
                this.announcementService.saveAnnouncement(model.getId(), model.getClassId(), model.getAnnouncementTypeId(), model.getSubject(), model.getContent(), model.getExpiresDate(), model.getAttachments(), model.getApplications(), model.getMarkingPeriodId());
            }, [ [ chlk.models.announcement.Announcement ] ], function submitAnnouncement(model) {
                this.announcementService.submitAnnouncement(model.getId(), model.getClassId(), model.getAnnouncementTypeId(), model.getSubject(), model.getContent(), model.getExpiresDate(), model.getAttachments(), model.getApplications(), model.getMarkingPeriodId()).then(function() {
                    this.redirect_("feed", "list", []);
                }.bind(this));
            } ]);
        })();
    })();
    __ASSETS._lwoje4cb9abmaemi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="feed-container">');
        isNotStudent = true;
        console.info(self.items);
        buf.push('<div class="action-bar not-transparent"><div class="container panel-bg"><div class="feed-notifications">Notifications</div></div></div>');
        jade.globals.Grid_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.GridBody_mixin.call({
                    buf: buf,
                    block: function() {
                        (function() {
                            var $$obj = self.items;
                            if ("number" == typeof $$obj.length) {
                                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                    var item = $$obj[$index];
                                    buf.push('<div class="feed-item">');
                                    if (isNotStudent) {
                                        buf.push('<div class="grades"></div>');
                                    }
                                    buf.push('<div class="bullet">');
                                    jade.globals.ActionLink_mixin.call({
                                        buf: buf,
                                        attributes: {
                                            "class": "star"
                                        },
                                        escaped: {}
                                    }, "announcement", "star", item.getId());
                                    jade.globals.ActionLink_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="item-type"><div class="tooltip-announcement ann-type"><div class="tooltip-content"><Tooltip></Tooltip></div></div><div class="attachment"><div class="icon blue icon announcements-type-7"></div><div class="icon black"></div><h2 class="animated-header">' + jade.escape(null == (jade.interp = item.getSubject()) ? "" : jade.interp) + '</h2><p class="animated">' + jade.escape(null == (jade.interp = item.getShortContent()) ? "" : jade.interp) + '</p><div class="animated">' + jade.escape(null == (jade.interp = item.getExpiresDate().toString("d/m/y")) ? "" : jade.interp) + '</div><div class="attachment-icon"></div><div class="apps-icon"></div></div></div>');
                                        },
                                        attributes: {
                                            "class": "d-b" + " " + "t-d" + " " + "not-blue"
                                        },
                                        escaped: {}
                                    }, "announcement", "view", item.getId());
                                    buf.push("</div></div>");
                                }
                            } else {
                                var $$l = 0;
                                for (var $index in $$obj) {
                                    $$l++;
                                    var item = $$obj[$index];
                                    buf.push('<div class="feed-item">');
                                    if (isNotStudent) {
                                        buf.push('<div class="grades"></div>');
                                    }
                                    buf.push('<div class="bullet">');
                                    jade.globals.ActionLink_mixin.call({
                                        buf: buf,
                                        attributes: {
                                            "class": "star"
                                        },
                                        escaped: {}
                                    }, "announcement", "star", item.getId());
                                    jade.globals.ActionLink_mixin.call({
                                        buf: buf,
                                        block: function() {
                                            buf.push('<div class="item-type"><div class="tooltip-announcement ann-type"><div class="tooltip-content"><Tooltip></Tooltip></div></div><div class="attachment"><div class="icon blue icon announcements-type-7"></div><div class="icon black"></div><h2 class="animated-header">' + jade.escape(null == (jade.interp = item.getSubject()) ? "" : jade.interp) + '</h2><p class="animated">' + jade.escape(null == (jade.interp = item.getShortContent()) ? "" : jade.interp) + '</p><div class="animated">' + jade.escape(null == (jade.interp = item.getExpiresDate().toString("d/m/y")) ? "" : jade.interp) + '</div><div class="attachment-icon"></div><div class="apps-icon"></div></div></div>');
                                        },
                                        attributes: {
                                            "class": "d-b" + " " + "t-d" + " " + "not-blue"
                                        },
                                        escaped: {}
                                    }, "announcement", "view", item.getId());
                                    buf.push("</div></div>");
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
        }, "feed", "page", self);
        buf.push("</div>");
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
            }([ ria.templates.TemplateBind("_idojvb3optzw7b9") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "PaginatedList", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(Object), "items", [ ria.templates.ModelPropertyBind ], Number, "pageIndex", [ ria.templates.ModelPropertyBind ], Number, "pageSize", [ ria.templates.ModelPropertyBind ], Number, "totalCount", [ ria.templates.ModelPropertyBind ], Number, "totalPages", [ ria.templates.ModelPropertyBind ], Boolean, "hasNextPage", [ ria.templates.ModelPropertyBind ], Boolean, "hasPreviousPage" ]);
        })();
    })();
    "chlk.templates.PaginatedList";
    "chlk.models.announcement.Announcement";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).feed = chlk.templates.feed || {};
        (function() {
            chlk.templates.feed.Feed = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.feed." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_lwoje4cb9abmaemi") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "Feed", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.announcement.Announcement), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.feed.Feed";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).feed = chlk.activities.feed || {};
        (function() {
            chlk.activities.feed.FeedListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.feed." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.feed.Feed) ], "FeedListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.AnnouncementService";
    "chlk.activities.feed.FeedListPage";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.FeedController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("FeedController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.AnnouncementService, "announcementService", [ [ Number ] ], function listAction(pageIndex_) {
                var result = this.announcementService.getAnnouncements(pageIndex_ | 0).attach(this.validateResponse_());
                return this.PushView(chlk.activities.feed.FeedListPage, result);
            }, [ [ Number ] ], function pageAction(pageIndex_) {
                var result = this.announcementService.getAnnouncements(pageIndex_ | 0).attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.feed.FeedListPage, result);
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
            }("DeveloperInfo", [ chlk.models.id.SchoolPersonId, "id", [ ria.serialize.SerializeProperty("displayname") ], String, "displayName", String, "email", [ ria.serialize.SerializeProperty("firstname") ], String, "firstName", [ ria.serialize.SerializeProperty("lastname") ], String, "lastName", String, "name", [ ria.serialize.SerializeProperty("schoolid") ], chlk.models.id.SchoolId, "schoolId", [ ria.serialize.SerializeProperty("website") ], String, "webSite" ]);
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
    __ASSETS._r52gg2e2clqv9529 = function anonymous(locals) {
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
            }([ ria.templates.TemplateBind("_r52gg2e2clqv9529") ], [ ria.templates.ModelBind(chlk.models.developer.DeveloperInfo) ], "DeveloperProfile", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolPersonId, "id", [ ria.templates.ModelPropertyBind ], String, "displayName", [ ria.templates.ModelPropertyBind ], String, "email", [ ria.templates.ModelPropertyBind ], String, "firstName", [ ria.templates.ModelPropertyBind ], String, "lastName", [ ria.templates.ModelPropertyBind ], String, "name", [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolId, "schoolId", [ ria.templates.ModelPropertyBind ], String, "webSite" ]);
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
    __ASSETS._45uac5c89scvunmi = function anonymous(locals) {
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
            }([ ria.templates.TemplateBind("_45uac5c89scvunmi") ], [ ria.templates.ModelBind(ria.__API.Class) ], "ChangePassword", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), []);
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
            chlk.models.id.PeriodId =             function wrapper() {
                var values = {};
                function PeriodId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new PeriodIdImpl(value);
                }
                ria.__API.identifier(PeriodId, "chlk.models.id.PeriodId");
                function PeriodIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.PeriodId#" + value + "]";
                    };
                }
                ria.__API.extend(PeriodIdImpl, PeriodId);
                return PeriodId;
            }();
        })();
    })();
    "ria.serialize.IDeserializable";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).common = chlk.models.common || {};
        (function() {
            "use strict";
            chlk.models.common.ChlkTime = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.common." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ChlkTime", ria.__SYNTAX.IMPLEMENTS(ria.serialize.IDeserializable), [ String, "time", [ [ String ] ], String, function toString() {
                return this.getTime();
            }, ria.__SYNTAX.Modifiers.VOID, function deserialize(raw) {
                var h = Math.floor(raw / 60);
                h %= 24;
                raw %= 60;
                if (raw < 10) raw = "0" + raw;
                this.setTime(h + ":" + raw);
            } ]);
        })();
    })();
    "chlk.models.id.PeriodId";
    "chlk.models.id.MarkingPeriodId";
    "chlk.models.common.ChlkTime";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).period = chlk.models.period || {};
        (function() {
            "use strict";
            chlk.models.period.Period = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.period." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Period", [ chlk.models.id.PeriodId, "id", [ ria.serialize.SerializeProperty("starttime") ], chlk.models.common.ChlkTime, "startTime", [ ria.serialize.SerializeProperty("endtime") ], chlk.models.common.ChlkTime, "endTime", Number, "order", [ ria.serialize.SerializeProperty("markingperiodid") ], chlk.models.id.MarkingPeriodId, "markingPeriodId" ]);
        })();
    })();
    "chlk.models.period.Period";
    "chlk.models.common.ChlkDate";
    "chlk.models.announcement.Announcement";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.AnnouncementPeriod = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementPeriod", ria.__SYNTAX.EXTENDS(chlk.models.Popup), [ chlk.models.period.Period, "period", [ ria.serialize.SerializeProperty("roomnumber") ], Number, "roomNumber", Number, "index", chlk.models.common.ChlkDate, "date", ria.__API.ArrayOf(chlk.models.announcement.Announcement), "announcements" ]);
        })();
    })();
    "chlk.models.announcement.AnnouncementPeriod";
    "chlk.models.announcement.Announcement";
    "chlk.models.Popup";
    (function() {
        (((chlk = chlk || {}).models = chlk.models || {}).calendar = chlk.models.calendar || {}).announcement = chlk.models.calendar.announcement || {};
        (function() {
            "use strict";
            chlk.models.calendar.announcement.WeekItem = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("WeekItem", ria.__SYNTAX.EXTENDS(chlk.models.Popup), [ chlk.models.common.ChlkDate, "date", Number, "day", Boolean, "sunday", String, "todayClassName", [ ria.serialize.SerializeProperty("announcementperiods") ], ria.__API.ArrayOf(chlk.models.announcement.AnnouncementPeriod), "announcementPeriods", ria.__API.ArrayOf(chlk.models.announcement.Announcement), "announcements" ]);
        })();
    })();
    "chlk.models.calendar.announcement.WeekItem";
    "chlk.models.calendar.announcement.Month";
    (function() {
        (((chlk = chlk || {}).models = chlk.models || {}).calendar = chlk.models.calendar || {}).announcement = chlk.models.calendar.announcement || {};
        (function() {
            "use strict";
            chlk.models.calendar.announcement.Week = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Week", ria.__SYNTAX.EXTENDS(chlk.models.calendar.announcement.Month), [ ria.__API.ArrayOf(chlk.models.calendar.announcement.WeekItem), "items" ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.ClassPeriodId =             function wrapper() {
                var values = {};
                function ClassPeriodId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new ClassPeriodIdImpl(value);
                }
                ria.__API.identifier(ClassPeriodId, "chlk.models.id.ClassPeriodId");
                function ClassPeriodIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.ClassPeriodId#" + value + "]";
                    };
                }
                ria.__API.extend(ClassPeriodIdImpl, ClassPeriodId);
                return ClassPeriodId;
            }();
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.RoomId =             function wrapper() {
                var values = {};
                function RoomId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new RoomIdImpl(value);
                }
                ria.__API.identifier(RoomId, "chlk.models.id.RoomId");
                function RoomIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.RoomId#" + value + "]";
                    };
                }
                ria.__API.extend(RoomIdImpl, RoomId);
                return RoomId;
            }();
        })();
    })();
    "chlk.models.id.ClassPeriodId";
    "chlk.models.id.ClassId";
    "chlk.models.id.RoomId";
    "chlk.models.period.Period";
    "chlk.models.class.Class";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).period = chlk.models.period || {};
        (function() {
            "use strict";
            chlk.models.period.ClassPeriod = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.period." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("ClassPeriod", [ chlk.models.id.ClassPeriodId, "id", chlk.models.period.Period, "period", [ ria.serialize.SerializeProperty("roomid") ], chlk.models.id.RoomId, "roomId", [ ria.serialize.SerializeProperty("roomnumber") ], Number, "roomNumber", [ ria.serialize.SerializeProperty("classid") ], chlk.models.id.ClassId, "classId", [ ria.serialize.SerializeProperty("class") ], chlk.models.class.Class, "clazz", [ ria.serialize.SerializeProperty("studentscount") ], Number, "studentsCount" ]);
        })();
    })();
    "chlk.models.period.ClassPeriod";
    "chlk.models.announcement.Announcement";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).announcement = chlk.models.announcement || {};
        (function() {
            "use strict";
            chlk.models.announcement.AnnouncementClassPeriod = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementClassPeriod", [ [ ria.serialize.SerializeProperty("classperiod") ], chlk.models.period.ClassPeriod, "classPeriod", [ ria.serialize.SerializeProperty("daynumber") ], Number, "dayNumber", ria.__API.ArrayOf(chlk.models.announcement.Announcement), "announcements" ]);
        })();
    })();
    "chlk.models.announcement.AnnouncementClassPeriod";
    "chlk.models.period.Period";
    (function() {
        (((chlk = chlk || {}).models = chlk.models || {}).calendar = chlk.models.calendar || {}).announcement = chlk.models.calendar.announcement || {};
        (function() {
            "use strict";
            chlk.models.calendar.announcement.CalendarDayItem = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("CalendarDayItem", [ [ ria.serialize.SerializeProperty("announcementclassperiods") ], ria.__API.ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), "announcementClassPeriods", chlk.models.period.Period, "period" ]);
        })();
    })();
    "chlk.models.calendar.announcement.CalendarDayItem";
    (function() {
        (((chlk = chlk || {}).models = chlk.models || {}).calendar = chlk.models.calendar || {}).announcement = chlk.models.calendar.announcement || {};
        (function() {
            "use strict";
            chlk.models.calendar.announcement.DayItem = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("DayItem", [ chlk.models.common.ChlkDate, "date", Number, "day", [ ria.serialize.SerializeProperty("calendardayitems") ], ria.__API.ArrayOf(chlk.models.calendar.announcement.CalendarDayItem), "calendarDayItems" ]);
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.period.ClassPeriod";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).calendar = chlk.models.calendar || {};
        (function() {
            "use strict";
            chlk.models.calendar.TeacherSettingsCalendarDay = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.calendar." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TeacherSettingsCalendarDay", [ chlk.models.common.ChlkDate, "date", Number, "day", [ ria.serialize.SerializeProperty("classperiods") ], ria.__API.ArrayOf(chlk.models.period.ClassPeriod), "classPeriods" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.common.ChlkDate";
    "chlk.models.calendar.announcement.MonthItem";
    "chlk.models.calendar.announcement.Week";
    "chlk.models.calendar.announcement.WeekItem";
    "chlk.models.calendar.announcement.DayItem";
    "chlk.models.calendar.TeacherSettingsCalendarDay";
    "chlk.models.id.ClassId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            var Serializer = new ria.serialize.JsonSerializer();
            chlk.services.CalendarService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("CalendarService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.ClassId, chlk.models.common.ChlkDate ] ], ria.async.Future, function listForMonth(classId_, date_) {
                return this.get("AnnouncementCalendar/List.json", ria.__API.ArrayOf(chlk.models.calendar.announcement.MonthItem), {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString("mm-dd-yy")
                }).then(function(model) {
                    this.getContext().getSession().set("monthCalendarData", model);
                    return model;
                }.bind(this));
            }, [ [ chlk.models.common.ChlkDate ] ], ria.async.Future, function getMonthDayInfo(date) {
                var monthCalendarData = this.getContext().getSession().get("monthCalendarData", []), res = null;
                monthCalendarData.forEach(function(day) {
                    if (day.getDate().isSameDay(date)) res = day;
                });
                return new ria.async.DeferredData(res);
            }, [ [ chlk.models.common.ChlkDate, Number ] ], ria.async.Future, function getWeekDayInfo(date, periodNumber_) {
                var weekCalendarData = this.getContext().getSession().get("weekCalendarData", []), res = null;
                weekCalendarData.forEach(function(day) {
                    if (day.getDate().isSameDay(date)) res = day;
                });
                if (periodNumber_ >= 0) res = res.getAnnouncementPeriods()[periodNumber_];
                return new ria.async.DeferredData(res);
            }, [ [ chlk.models.id.ClassId, chlk.models.common.ChlkDate ] ], ria.async.Future, function getTeacherClassWeek(classId_, date_) {
                return this.get("AnnouncementCalendar/TeacherClassWeek.json", null, {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString("mm-dd-yy")
                });
            }, [ [ ria.__API.ArrayOf(chlk.models.calendar.announcement.WeekItem), chlk.models.common.ChlkDate ] ], ria.__API.ArrayOf(chlk.models.calendar.announcement.WeekItem), function prepareWeekData(data, date_) {
                var max = 0, index = 0, kil = 0, empty = 0, empty2 = 0, sun, date, startArray = [], endArray = [];
                var len = data.length;
                if (len < 7) {
                    var dt = len ? data[0].getDate() : date_;
                    kil = dt.getDate().getDay();
                    sun = dt.add(chlk.models.common.ChlkDateEnum.DAY, -kil);
                    empty = 7 - len;
                    empty2 = dt.getDate().getDay();
                }
                len = 7;
                function pushEmptyItem(array, i) {
                    date = sun.add(chlk.models.common.ChlkDateEnum.DAY, i);
                    array.push(Serializer.deserialize({
                        announcementperiods: [],
                        announcements: [],
                        date: date.getDate(),
                        day: date.getDate().getDate(),
                        sunday: date.format("DD") == Msg.Sunday
                    }, chlk.models.calendar.announcement.WeekItem));
                }
                for (var i = 0; i < len; i++) {
                    if (kil) {
                        kil--;
                        pushEmptyItem(startArray, i);
                    } else {
                        if (empty && !data[i - empty2]) {
                            empty--;
                            pushEmptyItem(endArray, i);
                        } else {
                            if (data[i - empty2]) {
                                if (max < data[i - empty2].getAnnouncementPeriods().length) {
                                    max = data[i - empty2].getAnnouncementPeriods().length;
                                    index = i - empty2;
                                }
                            }
                        }
                    }
                }
                function pushEmptyPeriods(item, periodBeginIndex, periods, biggestItem) {
                    for (var j = periodBeginIndex; j < max; j++) {
                        var period = Object.extend({}, biggestItem.getAnnouncementPeriods()[j].getPeriod());
                        var newPeriod = Object.extend({}, period);
                        periods.push(Serializer.deserialize({
                            announcements: [],
                            period: newPeriod
                        }, chlk.models.announcement.AnnouncementPeriod));
                    }
                    return periods;
                }
                var dt = new chlk.models.common.ChlkDate(getDate());
                for (i = 0; i < data.length; i++) {
                    var announcementperiods = data[i].getAnnouncementPeriods();
                    data[i].setTodayClassName(dt.format("dd-mm-yy") == data[i].getDate().format("dd-mm-yy") ? "today" : "");
                    if (!(announcementperiods instanceof Array)) {
                        data[i].setAnnouncementPeriods([]);
                    }
                    announcementperiods.forEach(function(item, index) {
                        if (item.getPeriod()) item.setIndex(index);
                    });
                    if (announcementperiods.length < max) {
                        var begin = announcementperiods.length;
                        pushEmptyPeriods(data[i], begin, announcementperiods, data[index]);
                    }
                }
                startArray.forEach(function(item) {
                    pushEmptyPeriods(item, 0, item.getAnnouncementPeriods(), data[index]);
                });
                endArray.forEach(function(item) {
                    pushEmptyPeriods(item, 0, item.getAnnouncementPeriods(), data[index]);
                });
                var res = startArray.concat(data).concat(endArray);
                this.getContext().getSession().set("weekCalendarData", res);
                return res;
            }, [ [ chlk.models.common.ChlkDate ] ], ria.async.Future, function getAdminDay(date_) {
                return this.get("AnnouncementCalendar/AdminDay.json", ria.__API.ArrayOf(chlk.models.calendar.announcement.CalendarDay), {
                    date: date_ && date_.toString("mm-dd-yy")
                });
            }, [ [ chlk.models.id.ClassId, chlk.models.common.ChlkDate ] ], ria.async.Future, function getWeekInfo(classId_, date_) {
                return this.get("AnnouncementCalendar/Week.json", ria.__API.ArrayOf(chlk.models.calendar.announcement.WeekItem), {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString("mm-dd-yy")
                }).then(function(data) {
                    return this.prepareWeekData(data, date_);
                }.bind(this));
            } ]);
        })();
    })();
    __ASSETS._wnygqghqa9ssjor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="chlk-calendar">');
        var topData = self.getTopData();
        var topItems = topData.getTopItems();
        var selectedId = topData.getSelectedItemId();
        var selectedId = selectedId ? selectedId.valueOf() : "";
        jade.globals.LeftRightToolbar_mixin.call({
            buf: buf,
            attributes: {
                selectedItemId: selectedId,
                "class": "classes-bar"
            },
            escaped: {
                selectedItemId: true
            }
        }, topItems, chlk.templates.class.TopBar, "calendar", "week", [ self.getCurrentDate().format("mm-dd-yy") ]);
        buf.push('<div class="header"><div class="header-container"><div class="buttons-container"><div class="buttons"><div class="calendar-today-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "calendar-today" + " " + "week"
            },
            escaped: {}
        }, "calendar", "week", [ null ], selectedId);
        buf.push('</div><div class="prev-month-container">');
        if (self.getPrevDate()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "prev-month" + " " + "week" + " " + "teacher"
                },
                escaped: {}
            }, "calendar", "week", [ self.getPrevDate().format("mm-dd-yy"), selectedId ]);
        } else {
            buf.push('<A class="prev-month week teacher"></A>');
        }
        buf.push('</div><div class="current-month-container week"><label class="current-month">' + jade.escape(null == (jade.interp = self.getCurrentTitle()) ? "" : jade.interp) + '</label></div><div class="next-month-container">');
        if (self.getNextDate()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "next-month" + " " + "week"
                },
                escaped: {}
            }, "calendar", "week", [ self.getNextDate().format("mm-dd-yy"), selectedId ]);
        } else {
            buf.push('<A class="next-month week"></A>');
        }
        buf.push('</div><div class="calendar-day-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<span class="tooltip"></span>');
            },
            attributes: {
                "class": "calendar-day" + " " + "week"
            },
            escaped: {}
        }, "calendar", "day");
        buf.push('</div><div class="calendar-week-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<span class="tooltip"></span>');
            },
            attributes: {
                "class": "calendar-week" + " " + "week" + " " + "pressed"
            },
            escaped: {}
        }, "calendar", "week");
        buf.push('</div><div class="calendar-month-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<span class="tooltip"></span>');
            },
            attributes: {
                "class": "calendar-month" + " " + "week"
            },
            escaped: {}
        }, "calendar", "month");
        buf.push('</div></div></div></div></div><div class="day-names">');
        (function() {
            var $$obj = self.getItems();
            if ("number" == typeof $$obj.length) {
                for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                    var item = $$obj[i];
                    buf.push("<div" + jade.attrs({
                        "class": item.getTodayClassName() + " " + "week"
                    }, {}) + "><p>" + jade.escape(null == (jade.interp = item.getDate().format("DD")) ? "" : jade.interp) + "</p><p>" + jade.escape(null == (jade.interp = item.getDay()) ? "" : jade.interp) + "</p></div>");
                }
            } else {
                var $$l = 0;
                for (var i in $$obj) {
                    $$l++;
                    var item = $$obj[i];
                    buf.push("<div" + jade.attrs({
                        "class": item.getTodayClassName() + " " + "week"
                    }, {}) + "><p>" + jade.escape(null == (jade.interp = item.getDate().format("DD")) ? "" : jade.interp) + "</p><p>" + jade.escape(null == (jade.interp = item.getDay()) ? "" : jade.interp) + "</p></div>");
                }
            }
        }).call(this);
        buf.push('</div><div class="tb-announcements">');
        (function() {
            var $$obj = self.getItems();
            if ("number" == typeof $$obj.length) {
                for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                    var item = $$obj[i];
                    buf.push("<div>");
                    var anns = item.getAnnouncements();
                    if (anns && anns.length) {
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            attributes: {
                                "class": "bar-item"
                            },
                            escaped: {}
                        }, "calendar", "showWeekBarPopUp", item.getDate().format("mm-dd-yy"));
                    }
                    buf.push("</div>");
                }
            } else {
                var $$l = 0;
                for (var i in $$obj) {
                    $$l++;
                    var item = $$obj[i];
                    buf.push("<div>");
                    var anns = item.getAnnouncements();
                    if (anns && anns.length) {
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            attributes: {
                                "class": "bar-item"
                            },
                            escaped: {}
                        }, "calendar", "showWeekBarPopUp", item.getDate().format("mm-dd-yy"));
                    }
                    buf.push("</div>");
                }
            }
        }).call(this);
        buf.push('</div><div class="items">');
        (function() {
            var $$obj = self.getItems();
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var item = $$obj[$index];
                    buf.push("<div" + jade.attrs({
                        "data-day": item.getDay(),
                        "class": item.getTodayClassName() + " " + "gradient-cnt" + " " + "item" + " " + "week-day" + " " + "week"
                    }, {
                        "data-day": true
                    }) + "><ul>");
                    (function() {
                        var $$obj = item.getAnnouncementPeriods();
                        if ("number" == typeof $$obj.length) {
                            for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                var announcementPeriod = $$obj[i];
                                jade.globals.ActionLink_mixin.call({
                                    buf: buf,
                                    block: function() {
                                        if (item.getDate().format("DD") == Msg.Sunday) {
                                            buf.push('<div class="period-info">' + jade.escape(null == (jade.interp = i + 1) ? "" : jade.interp) + "</div>");
                                        } else {
                                            var anns = announcementPeriod.getAnnouncements();
                                            if (anns.length == 1) {
                                                buf.push('<li><div class="announcement-item"><div class="course-icon"><img/></div><div class="text-container"><p class="typename">' + jade.escape(null == (jade.interp = anns[0].getAnnouncementTypeName()) ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getClassName()) ? "" : jade.interp) + "</p></div></div></li>");
                                            }
                                            if (anns.length > 1) {
                                                buf.push('<li><div class="more-announcements-item"><p class="typename">' + jade.escape((jade.interp = anns.length) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = Msg.Item(true)) == null ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getTitle()) ? "" : jade.interp) + "</p></div></li>");
                                            }
                                        }
                                    },
                                    attributes: {
                                        "class": "period-item"
                                    },
                                    escaped: {}
                                }, "calendar", "showWeekBarPopUp", item.getDate().format("mm-dd-yy"), i);
                            }
                        } else {
                            var $$l = 0;
                            for (var i in $$obj) {
                                $$l++;
                                var announcementPeriod = $$obj[i];
                                jade.globals.ActionLink_mixin.call({
                                    buf: buf,
                                    block: function() {
                                        if (item.getDate().format("DD") == Msg.Sunday) {
                                            buf.push('<div class="period-info">' + jade.escape(null == (jade.interp = i + 1) ? "" : jade.interp) + "</div>");
                                        } else {
                                            var anns = announcementPeriod.getAnnouncements();
                                            if (anns.length == 1) {
                                                buf.push('<li><div class="announcement-item"><div class="course-icon"><img/></div><div class="text-container"><p class="typename">' + jade.escape(null == (jade.interp = anns[0].getAnnouncementTypeName()) ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getClassName()) ? "" : jade.interp) + "</p></div></div></li>");
                                            }
                                            if (anns.length > 1) {
                                                buf.push('<li><div class="more-announcements-item"><p class="typename">' + jade.escape((jade.interp = anns.length) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = Msg.Item(true)) == null ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getTitle()) ? "" : jade.interp) + "</p></div></li>");
                                            }
                                        }
                                    },
                                    attributes: {
                                        "class": "period-item"
                                    },
                                    escaped: {}
                                }, "calendar", "showWeekBarPopUp", item.getDate().format("mm-dd-yy"), i);
                            }
                        }
                    }).call(this);
                    buf.push("</ul></div>");
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var item = $$obj[$index];
                    buf.push("<div" + jade.attrs({
                        "data-day": item.getDay(),
                        "class": item.getTodayClassName() + " " + "gradient-cnt" + " " + "item" + " " + "week-day" + " " + "week"
                    }, {
                        "data-day": true
                    }) + "><ul>");
                    (function() {
                        var $$obj = item.getAnnouncementPeriods();
                        if ("number" == typeof $$obj.length) {
                            for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                var announcementPeriod = $$obj[i];
                                jade.globals.ActionLink_mixin.call({
                                    buf: buf,
                                    block: function() {
                                        if (item.getDate().format("DD") == Msg.Sunday) {
                                            buf.push('<div class="period-info">' + jade.escape(null == (jade.interp = i + 1) ? "" : jade.interp) + "</div>");
                                        } else {
                                            var anns = announcementPeriod.getAnnouncements();
                                            if (anns.length == 1) {
                                                buf.push('<li><div class="announcement-item"><div class="course-icon"><img/></div><div class="text-container"><p class="typename">' + jade.escape(null == (jade.interp = anns[0].getAnnouncementTypeName()) ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getClassName()) ? "" : jade.interp) + "</p></div></div></li>");
                                            }
                                            if (anns.length > 1) {
                                                buf.push('<li><div class="more-announcements-item"><p class="typename">' + jade.escape((jade.interp = anns.length) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = Msg.Item(true)) == null ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getTitle()) ? "" : jade.interp) + "</p></div></li>");
                                            }
                                        }
                                    },
                                    attributes: {
                                        "class": "period-item"
                                    },
                                    escaped: {}
                                }, "calendar", "showWeekBarPopUp", item.getDate().format("mm-dd-yy"), i);
                            }
                        } else {
                            var $$l = 0;
                            for (var i in $$obj) {
                                $$l++;
                                var announcementPeriod = $$obj[i];
                                jade.globals.ActionLink_mixin.call({
                                    buf: buf,
                                    block: function() {
                                        if (item.getDate().format("DD") == Msg.Sunday) {
                                            buf.push('<div class="period-info">' + jade.escape(null == (jade.interp = i + 1) ? "" : jade.interp) + "</div>");
                                        } else {
                                            var anns = announcementPeriod.getAnnouncements();
                                            if (anns.length == 1) {
                                                buf.push('<li><div class="announcement-item"><div class="course-icon"><img/></div><div class="text-container"><p class="typename">' + jade.escape(null == (jade.interp = anns[0].getAnnouncementTypeName()) ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getClassName()) ? "" : jade.interp) + "</p></div></div></li>");
                                            }
                                            if (anns.length > 1) {
                                                buf.push('<li><div class="more-announcements-item"><p class="typename">' + jade.escape((jade.interp = anns.length) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = Msg.Item(true)) == null ? "" : jade.interp) + '</p><p class="title">' + jade.escape(null == (jade.interp = anns[0].getTitle()) ? "" : jade.interp) + "</p></div></li>");
                                            }
                                        }
                                    },
                                    attributes: {
                                        "class": "period-item"
                                    },
                                    escaped: {}
                                }, "calendar", "showWeekBarPopUp", item.getDate().format("mm-dd-yy"), i);
                            }
                        }
                    }).call(this);
                    buf.push("</ul></div>");
                }
            }
        }).call(this);
        buf.push("</div></div>");
        return buf.join("");
    };
    __ASSETS._pclz4x3fyrlr3sor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="chlk-calendar">');
        var topData = self.getTopData();
        var topItems = topData.getTopItems();
        var selectedId = topData.getSelectedItemId();
        var selectedId = selectedId ? selectedId.valueOf() : "";
        jade.globals.LeftRightToolbar_mixin.call({
            buf: buf,
            attributes: {
                selectedItemId: selectedId,
                "class": "classes-bar"
            },
            escaped: {
                selectedItemId: true
            }
        }, topItems, chlk.templates.class.TopBar, "calendar", "month", [ self.getCurrentDate().format("mm-dd-yy") ]);
        buf.push('<div class="header"><div class="header-container"><div class="buttons-container"><div class="buttons"><div class="calendar-today-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "calendar-today"
            },
            escaped: {}
        }, "calendar", "month", [ null ], selectedId);
        buf.push('</div><div class="prev-month-container">');
        if (self.getPrevDate()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "prev-month"
                },
                escaped: {}
            }, "calendar", "month", [ self.getPrevDate().format("mm-dd-yy"), selectedId ]);
        } else {
            buf.push('<A class="prev-month"></A>');
        }
        buf.push('</div><div class="current-month-container"><label class="current-month">' + jade.escape(null == (jade.interp = self.getCurrentTitle()) ? "" : jade.interp) + '</label></div><div class="next-month-container">');
        if (self.getNextDate()) {
            jade.globals.ActionLink_mixin.call({
                buf: buf,
                attributes: {
                    "class": "next-month"
                },
                escaped: {}
            }, "calendar", "month", [ self.getNextDate().format("mm-dd-yy"), selectedId ]);
        } else {
            buf.push('<A class="next-month"></A>');
        }
        buf.push('</div><div class="calendar-day-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<span class="tooltip"></span>');
            },
            attributes: {
                "class": "calendar-day"
            },
            escaped: {}
        }, "calendar", "day");
        buf.push('</div><div class="calendar-week-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<span class="tooltip"></span>');
            },
            attributes: {
                "class": "calendar-week"
            },
            escaped: {}
        }, "calendar", "week");
        buf.push('</div><div class="calendar-month-container">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<span class="tooltip"></span>');
            },
            attributes: {
                "class": "calendar-month" + " " + "pressed"
            },
            escaped: {}
        }, "calendar", "month");
        buf.push('</div></div></div></div></div><div class="day-names"><div><p>' + jade.escape(null == (jade.interp = Msg.Sunday) ? "" : jade.interp) + "</p></div><div><p>" + jade.escape(null == (jade.interp = Msg.Monday) ? "" : jade.interp) + "</p></div><div><p>" + jade.escape(null == (jade.interp = Msg.Tuesday) ? "" : jade.interp) + "</p></div><div><p>" + jade.escape(null == (jade.interp = Msg.Wednesday) ? "" : jade.interp) + "</p></div><div><p>" + jade.escape(null == (jade.interp = Msg.Thursday) ? "" : jade.interp) + "</p></div><div><p>" + jade.escape(null == (jade.interp = Msg.Friday) ? "" : jade.interp) + "</p></div><div><p>" + jade.escape(null == (jade.interp = Msg.Saturday) ? "" : jade.interp) + '</p></div></div><div class="items">');
        (function() {
            var $$obj = self.getItems();
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var item = $$obj[$index];
                    jade.globals.ActionLink_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push('<div><h2 class="number">' + jade.escape(null == (jade.interp = item.getDay()) ? "" : jade.interp) + '</h2><div class="space"></div>');
                            var announcements = item.getAnnouncements();
                            var max = 7;
                            if (announcements.length > 0) {
                                buf.push("<ul" + jade.attrs({
                                    "class": item.getRole() + " " + "day-announcements"
                                }, {}) + ">");
                                (function() {
                                    var $$obj = announcements;
                                    if ("number" == typeof $$obj.length) {
                                        for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                            var announcement = $$obj[i];
                                            if (i < item.getAnnLimit() || announcements.length <= item.getAnnLimit()) {
                                                buf.push('<li class="items">' + jade.escape(null == (jade.interp = announcement.getTitle()) ? "" : jade.interp) + "</li>");
                                            }
                                        }
                                    } else {
                                        var $$l = 0;
                                        for (var i in $$obj) {
                                            $$l++;
                                            var announcement = $$obj[i];
                                            if (i < item.getAnnLimit() || announcements.length <= item.getAnnLimit()) {
                                                buf.push('<li class="items">' + jade.escape(null == (jade.interp = announcement.getTitle()) ? "" : jade.interp) + "</li>");
                                            }
                                        }
                                    }
                                }).call(this);
                                if (announcements.length > item.getAnnLimit()) {
                                    buf.push("<li>+" + jade.escape((jade.interp = announcements.length - item.getAnnLimit() + 1) == null ? "" : jade.interp) + " more</li>");
                                }
                                buf.push("</ul>");
                            }
                            buf.push('<ul class="day-items">');
                            var announcementsLen = announcements.length > 3 ? 3 : announcements.length;
                            var maxLen = max - announcementsLen;
                            var itemsArr = item.getItemsArray();
                            (function() {
                                var $$obj = itemsArr;
                                if ("number" == typeof $$obj.length) {
                                    for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                        var annItem = $$obj[i];
                                        if (i < maxLen || itemsArr.length <= maxLen) {
                                            buf.push("<li>" + jade.escape(null == (jade.interp = annItem.title) ? "" : jade.interp) + "</li>");
                                        }
                                        if (itemsArr.length > maxLen) {
                                            buf.push("<li>+" + jade.escape((jade.interp = itemsArr.length - maxLen + 1) == null ? "" : jade.interp) + " more</li>");
                                        }
                                    }
                                } else {
                                    var $$l = 0;
                                    for (var i in $$obj) {
                                        $$l++;
                                        var annItem = $$obj[i];
                                        if (i < maxLen || itemsArr.length <= maxLen) {
                                            buf.push("<li>" + jade.escape(null == (jade.interp = annItem.title) ? "" : jade.interp) + "</li>");
                                        }
                                        if (itemsArr.length > maxLen) {
                                            buf.push("<li>+" + jade.escape((jade.interp = itemsArr.length - maxLen + 1) == null ? "" : jade.interp) + " more</li>");
                                        }
                                    }
                                }
                            }).call(this);
                            buf.push("</ul></div>");
                        },
                        attributes: {
                            "class": (item.isSunday() ? "sunday " : " ") + item.getClassName() + " " + item.getTodayClassName() + " " + "gradient-cnt" + " " + "item"
                        },
                        escaped: {}
                    }, "calendar", "showMonthDayPopUp", item.getDate().format("mm-dd-yy"));
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var item = $$obj[$index];
                    jade.globals.ActionLink_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push('<div><h2 class="number">' + jade.escape(null == (jade.interp = item.getDay()) ? "" : jade.interp) + '</h2><div class="space"></div>');
                            var announcements = item.getAnnouncements();
                            var max = 7;
                            if (announcements.length > 0) {
                                buf.push("<ul" + jade.attrs({
                                    "class": item.getRole() + " " + "day-announcements"
                                }, {}) + ">");
                                (function() {
                                    var $$obj = announcements;
                                    if ("number" == typeof $$obj.length) {
                                        for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                            var announcement = $$obj[i];
                                            if (i < item.getAnnLimit() || announcements.length <= item.getAnnLimit()) {
                                                buf.push('<li class="items">' + jade.escape(null == (jade.interp = announcement.getTitle()) ? "" : jade.interp) + "</li>");
                                            }
                                        }
                                    } else {
                                        var $$l = 0;
                                        for (var i in $$obj) {
                                            $$l++;
                                            var announcement = $$obj[i];
                                            if (i < item.getAnnLimit() || announcements.length <= item.getAnnLimit()) {
                                                buf.push('<li class="items">' + jade.escape(null == (jade.interp = announcement.getTitle()) ? "" : jade.interp) + "</li>");
                                            }
                                        }
                                    }
                                }).call(this);
                                if (announcements.length > item.getAnnLimit()) {
                                    buf.push("<li>+" + jade.escape((jade.interp = announcements.length - item.getAnnLimit() + 1) == null ? "" : jade.interp) + " more</li>");
                                }
                                buf.push("</ul>");
                            }
                            buf.push('<ul class="day-items">');
                            var announcementsLen = announcements.length > 3 ? 3 : announcements.length;
                            var maxLen = max - announcementsLen;
                            var itemsArr = item.getItemsArray();
                            (function() {
                                var $$obj = itemsArr;
                                if ("number" == typeof $$obj.length) {
                                    for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                                        var annItem = $$obj[i];
                                        if (i < maxLen || itemsArr.length <= maxLen) {
                                            buf.push("<li>" + jade.escape(null == (jade.interp = annItem.title) ? "" : jade.interp) + "</li>");
                                        }
                                        if (itemsArr.length > maxLen) {
                                            buf.push("<li>+" + jade.escape((jade.interp = itemsArr.length - maxLen + 1) == null ? "" : jade.interp) + " more</li>");
                                        }
                                    }
                                } else {
                                    var $$l = 0;
                                    for (var i in $$obj) {
                                        $$l++;
                                        var annItem = $$obj[i];
                                        if (i < maxLen || itemsArr.length <= maxLen) {
                                            buf.push("<li>" + jade.escape(null == (jade.interp = annItem.title) ? "" : jade.interp) + "</li>");
                                        }
                                        if (itemsArr.length > maxLen) {
                                            buf.push("<li>+" + jade.escape((jade.interp = itemsArr.length - maxLen + 1) == null ? "" : jade.interp) + " more</li>");
                                        }
                                    }
                                }
                            }).call(this);
                            buf.push("</ul></div>");
                        },
                        attributes: {
                            "class": (item.isSunday() ? "sunday " : " ") + item.getClassName() + " " + item.getTodayClassName() + " " + "gradient-cnt" + " " + "item"
                        },
                        escaped: {}
                    }, "calendar", "showMonthDayPopUp", item.getDate().format("mm-dd-yy"));
                }
            }
        }).call(this);
        buf.push("</div></div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.calendar.announcement.Month";
    (function() {
        (((chlk = chlk || {}).templates = chlk.templates || {}).calendar = chlk.templates.calendar || {}).announcement = chlk.templates.calendar.announcement || {};
        (function() {
            chlk.templates.calendar.announcement.MonthPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_pclz4x3fyrlr3sor") ], [ ria.templates.ModelBind(chlk.models.calendar.announcement.Month) ], [ chlk.activities.lib.PageClass("calendar") ], "MonthPage", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.calendar.announcement.MonthItem), "items", [ ria.templates.ModelPropertyBind ], chlk.models.class.ClassesForTopBar, "topData", [ ria.templates.ModelPropertyBind ], Number, "selectedTypeId", [ ria.templates.ModelPropertyBind ], String, "currentTitle", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "nextDate", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "prevDate", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "currentDate" ]);
        })();
    })();
    "chlk.templates.calendar.announcement.MonthPage";
    "chlk.models.calendar.announcement.Week";
    (function() {
        (((chlk = chlk || {}).templates = chlk.templates || {}).calendar = chlk.templates.calendar || {}).announcement = chlk.templates.calendar.announcement || {};
        (function() {
            chlk.templates.calendar.announcement.WeekPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_wnygqghqa9ssjor") ], [ ria.templates.ModelBind(chlk.models.calendar.announcement.Week) ], [ chlk.activities.lib.PageClass("calendar") ], "WeekPage", ria.__SYNTAX.EXTENDS(chlk.templates.calendar.announcement.MonthPage), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.calendar.announcement.WeekItem), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.calendar.announcement.WeekPage";
    (function() {
        (((chlk = chlk || {}).activities = chlk.activities || {}).calendar = chlk.activities.calendar || {}).announcement = chlk.activities.calendar.announcement || {};
        (function() {
            chlk.activities.calendar.announcement.WeekPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("calendar") ], [ ria.mvc.TemplateBind(chlk.templates.calendar.announcement.WeekPage) ], "WeekPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.calendar.announcement.MonthPage";
    (function() {
        (((chlk = chlk || {}).activities = chlk.activities || {}).calendar = chlk.activities.calendar || {}).announcement = chlk.activities.calendar.announcement || {};
        (function() {
            chlk.activities.calendar.announcement.MonthPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("calendar") ], [ ria.mvc.TemplateBind(chlk.templates.calendar.announcement.MonthPage) ], "MonthPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
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
    __ASSETS._wzceypuxnknsif6r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="calendar-popup">');
        var announcements = self.getAnnouncements();
        var items = self.getItems();
        if (announcements.length > 0) {
            buf.push("<h2>" + jade.escape(null == (jade.interp = Msg.Announcement(true)) ? "" : jade.interp) + '</h2><div class="announcements-list"><ul class="day-announcements">');
            (function() {
                var $$obj = announcements;
                if ("number" == typeof $$obj.length) {
                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                        var item = $$obj[$index];
                        buf.push('<li class="items"><!--A(href="#announcement/view/" + item.getId().valueOf()).pop-up-announcement-->');
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="title">' + jade.escape(null == (jade.interp = item.getTitle()) ? "" : jade.interp) + '</p><p class="under">' + jade.escape(null == (jade.interp = item.getClassName() || item.getPersonName()) ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-announcement"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                } else {
                    var $$l = 0;
                    for (var $index in $$obj) {
                        $$l++;
                        var item = $$obj[$index];
                        buf.push('<li class="items"><!--A(href="#announcement/view/" + item.getId().valueOf()).pop-up-announcement-->');
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="title">' + jade.escape(null == (jade.interp = item.getTitle()) ? "" : jade.interp) + '</p><p class="under">' + jade.escape(null == (jade.interp = item.getClassName() || item.getPersonName()) ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-announcement"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                }
            }).call(this);
            buf.push("</ul></div>");
        }
        buf.push('<div class="items-list">');
        if (items.length > 0) {
            buf.push("<h2>" + jade.escape(null == (jade.interp = Msg.Item(true)) ? "" : jade.interp) + '</h2><ul class="day-items">');
            (function() {
                var $$obj = items;
                if ("number" == typeof $$obj.length) {
                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                        var item = $$obj[$index];
                        buf.push("<li" + jade.attrs({
                            "class": "announcement-type-" + item.getAnnouncementTypeId().valueOf() + " " + "items"
                        }, {}) + ">");
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="item-title">' + jade.escape(null == (jade.interp = item.getTitle()) ? "" : jade.interp) + '</p><p class="classname-under">' + jade.escape(null == (jade.interp = item.getClassName()) ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-item"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                } else {
                    var $$l = 0;
                    for (var $index in $$obj) {
                        $$l++;
                        var item = $$obj[$index];
                        buf.push("<li" + jade.attrs({
                            "class": "announcement-type-" + item.getAnnouncementTypeId().valueOf() + " " + "items"
                        }, {}) + ">");
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="item-title">' + jade.escape(null == (jade.interp = item.getTitle()) ? "" : jade.interp) + '</p><p class="classname-under">' + jade.escape(null == (jade.interp = item.getClassName()) ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-item"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                }
            }).call(this);
            buf.push("</ul>");
        }
        buf.push("</div>");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "ann-button" + " " + "plus-ann"
            },
            escaped: {}
        }, "announcement", "add");
        buf.push("</div>");
        return buf.join("");
    };
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
    "chlk.templates.Popup";
    "chlk.models.calendar.announcement.MonthItem";
    (function() {
        (((chlk = chlk || {}).templates = chlk.templates || {}).calendar = chlk.templates.calendar || {}).announcement = chlk.templates.calendar.announcement || {};
        (function() {
            chlk.templates.calendar.announcement.MonthDay = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_wzceypuxnknsif6r") ], [ ria.templates.ModelBind(chlk.models.calendar.announcement.MonthItem) ], "MonthDay", ria.__SYNTAX.EXTENDS(chlk.templates.Popup), [ [ ria.templates.ModelPropertyBind ], Number, "day", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "date", [ ria.templates.ModelPropertyBind ], chlk.models.schoolYear.ScheduleSection, "scheduleSection", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.announcement.Announcement), "announcements", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.announcement.Announcement), "items", [ ria.templates.ModelPropertyBind ], ria.dom.Dom, "target" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePopup";
    "chlk.templates.calendar.announcement.MonthDay";
    (function() {
        (((chlk = chlk || {}).activities = chlk.activities || {}).calendar = chlk.activities.calendar || {}).announcement = chlk.activities.calendar.announcement || {};
        (function() {
            chlk.activities.calendar.announcement.MonthDayPopUp = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-pop-up-container") ], [ chlk.activities.lib.IsHorizontalAxis(false) ], [ chlk.activities.lib.isTopLeftPosition(false) ], [ ria.mvc.ActivityGroup("CalendarPopUp") ], [ ria.mvc.TemplateBind(chlk.templates.calendar.announcement.MonthDay) ], "MonthDayPopUp", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePopup), []);
        })();
    })();
    __ASSETS._ojsvjbr2asymygb9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="calendar-popup">');
        var announcements = self.getAnnouncements();
        if (announcements.length > 0) {
            buf.push('<div class="announcements-list"><h2>' + jade.escape(null == (jade.interp = Msg.Announcement(true)) ? "" : jade.interp) + '</h2><ul class="day-announcements">');
            (function() {
                var $$obj = announcements;
                if ("number" == typeof $$obj.length) {
                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                        var item = $$obj[$index];
                        buf.push('<li class="items"><!--A(href="#announcement/view/" + item.getId().valueOf()).pop-up-announcement-->');
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="title">' + jade.escape(null == (jade.interp = item.getTitle()) ? "" : jade.interp) + '</p><p class="classname-under">' + jade.escape(null == (jade.interp = item.getClassName() || item.getPersonName()) ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-announcement"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                } else {
                    var $$l = 0;
                    for (var $index in $$obj) {
                        $$l++;
                        var item = $$obj[$index];
                        buf.push('<li class="items"><!--A(href="#announcement/view/" + item.getId().valueOf()).pop-up-announcement-->');
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="title">' + jade.escape(null == (jade.interp = item.getTitle()) ? "" : jade.interp) + '</p><p class="classname-under">' + jade.escape(null == (jade.interp = item.getClassName() || item.getPersonName()) ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-announcement"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                }
            }).call(this);
            buf.push("</ul></div>");
        }
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "ann-button" + " " + "plus-ann"
            },
            escaped: {}
        }, "announcement", "add");
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.Popup";
    "chlk.models.calendar.announcement.WeekItem";
    (function() {
        (((chlk = chlk || {}).templates = chlk.templates || {}).calendar = chlk.templates.calendar || {}).announcement = chlk.templates.calendar.announcement || {};
        (function() {
            chlk.templates.calendar.announcement.WeekDay = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_ojsvjbr2asymygb9") ], [ ria.templates.ModelBind(chlk.models.calendar.announcement.WeekItem) ], "WeekDay", ria.__SYNTAX.EXTENDS(chlk.templates.Popup), [ [ ria.templates.ModelPropertyBind ], Number, "day", [ ria.templates.ModelPropertyBind ], Boolean, "sunday", [ ria.templates.ModelPropertyBind ], String, "todayClassName", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "date", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.announcement.AnnouncementPeriod), "announcementPeriods", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.announcement.Announcement), "announcements", [ ria.templates.ModelPropertyBind ], ria.dom.Dom, "target" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePopup";
    "chlk.templates.calendar.announcement.WeekDay";
    (function() {
        (((chlk = chlk || {}).activities = chlk.activities || {}).calendar = chlk.activities.calendar || {}).announcement = chlk.activities.calendar.announcement || {};
        (function() {
            chlk.activities.calendar.announcement.WeekBarPopUp = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-pop-up-container") ], [ chlk.activities.lib.IsHorizontalAxis(false) ], [ chlk.activities.lib.isTopLeftPosition(false) ], [ ria.mvc.ActivityGroup("CalendarPopUp") ], [ ria.mvc.TemplateBind(chlk.templates.calendar.announcement.WeekDay) ], "WeekBarPopUp", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePopup), []);
        })();
    })();
    __ASSETS._6mvgnbzqjhdgf1or = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="calendar-popup">');
        var announcements = self.getAnnouncements();
        var period = self.getPeriod();
        var roomNumber = self.getRoomNumber();
        if (announcements.length > 0) {
            buf.push('<div class="class-name"><h3 class="day-day"><!--todo add link to the class--><a>' + jade.escape(null == (jade.interp = announcements[0].getClassName()) ? "" : jade.interp) + "</a></h3></div>");
        }
        buf.push('<div class="info">');
        if (roomNumber) {
            buf.push('<p class="day-down"><a>Rm ' + jade.escape((jade.interp = roomNumber) == null ? "" : jade.interp) + "</a></p><br/>");
        }
        buf.push('<p class="day-down"><a>' + jade.escape((jade.interp = period.getStartTime()) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = period.getEndTime()) == null ? "" : jade.interp) + '</a></p><br/><p class="day-down"><a>' + jade.escape(null == (jade.interp = self.getDate().format("DD, MM d")) ? "" : jade.interp) + "</a></p><br/></div>");
        if (announcements.length > 0) {
            buf.push('<div class="announcements-list"><ul class="day-announcements">');
            (function() {
                var $$obj = announcements;
                if ("number" == typeof $$obj.length) {
                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                        var item = $$obj[$index];
                        buf.push('<li class="items">');
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="item-title">' + jade.escape((jade.interp = item.getAnnouncementTypeName()) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = item.getOrder()) == null ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-announcement"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                } else {
                    var $$l = 0;
                    for (var $index in $$obj) {
                        $$l++;
                        var item = $$obj[$index];
                        buf.push('<li class="items">');
                        jade.globals.ActionLink_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div><p class="item-title">' + jade.escape((jade.interp = item.getAnnouncementTypeName()) == null ? "" : jade.interp) + " " + jade.escape((jade.interp = item.getOrder()) == null ? "" : jade.interp) + "</p></div>");
                            },
                            attributes: {
                                "class": "pop-up-announcement"
                            },
                            escaped: {}
                        }, "announcement", "view", item.getId());
                        buf.push("</li>");
                    }
                }
            }).call(this);
            buf.push("</ul></div>");
        }
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                "class": "ann-button" + " " + "plus-ann"
            },
            escaped: {}
        }, "announcement", "add");
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.Popup";
    "chlk.models.announcement.AnnouncementPeriod";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).announcement = chlk.templates.announcement || {};
        (function() {
            chlk.templates.announcement.AnnouncementPeriod = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_6mvgnbzqjhdgf1or") ], [ ria.templates.ModelBind(chlk.models.announcement.AnnouncementPeriod) ], "AnnouncementPeriod", ria.__SYNTAX.EXTENDS(chlk.templates.Popup), [ [ ria.templates.ModelPropertyBind ], chlk.models.period.Period, "period", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.announcement.Announcement), "announcements", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "date", [ ria.templates.ModelPropertyBind ], Number, "roomNumber" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePopup";
    "chlk.templates.announcement.AnnouncementPeriod";
    (function() {
        (((chlk = chlk || {}).activities = chlk.activities || {}).calendar = chlk.activities.calendar || {}).announcement = chlk.activities.calendar.announcement || {};
        (function() {
            chlk.activities.calendar.announcement.WeekDayPopUp = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.calendar.announcement." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-pop-up-container") ], [ chlk.activities.lib.IsHorizontalAxis(true) ], [ chlk.activities.lib.isTopLeftPosition(false) ], [ ria.mvc.ActivityGroup("CalendarPopUp") ], [ ria.mvc.TemplateBind(chlk.templates.announcement.AnnouncementPeriod) ], "WeekDayPopUp", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePopup), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.CalendarService";
    "chlk.services.ClassService";
    "chlk.activities.calendar.announcement.WeekPage";
    "chlk.activities.calendar.announcement.MonthPage";
    "chlk.activities.calendar.announcement.MonthDayPopUp";
    "chlk.activities.calendar.announcement.WeekBarPopUp";
    "chlk.activities.calendar.announcement.WeekDayPopUp";
    "chlk.models.calendar.announcement.Month";
    "chlk.models.class.ClassesForTopBar";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.CalendarController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("CalendarController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.CalendarService, "calendarService", [ ria.mvc.Inject ], chlk.services.ClassService, "classService", [ chlk.controllers.AccessForRoles([ chlk.models.common.RoleEnum.TEACHER ]) ], [ [ chlk.models.common.ChlkDate ] ], ria.__SYNTAX.Modifiers.VOID, function showMonthDayPopUpAction(date) {
                var result = this.calendarService.getMonthDayInfo(date).then(function(model) {
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    return model;
                });
                return this.ShadeView(chlk.activities.calendar.announcement.MonthDayPopUp, result);
            }, [ [ chlk.models.common.ChlkDate, Number ] ], ria.__SYNTAX.Modifiers.VOID, function showWeekBarPopUpAction(date, periodNumber_) {
                var result = this.calendarService.getWeekDayInfo(date, periodNumber_).then(function(model) {
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    if (periodNumber_ >= 0) model.setDate(date);
                    return model;
                });
                if (periodNumber_ >= 0) return this.ShadeView(chlk.activities.calendar.announcement.WeekDayPopUp, result);
                return this.ShadeView(chlk.activities.calendar.announcement.WeekBarPopUp, result);
            }, [ [ chlk.models.common.ChlkDate, chlk.models.id.ClassId ] ], function weekAction(date_, classId_) {
                var markingPeriod = this.getContext().getSession().get("markingPeriod");
                var today = new chlk.models.common.ChlkDate(new Date());
                var date = date_ || today;
                var dayNumber = date.getDate().getDay(), sunday = date, saturday = date;
                if (dayNumber) {
                    sunday = date.add(chlk.models.common.ChlkDateEnum.DAY, -dayNumber);
                }
                if (dayNumber != 6) saturday = sunday.add(chlk.models.common.ChlkDateEnum.DAY, 6);
                var title = sunday.format("MM d - ");
                title = title + (sunday.format("M") == saturday.format("M") ? saturday.format("d") : saturday.format("M d"));
                var prevDate = sunday.add(chlk.models.common.ChlkDateEnum.DAY, -1);
                var nextDate = saturday.add(chlk.models.common.ChlkDateEnum.DAY, 1);
                var result = this.calendarService.getWeekInfo(classId_, date_).attach(this.validateResponse_()).then(function(days) {
                    var model = new chlk.models.calendar.announcement.Week();
                    model.setCurrentTitle(title);
                    model.setCurrentDate(date);
                    var startDate = markingPeriod.getStartDate();
                    var endDate = markingPeriod.getEndDate();
                    if (prevDate.format("yy-mm-dd") >= startDate.format("yy-mm-dd")) {
                        model.setPrevDate(prevDate);
                    }
                    if (nextDate.format("yy-mm-dd") <= endDate.format("yy-mm-dd")) {
                        model.setNextDate(nextDate);
                    }
                    model.setItems(days);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(false);
                    classId_ && topModel.setSelectedItemId(classId_);
                    model.setTopData(topModel);
                    return new ria.async.DeferredData(model);
                }.bind(this));
                return this.PushView(chlk.activities.calendar.announcement.WeekPage, result);
            }, [ chlk.controllers.SidebarButton("calendar") ], [ [ chlk.models.common.ChlkDate, chlk.models.id.ClassId ] ], function monthAction(date_, classId_) {
                var markingPeriod = this.getContext().getSession().get("markingPeriod");
                var today = new chlk.models.common.ChlkDate(new Date());
                var date = date_ || today;
                var year = date.getDate().getFullYear();
                var month = date.getDate().getMonth();
                var day = date.getDate().getDate();
                var prevMonth = month ? month - 1 : 11;
                var prevYear = month ? year : year - 1;
                var prevDate = new Date(prevYear, prevMonth, day);
                var nextMonth = month == 11 ? 0 : month + 1;
                var nextYear = month == 11 ? year + 1 : year;
                var nextDate = new Date(nextYear, nextMonth, day);
                var result = this.calendarService.listForMonth(classId_, date_).attach(this.validateResponse_()).then(function(days) {
                    var model = new chlk.models.calendar.announcement.Month();
                    model.setCurrentTitle(date.format("MM"));
                    model.setCurrentDate(date);
                    var startDate = markingPeriod.getStartDate().getDate();
                    var endDate = markingPeriod.getEndDate().getDate();
                    if (prevDate >= startDate) {
                        model.setPrevDate(new chlk.models.common.ChlkDate(prevDate));
                    } else {
                        if (startDate.getMonth() != date.getDate().getMonth()) model.setPrevDate(markingPeriod.getStartDate());
                    }
                    if (nextDate <= endDate) {
                        model.setNextDate(new chlk.models.common.ChlkDate(nextDate));
                    } else {
                        if (nextDate.getMonth() != date.getDate().getMonth()) model.setNextDate(markingPeriod.getEndDate());
                    }
                    days.forEach(function(day) {
                        var itemsArray = [], itemsObject = {};
                        var items = day.getItems();
                        for (var i = 0; i < items.length; i++) {
                            var typeName = items[i].getAnnouncementTypeName();
                            var title = items[i].getTitle();
                            var typeId = items[i].getAnnouncementTypeId();
                            var typesEnum = chlk.models.announcement.AnnouncementTypeEnum;
                            if (itemsObject[typeName]) {
                                if (typeof itemsObject[typeName] == "number") {
                                    itemsObject[typeName] = itemsObject[typeName] + 1;
                                } else {
                                    itemsObject[typeName] = 2;
                                }
                            } else {
                                var showSubject = title !== null && typeId == typesEnum.ADMIN || typeId == typesEnum.ANNOUNCEMENT;
                                itemsObject[typeName] = showSubject ? title + " " + typeName : typeName;
                            }
                        }
                        for (var a in itemsObject) {
                            if (typeof itemsObject[a] == "number") {
                                var count = itemsObject[a];
                                itemsArray.push({
                                    count: count,
                                    title: count + " " + a + "s"
                                });
                            } else {
                                itemsArray.push({
                                    title: itemsObject[a],
                                    count: 1
                                });
                            }
                        }
                        day.setItemsArray(itemsArray);
                        var date = day.getDate().getDate();
                        day.setTodayClassName(today.format("mm-dd-yy") == day.getDate().format("mm-dd-yy") ? "today" : "");
                        day.setRole(this.userIsAdmin() ? "admin" : "no-admin");
                        day.setAnnLimit(this.userIsAdmin() ? 7 : 3);
                        day.setClassName(day.isCurrentMonth() && date >= markingPeriod.getStartDate().getDate() && date <= markingPeriod.getEndDate().getDate() ? "" : "not-current-month");
                    }.bind(this));
                    model.setItems(days);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(false);
                    classId_ && topModel.setSelectedItemId(classId_);
                    model.setTopData(topModel);
                    return new ria.async.DeferredData(model);
                }.bind(this));
                return this.PushView(chlk.activities.calendar.announcement.MonthPage, result);
            } ]);
        })();
    })();
    __ASSETS._mxecjw9kopfrbe29 = function anonymous(locals) {
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
            }([ ria.templates.TemplateBind("_mxecjw9kopfrbe29") ], [ ria.templates.ModelBind(chlk.models.settings.Dashboard) ], "Dashboard", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Boolean, "departmentsVisible", [ ria.templates.ModelPropertyBind ], Boolean, "appCategoriesVisible", [ ria.templates.ModelPropertyBind ], Boolean, "storageMonitorVisible", [ ria.templates.ModelPropertyBind ], Boolean, "preferencesVisible", [ ria.templates.ModelPropertyBind ], Boolean, "backgroundTaskMonitorVisible", [ ria.templates.ModelPropertyBind ], Boolean, "dbMaintenanceVisible" ]);
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
    __ASSETS._54aj4u9tn0xav2t9 = function anonymous(locals) {
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
            }([ ria.templates.TemplateBind("_54aj4u9tn0xav2t9") ], [ ria.templates.ModelBind(chlk.models.settings.TeacherSettings) ], "TeacherSettings", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Boolean, "annoucementNotificationsViaSms", [ ria.templates.ModelPropertyBind ], Boolean, "messagesNotificationsViaSms", [ ria.templates.ModelPropertyBind ], Boolean, "notificationsViaSms", [ ria.templates.ModelPropertyBind ], Boolean, "annoucementNotificationsViaEmail", [ ria.templates.ModelPropertyBind ], Boolean, "messagesNotificationsViaEmail", [ ria.templates.ModelPropertyBind ], Boolean, "notificationsViaEmail" ]);
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
    __ASSETS._bt2dh3qwyer0be29 = function anonymous(locals) {
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
            }([ ria.templates.TemplateBind("_bt2dh3qwyer0be29") ], [ ria.templates.ModelBind(chlk.models.settings.PreferencesList) ], "Preferences", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.settings.Preference), "items" ]);
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
    __ASSETS._7a2x6jho4g8n0zfr = function anonymous(locals) {
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
            }([ ria.templates.TemplateBind("_7a2x6jho4g8n0zfr") ], [ ria.templates.ModelBind(chlk.models.settings.DeveloperSettings) ], "DeveloperSettings", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolPersonId, "developerId", [ ria.templates.ModelPropertyBind ], chlk.models.id.AppId, "currentAppId" ]);
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
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.SchoolPersonId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.TeacherService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TeacherService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.SchoolPersonId ] ], ria.async.Future, function getInfo(personId) {
                return this.get("Teacher/Info.json", chlk.models.people.User, {
                    personId: personId.valueOf()
                });
            }, [ [ chlk.models.id.SchoolPersonId, String, String, String, String, String, String, String, chlk.models.common.ChlkDate ] ], ria.async.Future, function updateInfo(personId, addresses, email, firstName, lastName, gender, phones, salutation, birthDate) {
                return this.post("Teacher/UpdateInfo.json", chlk.models.people.User, {
                    personId: personId.valueOf(),
                    addresses: addresses && JSON.parse(addresses),
                    email: email,
                    firstName: firstName,
                    lastName: lastName,
                    gender: gender,
                    phones: phones && JSON.parse(phones),
                    salutation: salutation,
                    birthdayDate: birthDate && JSON.stringify(birthDate.getDate()).slice(1, -1)
                });
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.FinalGradeId =             function wrapper() {
                var values = {};
                function FinalGradeId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new FinalGradeIdImpl(value);
                }
                ria.__API.identifier(FinalGradeId, "chlk.models.id.FinalGradeId");
                function FinalGradeIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.FinalGradeId#" + value + "]";
                    };
                }
                ria.__API.extend(FinalGradeIdImpl, FinalGradeId);
                return FinalGradeId;
            }();
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.AnnouncementTypeGradingId =             function wrapper() {
                var values = {};
                function AnnouncementTypeGradingId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new AnnouncementTypeGradingIdImpl(value);
                }
                ria.__API.identifier(AnnouncementTypeGradingId, "chlk.models.id.AnnouncementTypeGradingId");
                function AnnouncementTypeGradingIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.AnnouncementTypeGradingId#" + value + "]";
                    };
                }
                ria.__API.extend(AnnouncementTypeGradingIdImpl, AnnouncementTypeGradingId);
                return AnnouncementTypeGradingId;
            }();
        })();
    })();
    "chlk.models.id.AnnouncementTypeGradingId";
    "chlk.models.id.FinalGradeId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).grading = chlk.models.grading || {};
        (function() {
            "use strict";
            chlk.models.grading.AnnouncementTypeGrading = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.grading." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementTypeGrading", [ [ ria.serialize.SerializeProperty("droplowest") ], Boolean, "dropLowest", [ ria.serialize.SerializeProperty("gradingstyle") ], Number, "gradingStyle", chlk.models.id.AnnouncementTypeGradingId, "id", [ ria.serialize.SerializeProperty("typename") ], String, "typeName", Number, "value", Number, "index" ]);
        })();
    })();
    "chlk.models.id.AnnouncementTypeGradingId";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).grading = chlk.models.grading || {};
        (function() {
            "use strict";
            chlk.models.grading.AnnouncementTypeFinal = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.grading." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("AnnouncementTypeFinal", [ Boolean, "dropLowest", Number, "gradingStyle", chlk.models.id.AnnouncementTypeGradingId, "finalGradeAnnouncementTypeId", Number, "percentValue", chlk.models.id.FinalGradeId, "finalGradeId" ]);
        })();
    })();
    "chlk.models.id.FinalGradeId";
    "chlk.models.class.Class";
    "chlk.models.grading.AnnouncementTypeGrading";
    "chlk.models.grading.AnnouncementTypeFinal";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).grading = chlk.models.grading || {};
        (function() {
            "use strict";
            chlk.models.grading.Final = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.grading." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Final", [ chlk.models.id.FinalGradeId, "id", Number, "state", [ ria.serialize.SerializeProperty("class") ], chlk.models.class.Class, "clazz", [ ria.serialize.SerializeProperty("gradedstudentcount") ], Number, "gradedStudentCount", Number, "participation", Number, "discipline", [ ria.serialize.SerializeProperty("droplowestdiscipline") ], Number, "dropLowestDiscipline", Number, "attendance", [ ria.serialize.SerializeProperty("droplowestattendance") ], Number, "dropLowestAttendance", [ ria.serialize.SerializeProperty("gradingstyle") ], Number, "gradingStyle", [ ria.serialize.SerializeProperty("finalgradeanntype") ], ria.__API.ArrayOf(chlk.models.grading.AnnouncementTypeGrading), "finalGradeAnnType", ria.__API.ArrayOf(chlk.models.grading.AnnouncementTypeFinal), "finalGradeAnnTypeSend", Boolean, "changed", String, "submitType", String, "finalGradeAnnouncementTypeIds", String, "percents", String, "dropLowest", String, "gradingStyleByType", Boolean, "needsTypesForClasses", Number, "nextClassNumber" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.grading.Final";
    "chlk.models.id.ClassId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.FinalGradeService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("FinalGradeService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.ClassId, Boolean ] ], ria.async.Future, function getFinalGrades(classId, needBuildItems_) {
                return this.get("FinalGrade/Get.json", chlk.models.grading.Final, {
                    classId: classId.valueOf(),
                    needBuildItems: needBuildItems_
                });
            }, [ [ chlk.models.id.FinalGradeId, Number, Number, Boolean, Number, Boolean, Number, Array, Boolean ] ], ria.async.Future, function update(id, participation, attendance, dropLowestAttendance, discipline, dropLowestDiscipline, gradingStyle, finalGradeAnnouncementTypes, needsTypesForClasses) {
                return this.post("FinalGrade/Update.json", chlk.models.grading.Final, {
                    finalGradeId: id.valueOf(),
                    participationPercent: participation,
                    attendance: attendance,
                    dropLowestAttendance: dropLowestAttendance,
                    discipline: discipline,
                    dropLowestDiscipline: dropLowestDiscipline,
                    gradingStyle: gradingStyle,
                    finalGradeAnnouncementType: finalGradeAnnouncementTypes,
                    needsTypesForClasses: needsTypesForClasses
                });
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.settings.Preference";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.PreferenceService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("PreferenceService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ String ] ], ria.async.Future, function getPublic(key) {
                return this.get("Preference/GetPublic.json", chlk.models.settings.Preference, {
                    key: key
                });
            } ]);
        })();
    })();
    __ASSETS._jnfm264nsy30udi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="setup-page setup-hello loader-container"><div class="loader"></div><h1 class="title">Confirm your info</h1>');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", self.getId());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "addressesValue", self.getAddressesValue());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "phonesValue", self.getPhonesValue());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "birthdate", self.getBirthDate());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "gender", self.getGender());
                buf.push('<div class="x-form-field"><label for="salutation">Name:</label><input' + jade.attrs({
                    type: "text",
                    name: "salutation",
                    value: self.getSalutation(),
                    "class": "validate[required] salutation name-field"
                }, {
                    type: true,
                    name: true,
                    "class": true,
                    value: true
                }) + "/><input" + jade.attrs({
                    type: "text",
                    name: "firstname",
                    value: self.getFirstName(),
                    "class": "validate[required] first-name name-field"
                }, {
                    type: true,
                    name: true,
                    "class": true,
                    value: true
                }) + "/><input" + jade.attrs({
                    type: "text",
                    name: "lastname",
                    value: self.getLastName(),
                    "class": "validate[required] last-name name-field"
                }, {
                    type: true,
                    name: true,
                    "class": true,
                    value: true
                }) + '/><div class="form-ok name-ok"></div></div><div class="x-form-field"><label for="salutation-label"></label><label id="example">How students see your name.</label><input' + jade.attrs({
                    type: "text",
                    name: "salutation-label",
                    value: (self.getSalutation() || "") + " " + self.getFirstName() + " " + self.getLastName(),
                    required: "required",
                    "class": "validate[required] salutation-label"
                }, {
                    type: true,
                    name: true,
                    "class": true,
                    value: true,
                    required: true
                }) + '/></div><div class="x-form-field"><label for="email">Email:</label><input' + jade.attrs({
                    type: "text",
                    name: "email",
                    value: self.getEmail(),
                    "class": "validate[required,custom[email]] email"
                }, {
                    type: true,
                    name: true,
                    "class": true,
                    value: true
                }) + '/><div class="form-ok"></div></div><div class="x-form-field"><label for="password">Password:</label><input type="password" name="password" id="password" class="validate[required]"/><div class="form-ok"></div></div><div class="x-form-field"><label for="passwordConfirm">Password again:</label><input type="password" name="passwordConfirm" class="validate[required, equals[password]] password-confirm"/><div class="form-ok"></div></div><div class="x-form-field"><label></label>');
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        type: "submit",
                        id: "confirm-button",
                        "class": "special-button" + " " + "blue-button"
                    },
                    escaped: {
                        type: true
                    }
                });
                buf.push("</div>");
            },
            attributes: {
                id: "hello-form"
            },
            escaped: {}
        }, "setup", "infoEdit");
        buf.push("</div>");
        return buf.join("");
    };
    __ASSETS._xvrtoehzmlutmx6r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        addresses = self.getAddresses() || [];
        primaryPhone = self.getPrimaryPhone() ? self.getPrimaryPhone().getValue() : "";
        homePhone = self.getHomePhone() ? self.getHomePhone().getValue() : "";
        buf.push('<div class="teacher-info info-page view-mode loader-container"><div class="loader"></div><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Now");
            }
        }, "teachers", "info", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Info");
            },
            attributes: {
                "class": "pressed"
            },
            escaped: {}
        }, "teachers", "info", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Scheduling");
            }
        }, "teachers", "info", self.getId());
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Apps");
            }
        }, "teachers", "info", self.getId());
        buf.push('</div><div class="right">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Settings");
            },
            attributes: {
                "class": "settings"
            },
            escaped: {}
        }, "settings", "dashboard");
        buf.push('</div></div></div><div class="info-view"><div class="title"><div class="avatar-container with-border"><img' + jade.attrs({
            src: self.getPictureUrl(),
            "class": "avatar-128" + " " + "avatar"
        }, {
            src: true
        }) + "/></div><h1>" + jade.escape(null == (jade.interp = self.getFullName()) ? "" : jade.interp) + '</h1></div><div class="sections"><div class="sections-info"><div class="section-title pacifico">' + jade.escape(null == (jade.interp = self.getRole().getName()) ? "" : jade.interp) + '</div><div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Email) ? "" : jade.interp) + '</div><div class="data-info low-font-size"><input' + jade.attrs({
            type: "text",
            value: self.getEmail()
        }, {
            type: true,
            value: true
        }) + '/></div></div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Cell) ? "" : jade.interp) + '</div><div class="data-info big-font-size">' + jade.escape(null == (jade.interp = primaryPhone) ? "" : jade.interp) + '</div></div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Birthday) ? "" : jade.interp) + '</div><div class="data-info low-font-size">' + jade.escape(null == (jade.interp = self.getBirthDateText()) ? "" : jade.interp) + '</div></div><div class="section-data gender-box"><div' + jade.attrs({
            "class": (self.getGender() ? self.getGender().toLowerCase() : "") + " " + "gender-img-box"
        }, {}) + "></div><p>" + jade.escape(null == (jade.interp = self.getGenderFullText()) ? "" : jade.interp) + '</p></div></div></div><div class="sections-info"><div class="section-title pacifico">' + jade.escape(null == (jade.interp = Msg.Home) ? "" : jade.interp) + '</div><div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Home_Phone) ? "" : jade.interp) + '</div><div class="data-info big-font-size">' + jade.escape(null == (jade.interp = homePhone) ? "" : jade.interp) + '</div></div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Home_Address) ? "" : jade.interp) + "</div>");
        (function() {
            var $$obj = addresses;
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var address = $$obj[$index];
                    buf.push('<div class="data-info low-font-size">' + jade.escape(null == (jade.interp = address.getValue()) ? "" : jade.interp) + "</div>");
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var address = $$obj[$index];
                    buf.push('<div class="data-info low-font-size">' + jade.escape(null == (jade.interp = address.getValue()) ? "" : jade.interp) + "</div>");
                }
            }
        }).call(this);
        buf.push('</div></div></div></div><div class="sections-info other-section"><div class="section-title pacifico">' + jade.escape(null == (jade.interp = Msg.Other(true)) ? "" : jade.interp) + "</div></div>");
        if (self.isAbleEdit()) {
            jade.globals.Button_mixin.call({
                buf: buf,
                attributes: {
                    id: "edit-info-button",
                    "class": "special-button" + " " + "blue-button"
                },
                escaped: {}
            });
        }
        buf.push('</div><div class="info-edit">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="title"><div class="avatar-block"><div class="avatar-container with-border"><img' + jade.attrs({
                    src: self.getPictureUrl(),
                    "class": "avatar-128" + " " + "avatar"
                }, {
                    src: true
                }) + '/></div><button type="button" class="simple-gray-button file">Upload');
                jade.globals.FileUpload_mixin.call({
                    buf: buf,
                    attributes: {
                        id: "add-attachment",
                        "class": "simple-gray-button"
                    },
                    escaped: {}
                }, "teachers", "uploadPicture", [ self.getId().valueOf() ]);
                buf.push('</button></div><div class="name-block"><input' + jade.attrs({
                    name: "firstname",
                    value: self.getFirstName(),
                    "class": "validate[required]"
                }, {
                    name: true,
                    value: true,
                    "class": true
                }) + "/><input" + jade.attrs({
                    name: "lastname",
                    value: self.getLastName(),
                    "class": "validate[required]"
                }, {
                    name: true,
                    value: true,
                    "class": true
                }) + "/><input" + jade.attrs({
                    type: "hidden",
                    name: "id",
                    value: self.getId()
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + "/><input" + jade.attrs({
                    type: "hidden",
                    name: "salutation",
                    value: self.getSalutation()
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div></div><div class="sections"><div class="sections-info"><div class="section-title">' + jade.escape(null == (jade.interp = self.getRole().getName()) ? "" : jade.interp) + '</div><div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Email) ? "" : jade.interp) + "</div><input" + jade.attrs({
                    name: "email",
                    value: self.getEmail(),
                    "class": "validate[custom[email], required]"
                }, {
                    name: true,
                    value: true,
                    "class": true
                }) + '/></div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Cell) ? "" : jade.interp) + "</div><input" + jade.attrs({
                    "data-id": primaryPhone && self.getPrimaryPhone().getId().valueOf(),
                    "data-type": 2,
                    "data-isPrimary": true,
                    value: primaryPhone,
                    "class": "validate[custom[phone], required] primary-phone"
                }, {
                    "data-id": true,
                    "data-type": true,
                    "data-isPrimary": true,
                    value: true,
                    "class": true
                }) + "/><input" + jade.attrs({
                    name: "phonesValue",
                    value: self.getPhonesValue(),
                    type: "hidden"
                }, {
                    name: true,
                    value: true,
                    type: true
                }) + "/>");
                jade.globals.Select_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("<option>AT&T</option><option>Verizon</option><option>Sprint</option><option>T-mobile</option><option>Boost Mobile</option><option>Other</option>");
                    },
                    attributes: {
                        "data-placeholder": "*Cell Service*",
                        value: "",
                        id: "cellService",
                        "class": "grey-combo"
                    },
                    escaped: {
                        "data-placeholder": true,
                        value: true
                    }
                }, "cellService");
                buf.push('</div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Birthday) ? "" : jade.interp) + "</div>");
                jade.globals.DateSelect_mixin.call({
                    buf: buf,
                    attributes: {
                        value: self.getBirthDate() ? self.getBirthDate().getDate() : null,
                        name: "birthdate",
                        "class": "grey-combo-container" + " " + "bd-select"
                    },
                    escaped: {
                        value: true,
                        name: true
                    }
                });
                buf.push('<div class="gender-part">');
                jade.globals.Select_mixin.call({
                    buf: buf,
                    block: function() {
                        jade.globals.Option_mixin.call({
                            buf: buf
                        }, "m", "Male", self.getGender() && self.getGender().toLowerCase() == "m");
                        jade.globals.Option_mixin.call({
                            buf: buf
                        }, "f", "Female", self.getGender() && self.getGender().toLowerCase() == "f");
                    },
                    attributes: {
                        "data-placeholder": "Gender",
                        id: "genderSelect",
                        "class": "grey-combo"
                    },
                    escaped: {
                        "data-placeholder": true
                    }
                }, "gender");
                buf.push('</div></div></div></div><div class="sections-info"><div class="section-title">' + jade.escape(null == (jade.interp = Msg.Home) ? "" : jade.interp) + '</div><div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Home_Phone) ? "" : jade.interp) + "</div><input" + jade.attrs({
                    value: homePhone,
                    "data-id": homePhone && self.getHomePhone().getId().valueOf(),
                    "data-type": 0,
                    "data-isPrimary": false,
                    "class": "validate[custom[phone], required] home-phone"
                }, {
                    value: true,
                    "class": true,
                    "data-id": true,
                    "data-type": true,
                    "data-isPrimary": true
                }) + '/></div><div class="section-data"><div class="info-title">' + jade.escape(null == (jade.interp = Msg.Home_Address) ? "" : jade.interp) + '<a class="add-address add-button">add</a></div><input' + jade.attrs({
                    name: "addressesValue",
                    value: self.getAddressesValue(),
                    type: "hidden"
                }, {
                    name: true,
                    value: true,
                    type: true
                }) + '/><div class="adresses">');
                addressesModel = new chlk.models.people.Addresses();
                addressesModel.setItems(addresses);
                jade.globals.RenderWith_mixin.call({
                    buf: buf
                }, addressesModel, chlk.templates.people.Addresses);
                buf.push('</div></div></div></div></div><div class="section-buttons">');
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
        }, "teachers", "infoEdit");
        buf.push("</div></div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.people.User";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.id.SchoolId";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).people = chlk.templates.people || {};
        (function() {
            chlk.templates.people.User = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_xvrtoehzmlutmx6r") ], [ ria.templates.ModelBind(chlk.models.people.User) ], "User", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Boolean, "active", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.people.Address), "addresses", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "birthDate", [ ria.templates.ModelPropertyBind ], String, "birthDateText", [ ria.templates.ModelPropertyBind ], String, "displayName", [ ria.templates.ModelPropertyBind ], String, "email", [ ria.templates.ModelPropertyBind ], String, "firstName", [ ria.templates.ModelPropertyBind ], String, "fullName", [ ria.templates.ModelPropertyBind ], String, "gender", [ ria.templates.ModelPropertyBind ], String, "genderFullText", [ ria.templates.ModelPropertyBind ], String, "grade", [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolPersonId, "id", [ ria.templates.ModelPropertyBind ], String, "lastName", [ ria.templates.ModelPropertyBind ], String, "localId", [ ria.templates.ModelPropertyBind ], String, "password", [ ria.templates.ModelPropertyBind ], String, "pictureUrl", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.people.Phone), "phones", [ ria.templates.ModelPropertyBind ], chlk.models.people.Role, "role", [ ria.templates.ModelPropertyBind ], String, "salutation", [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolId, "schoolId", [ ria.templates.ModelPropertyBind ], chlk.models.people.Phone, "primaryPhone", [ ria.templates.ModelPropertyBind ], chlk.models.people.Phone, "homePhone", [ ria.templates.ModelPropertyBind ], String, "addressesValue", [ ria.templates.ModelPropertyBind ], String, "phonesValue", [ ria.templates.ModelPropertyBind ], Number, "index", [ ria.templates.ModelPropertyBind ], Boolean, "selected" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.people.User";
    "chlk.templates.people.User";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).setup = chlk.templates.setup || {};
        (function() {
            chlk.templates.setup.Hello = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_jnfm264nsy30udi") ], [ ria.templates.ModelBind(chlk.models.people.User) ], "Hello", ria.__SYNTAX.EXTENDS(chlk.templates.people.User), []);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.setup.Hello";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).setup = chlk.activities.setup || {};
        (function() {
            chlk.activities.setup.HelloPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.setup.Hello) ], "HelloPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("keyup", ".name-field") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function nameChange(node, event) {
                var value = this.dom.find(".salutation").getValue() + " " + this.dom.find(".first-name").getValue() + " " + this.dom.find(".last-name").getValue();
                this.dom.find(".salutation-label").setValue(value);
            }, [ ria.mvc.DomEventBind("change", ".name-field") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function nameChanged(node, event) {
                setTimeout(function() {
                    var el = new ria.dom.Dom(".name-ok");
                    if (this.dom.find(".salutation").hasClass("ok") && this.dom.find(".first-name").hasClass("ok") && this.dom.find(".last-name").hasClass("ok")) {
                        el.removeClass("x-hidden");
                    } else {
                        el.addClass("x-hidden");
                    }
                }.bind(this), 10);
            } ]);
        })();
    })();
    __ASSETS._eacds7yik0a1nhfr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="setup-page video loader-container"><div class="loader"></div><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="right">');
        action = self.getType() > 0 ? "teacherSettings" : "start";
        params = self.getType() > 0 ? [ 0 ] : [];
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            attributes: {
                id: "lets-do-it"
            },
            escaped: {}
        }, "setup", action, params);
        buf.push('</div></div></div><div class="blue-arrow right-up-arrow">' + jade.escape(null == (jade.interp = Msg.Click.toLowerCase()) ? "" : jade.interp) + '</div><div class="video-chalkable center">');
        jade.globals.Video_mixin.call({
            buf: buf,
            attributes: {
                src: self.getValue() || "",
                width: "610",
                height: "400"
            },
            escaped: {
                src: true,
                width: true,
                height: true
            }
        }, true);
        buf.push('</div><div class="blue-arrow div bottom-text center">Next just setup how you grade<br/>and you\'re done!</div></div>');
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.settings.Preference";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).setup = chlk.templates.setup || {};
        (function() {
            chlk.templates.setup.Video = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_eacds7yik0a1nhfr") ], [ ria.templates.ModelBind(chlk.models.settings.Preference) ], "Video", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], String, "value", [ ria.templates.ModelPropertyBind ], Number, "type" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.setup.Video";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).setup = chlk.activities.setup || {};
        (function() {
            chlk.activities.setup.VideoPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.setup.Video) ], "VideoPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._trfriswfn8doyldi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="setup-page start loader-container"><div class="loader"></div><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
        action = self.getType() > 0 ? "teacherSettings" : "video";
        params = self.getType() > 0 ? [ self.getType() - 1 ] : [];
        jade.globals.ActionLink_mixin.call({
            buf: buf
        }, "setup", action, params);
        buf.push('</div><div class="right">');
        jade.globals.ActionLink_mixin.call({
            buf: buf
        }, "feed", "list");
        buf.push('</div></div></div><div class="blue-arrow right-up-arrow">' + jade.escape(null == (jade.interp = Msg.Click.toLowerCase()) ? "" : jade.interp) + '</div><div class="blue-arrow div bottom-text center">You\'re all set. Great job!<br/>Go try Chalkable</div></div>');
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.settings.Preference";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).setup = chlk.templates.setup || {};
        (function() {
            chlk.templates.setup.Start = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_trfriswfn8doyldi") ], [ ria.templates.ModelBind(chlk.models.settings.Preference) ], "Start", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], Number, "type" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.setup.Start";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).setup = chlk.activities.setup || {};
        (function() {
            chlk.activities.setup.StartPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.setup.Start) ], "StartPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), []);
        })();
    })();
    __ASSETS._os8r93okvsnpzaor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="setup-page teacher-settings loader-container">');
        topData = self.getTopData();
        topItems = topData.getTopItems();
        selectedIndex = 0;
        gradingInfo = self.getGradingInfo();
        gradingTypes = [ [ 1, "A-F" ], [ 0, "100-0" ], [ 2, Msg.Complete + "-" + Msg.Incomplete ], [ 3, Msg.Check.toLowerCase() ] ];
        buf.push('<div class="loader"></div>');
        jade.globals.LeftRightToolbar_mixin.call({
            buf: buf,
            attributes: {
                selectedItemId: gradingInfo.getClazz().getId().valueOf(),
                pressAfterClick: false,
                "class": "classes-bar"
            },
            escaped: {
                selectedItemId: true,
                pressAfterClick: true
            }
        }, topItems, chlk.templates.class.TopBar);
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "id", gradingInfo.getId());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "changed");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "needsTypesForClasses", true);
                jade.globals.Hidden_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push(")");
                    }
                }, "gradingStyleByType");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "dropLowest");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "percents");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "attendance");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "discipline");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "participation");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "finalGradeAnnouncementTypeIds");
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "nextClassNumber", gradingInfo.getNextClassNumber());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "gradingStyle", gradingInfo.getGradingStyle());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "dropLowestAttendance", gradingInfo.getDropLowestAttendance());
                jade.globals.Hidden_mixin.call({
                    buf: buf
                }, "dropLowestDiscipline", gradingInfo.getDropLowestDiscipline());
                buf.push('<input type="submit" id="update-ann-type-submit" class="x-hidden"/><div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
                nextClass = gradingInfo.getNextClassNumber();
                action = nextClass == 1 ? "video" : "teacherSettings";
                params = nextClass == 1 ? [] : [ nextClass - 2 ];
                buf.push('<Button type="submit" name="submitType" value="back" class="action-button go-back">' + jade.escape(null == (jade.interp = Msg.Go_Back) ? "" : jade.interp) + '</Button><label id="step">' + jade.escape(null == (jade.interp = Msg.Step_of(2, 2)) ? "" : jade.interp) + '</label></div><div class="right"><Button type="submit" disabled="disabled" class="action-button next-class">' + jade.escape(null == (jade.interp = nextClass == topItems.length ? Msg.Confirm : Msg.Next_Class) ? "" : jade.interp) + '</Button></div></div></div><div id="setup-schedule-view" class="panel-bg">');
                classInfo = gradingInfo.getClazz();
                calendarInfo = self.getCalendarInfo();
                buf.push('<div class="left-block"><div><h1><div class="img-container"><img src="https://localhost:8080/chalkable/Course/GetIcon?courseInfoId=30"/></div>' + jade.escape(null == (jade.interp = classInfo.getName()) ? "" : jade.interp) + '</h1><div class="bottom"><div class="course-room">' + jade.escape(null == (jade.interp = classInfo.getCourse().getTitle()) ? "" : jade.interp) + '</div></div></div></div><div class="right-block">');
                (function() {
                    var $$obj = calendarInfo;
                    if ("number" == typeof $$obj.length) {
                        for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                            var item = $$obj[$index];
                            buf.push("<div><div><h3>" + jade.escape(null == (jade.interp = item.getDate().format("D")) ? "" : jade.interp) + '</h3><div class="bottom">');
                            (function() {
                                var $$obj = item.getClassPeriods();
                                if ("number" == typeof $$obj.length) {
                                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                        var classPeriod = $$obj[$index];
                                        period = classPeriod.getPeriod();
                                        buf.push("<p>" + jade.escape(null == (jade.interp = period.getStartTime() + " - " + period.getEndTime()) ? "" : jade.interp) + "</p>");
                                    }
                                } else {
                                    var $$l = 0;
                                    for (var $index in $$obj) {
                                        $$l++;
                                        var classPeriod = $$obj[$index];
                                        period = classPeriod.getPeriod();
                                        buf.push("<p>" + jade.escape(null == (jade.interp = period.getStartTime() + " - " + period.getEndTime()) ? "" : jade.interp) + "</p>");
                                    }
                                }
                            }).call(this);
                            buf.push("</div></div></div>");
                        }
                    } else {
                        var $$l = 0;
                        for (var $index in $$obj) {
                            $$l++;
                            var item = $$obj[$index];
                            buf.push("<div><div><h3>" + jade.escape(null == (jade.interp = item.getDate().format("D")) ? "" : jade.interp) + '</h3><div class="bottom">');
                            (function() {
                                var $$obj = item.getClassPeriods();
                                if ("number" == typeof $$obj.length) {
                                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                        var classPeriod = $$obj[$index];
                                        period = classPeriod.getPeriod();
                                        buf.push("<p>" + jade.escape(null == (jade.interp = period.getStartTime() + " - " + period.getEndTime()) ? "" : jade.interp) + "</p>");
                                    }
                                } else {
                                    var $$l = 0;
                                    for (var $index in $$obj) {
                                        $$l++;
                                        var classPeriod = $$obj[$index];
                                        period = classPeriod.getPeriod();
                                        buf.push("<p>" + jade.escape(null == (jade.interp = period.getStartTime() + " - " + period.getEndTime()) ? "" : jade.interp) + "</p>");
                                    }
                                }
                            }).call(this);
                            buf.push("</div></div></div>");
                        }
                    }
                }).call(this);
                buf.push('</div><a href="mailto:someEmail@gmail.com?Subject=Classes setup mistake" id="mistake">' + jade.escape(null == (jade.interp = Msg.Mistake) ? "" : jade.interp) + '</a></div><div id="settings-step1"><label id="how-grade" class="blue-arrow div">' + jade.escape(null == (jade.interp = Msg.How_do_you_grade_students_in(classInfo.getName())) ? "" : jade.interp) + "</label>");
                jade.globals.Button_mixin.call({
                    buf: buf,
                    attributes: {
                        type: "submit",
                        name: "submitType",
                        value: "message",
                        id: "tell-button",
                        "class": "special-button2" + " " + "blue"
                    },
                    escaped: {
                        type: true,
                        name: true,
                        value: true
                    }
                });
                buf.push('</div><div id="settings-step2" class="x-hidden"><label id="percents-label" class="blue-arrow div">' + jade.escape(null == (jade.interp = Msg.Student_grades_are_out_of(classInfo.getName())) ? "" : jade.interp));
                per = self.getPercentsSum();
                buf.push("<span" + jade.attrs({
                    "data-tooltip": per == 100 ? null : "Add " + (100 - per) + "%",
                    "class": (per == 100 ? "" : "error") + " " + "percents-count"
                }, {
                    "data-tooltip": true
                }) + ">" + jade.escape(null == (jade.interp = per + "%") ? "" : jade.interp) + '</span></label></div><div id="settings-step3" class="x-hidden"><div class="ann-types-container">');
                jade.globals.RenderWith_mixin.call({
                    buf: buf
                }, gradingInfo, chlk.templates.grading.Final);
                buf.push('</div><div class="hint-text">If you don’t use an item type just set it to 0%.</div><div id="other-data" class="not-transparent items-container"><div' + jade.attrs({
                    index: 0,
                    "class": (gradingInfo.getParticipation() ? "active" : "") + " " + "announcement-type-item" + " " + "gradient"
                }, {
                    index: true
                }) + '><div><div class="left name">' + jade.escape(null == (jade.interp = Msg.Participation) ? "" : jade.interp) + '</div><div class="right relative"><input' + jade.attrs({
                    name: "participation",
                    value: gradingInfo.getParticipation() + "%",
                    id: "participation-value",
                    "class": "announcement-type-weight"
                }, {
                    name: true,
                    value: true
                }) + '/><div class="hint absolute">' + jade.escape(null == (jade.interp = Msg.Weight) ? "" : jade.interp) + "</div></div></div></div><div" + jade.attrs({
                    index: 1,
                    "class": (gradingInfo.getAttendance() ? "active" : "") + " " + "announcement-type-item" + " " + "gradient"
                }, {
                    index: true
                }) + '><div><div class="left name">' + jade.escape(null == (jade.interp = Msg.Attendance) ? "" : jade.interp) + '</div><div class="right relative"><input' + jade.attrs({
                    name: "attendance",
                    value: gradingInfo.getAttendance() + "%",
                    id: "attendance-value",
                    "class": "announcement-type-weight"
                }, {
                    name: true,
                    value: true
                }) + '/><div class="hint absolute">' + jade.escape(null == (jade.interp = Msg.Weight) ? "" : jade.interp) + "</div></div></div></div><div" + jade.attrs({
                    index: 2,
                    "class": (gradingInfo.getDiscipline() ? "active" : "") + " " + "announcement-type-item" + " " + "gradient"
                }, {
                    index: true
                }) + '><div><div class="left name">' + jade.escape(null == (jade.interp = Msg.Discipline) ? "" : jade.interp) + '</div><div class="right relative"><input' + jade.attrs({
                    name: "discipline",
                    value: gradingInfo.getDiscipline() + "%",
                    id: "discipline-value",
                    "class": "announcement-type-weight"
                }, {
                    name: true,
                    value: true
                }) + '/><div class="hint absolute">' + jade.escape(null == (jade.interp = Msg.Weight) ? "" : jade.interp) + "</div></div></div></div></div></div>");
            },
            attributes: {
                id: "update-ann-type-submit-form"
            },
            escaped: {}
        }, "setup", "teacherSettingsEdit");
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.models.class.ClassesForTopBar";
    "chlk.models.calendar.TeacherSettingsCalendarDay";
    "chlk.models.grading.Final";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).setup = chlk.models.setup || {};
        (function() {
            "use strict";
            chlk.models.setup.TeacherSettings = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TeacherSettings", [ chlk.models.class.ClassesForTopBar, "topData", ria.__API.ArrayOf(chlk.models.calendar.TeacherSettingsCalendarDay), "calendarInfo", chlk.models.grading.Final, "gradingInfo", Number, "percentsSum" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.setup.TeacherSettings";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).setup = chlk.templates.setup || {};
        (function() {
            "use strict";
            chlk.templates.setup.TeacherSettings = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_os8r93okvsnpzaor") ], [ ria.templates.ModelBind(chlk.models.setup.TeacherSettings) ], "TeacherSettings", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.class.ClassesForTopBar, "topData", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.calendar.TeacherSettingsCalendarDay), "calendarInfo", [ ria.templates.ModelPropertyBind ], chlk.models.grading.Final, "gradingInfo", [ ria.templates.ModelPropertyBind ], Number, "percentsSum" ]);
        })();
    })();
    __ASSETS._fci8flfdgc766r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div id="new-items-data" class="grid-container not-transparent items-container relative">');
        (function() {
            var $$obj = gradingInfo.getFinalGradeAnnType();
            if ("number" == typeof $$obj.length) {
                for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                    var item = $$obj[i];
                    buf.push("<div" + jade.attrs({
                        index: item.getIndex(),
                        "class": (item.getValue() ? "active" : "") + " " + "announcement-type-item" + " " + "gradient"
                    }, {
                        index: true
                    }) + '><div><div class="left name">' + jade.escape(null == (jade.interp = item.getTypeName()) ? "" : jade.interp) + '</div><div class="right relative"><input' + jade.attrs({
                        typeId: item.getId(),
                        value: item.getValue() + "%",
                        "class": "announcement-type-weight"
                    }, {
                        typeId: true,
                        value: true
                    }) + '/><div class="hint absolute">' + jade.escape(null == (jade.interp = Msg.Weight) ? "" : jade.interp) + '</div></div><div class="right type-container">');
                    jade.globals.Select_mixin.call({
                        buf: buf,
                        block: function() {
                            (function() {
                                var $$obj = gradingTypes;
                                if ("number" == typeof $$obj.length) {
                                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                        var gsItem = $$obj[$index];
                                        jade.globals.Option_mixin.call({
                                            buf: buf
                                        }, gsItem[0], gsItem[1], item.getGradingStyle() == gsItem[0]);
                                    }
                                } else {
                                    var $$l = 0;
                                    for (var $index in $$obj) {
                                        $$l++;
                                        var gsItem = $$obj[$index];
                                        jade.globals.Option_mixin.call({
                                            buf: buf
                                        }, gsItem[0], gsItem[1], item.getGradingStyle() == gsItem[0]);
                                    }
                                }
                            }).call(this);
                        }
                    }, "grading-type");
                    buf.push('</div><div class="right relative drop-lowest-block"><div class="hint absolute">' + jade.escape(null == (jade.interp = Msg.Drop_lowest) ? "" : jade.interp) + "</div>");
                    jade.globals.SlideCheckbox_mixin.call({
                        buf: buf,
                        attributes: {
                            "class": "drop-lowest"
                        },
                        escaped: {}
                    }, "dropLowest" + i, item.isDropLowest());
                    buf.push("</div></div></div>");
                }
            } else {
                var $$l = 0;
                for (var i in $$obj) {
                    $$l++;
                    var item = $$obj[i];
                    buf.push("<div" + jade.attrs({
                        index: item.getIndex(),
                        "class": (item.getValue() ? "active" : "") + " " + "announcement-type-item" + " " + "gradient"
                    }, {
                        index: true
                    }) + '><div><div class="left name">' + jade.escape(null == (jade.interp = item.getTypeName()) ? "" : jade.interp) + '</div><div class="right relative"><input' + jade.attrs({
                        typeId: item.getId(),
                        value: item.getValue() + "%",
                        "class": "announcement-type-weight"
                    }, {
                        typeId: true,
                        value: true
                    }) + '/><div class="hint absolute">' + jade.escape(null == (jade.interp = Msg.Weight) ? "" : jade.interp) + '</div></div><div class="right type-container">');
                    jade.globals.Select_mixin.call({
                        buf: buf,
                        block: function() {
                            (function() {
                                var $$obj = gradingTypes;
                                if ("number" == typeof $$obj.length) {
                                    for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                        var gsItem = $$obj[$index];
                                        jade.globals.Option_mixin.call({
                                            buf: buf
                                        }, gsItem[0], gsItem[1], item.getGradingStyle() == gsItem[0]);
                                    }
                                } else {
                                    var $$l = 0;
                                    for (var $index in $$obj) {
                                        $$l++;
                                        var gsItem = $$obj[$index];
                                        jade.globals.Option_mixin.call({
                                            buf: buf
                                        }, gsItem[0], gsItem[1], item.getGradingStyle() == gsItem[0]);
                                    }
                                }
                            }).call(this);
                        }
                    }, "grading-type");
                    buf.push('</div><div class="right relative drop-lowest-block"><div class="hint absolute">' + jade.escape(null == (jade.interp = Msg.Drop_lowest) ? "" : jade.interp) + "</div>");
                    jade.globals.SlideCheckbox_mixin.call({
                        buf: buf,
                        attributes: {
                            "class": "drop-lowest"
                        },
                        escaped: {}
                    }, "dropLowest" + i, item.isDropLowest());
                    buf.push("</div></div></div>");
                }
            }
        }).call(this);
        buf.push("</div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.grading.Final";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).grading = chlk.templates.grading || {};
        (function() {
            "use strict";
            chlk.templates.grading.Final = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.grading." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_fci8flfdgc766r") ], [ ria.templates.ModelBind(chlk.models.grading.Final) ], "Final", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.FinalGradeId, "id", [ ria.templates.ModelPropertyBind ], Number, "state", [ ria.templates.ModelPropertyBind ], chlk.models.class.Class, "clazz", [ ria.templates.ModelPropertyBind ], Number, "gradedStudentCount", [ ria.templates.ModelPropertyBind ], Number, "participation", [ ria.templates.ModelPropertyBind ], Number, "discipline", [ ria.templates.ModelPropertyBind ], Number, "dropLowestDiscipline", [ ria.templates.ModelPropertyBind ], Number, "attendance", [ ria.templates.ModelPropertyBind ], Number, "dropLowestAttendance", [ ria.templates.ModelPropertyBind ], Number, "gradingStyle", [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.grading.AnnouncementTypeGrading), "finalGradeAnnType", ria.__API.ArrayOf(chlk.models.grading.AnnouncementTypeFinal), "finalGradeAnnTypeSend", [ ria.templates.ModelPropertyBind ], String, "finalGradeAnnouncementTypeIds", [ ria.templates.ModelPropertyBind ], String, "percents", [ ria.templates.ModelPropertyBind ], Boolean, "changed", [ ria.templates.ModelPropertyBind ], String, "submitType", [ ria.templates.ModelPropertyBind ], String, "dropLowest", [ ria.templates.ModelPropertyBind ], String, "gradingStyleByType", [ ria.templates.ModelPropertyBind ], Boolean, "needsTypesForClasses", [ ria.templates.ModelPropertyBind ], Number, "nextClassNumber" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.setup.TeacherSettings";
    "chlk.templates.grading.Final";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).setup = chlk.activities.setup || {};
        (function() {
            chlk.activities.setup.TeacherSettingsPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.setup." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.PartialUpdateRule(chlk.templates.grading.Final, "", ".ann-types-container", ria.mvc.PartialUpdateRuleActions.Replace) ], [ ria.mvc.TemplateBind(chlk.templates.setup.TeacherSettings) ], "TeacherSettingsPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("click", ".next-class") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function nextButtonClick(el, event) {
                var finalGradeAnnouncementTypeIds = [], percents = [], dropLowest = [], gradingStyleByType = [];
                new ria.dom.Dom("#new-items-data").find(".announcement-type-item").forEach(function(node) {
                    var index = node.getAttr("index");
                    var weightNode = node.find(".announcement-type-weight");
                    finalGradeAnnouncementTypeIds[index] = weightNode.getAttr("typeId");
                    percents[index] = parseInt(weightNode.getValue(), 10);
                    dropLowest[index] = node.find("[type=checkbox]").checked();
                    gradingStyleByType[index] = node.find("select").getValue();
                });
                this.dom.find("input[name=participation]").setValue(parseInt(this.dom.find("#participation-value").getValue(), 10));
                this.dom.find("input[name=attendance]").setValue(parseInt(this.dom.find("#attendance-value").getValue(), 10));
                this.dom.find("input[name=discipline]").setValue(parseInt(this.dom.find("#discipline-value").getValue(), 10));
                this.dom.find("input[name=finalGradeAnnouncementTypeIds]").setValue(finalGradeAnnouncementTypeIds.join(","));
                this.dom.find("input[name=percents]").setValue(percents.join(","));
                this.dom.find("input[name=dropLowest]").setValue(dropLowest.join(","));
                this.dom.find("input[name=gradingStyleByType]").setValue(gradingStyleByType.join(","));
            }, [ ria.mvc.DomEventBind("click", "#tell-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function tellButtonClick(el, event) {
                this.dom.find("#setup-schedule-view").addClass("opacity0");
                this.dom.find("#settings-step1").addClass("x-hidden");
                this.dom.find("#settings-step2, #settings-step3").removeClass("x-hidden");
                var node = this.dom.find(".percents-count");
                this.checkPercents();
            }, ria.__SYNTAX.Modifiers.VOID, function checkPercents() {
                var sum = 0;
                this.dom.find(".announcement-type-item:not(.clone)").find(".announcement-type-weight").forEach(function(node) {
                    sum += parseInt(node.getValue(), 10);
                });
                var node = this.dom.find(".percents-count");
                if (sum == 100) {
                    this.dom.find(".next-class").setAttr("disabled", false);
                    node.removeClass("error").setHTML("100%").setAttr("data-tooltip", false).setData("tooltip", false);
                    setTimeout(function() {
                        jQuery(node.valueOf()[0]).trigger("mouseleave");
                    }, 10);
                } else {
                    var text = sum > 100 ? "Remove " + (sum - 100) + "%" : "Add " + (100 - sum) + "%";
                    this.dom.find(".next-class").setAttr("disabled", true);
                    node.addClass("error").setAttr("data-tooltip", text).setData("tooltip", text).setHTML(sum + "%").triggerEvent("mouseover");
                }
            }, [ ria.mvc.DomEventBind("change", "input, select") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function globalChange(el, event) {
                this.dom.find("[name=changed]").setValue(true);
            }, [ ria.mvc.DomEventBind("change", ".announcement-type-weight") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function weightChange(el, event) {
                function animateDiv(active) {
                    var height = 53, allHeight = 76;
                    var clone = item.clone();
                    clone.css("position", "absolute").css("top", prevCount * allHeight + 3).appendTo(container).addClass("clone").find("input.announcement-type").val(value);
                    var clone2 = item.clone();
                    clone2.addClass("opacity0").css("height", 0).addClass("clone").find("input.announcement-type").val(value);
                    last[0] ? last.after(clone2) : container.prepend(clone2);
                    item.addClass("opacity0");
                    clone.animate({
                        top: (activeCount - (active ? 1 : 0)) * allHeight + 3 + "px"
                    }, duration);
                    item.animate({
                        height: 0
                    }, duration);
                    clone2.animate({
                        height: height
                    }, duration + 1, function() {
                        var select = new chlk.controls.SelectControl();
                        clone2.remove();
                        clone.remove();
                        item = item.remove();
                        if (active) {
                            item.removeClass("active");
                        }
                        item.removeClass("opacity0");
                        item.height("auto").find(".chzn-container").remove();
                        last.after(item);
                        var oldSelect = item.find("select[name=grading-type]").removeClass("chzn-done");
                        select.updateSelect(oldSelect);
                    });
                }
                var parseVal = parseInt(el.getValue(), 10), form = new ria.dom.Dom("#update-ann-type-submit-form");
                el.setValue(parseVal && parseVal > 0 ? parseVal : 0);
                var node = jQuery(el.valueOf());
                var item = node.parents(".announcement-type-item");
                if (node.parents("#new-items-data")[0]) {
                    var value = el.getValue() + "%";
                    var container = node.parents(".items-container");
                    var activeCount = container.find(".announcement-type-item.active").length;
                    var prevCount = item.prevAll().length;
                    var last = container.find(".announcement-type-item.active:last");
                    var duration = 100 * (Math.abs(activeCount - prevCount) || 1);
                    if (el.getValue() > 0) {
                        if (!item.hasClass("active")) {
                            item.addClass("active");
                            if (prevCount != activeCount) {
                                animateDiv();
                            }
                        }
                    } else {
                        if (item.hasClass("active")) {
                            if (prevCount == activeCount - 1) {
                                item.removeClass("active");
                            } else {
                                animateDiv(true);
                            }
                        }
                    }
                } else {
                    if (el.getValue() > 0) {
                        item.addClass("active");
                    } else {
                        item.removeClass("active");
                    }
                }
                setTimeout(function() {
                    item.next().find("input.announcement-type-weight").focus();
                }, 1);
                el.setValue(el.getValue() + "%");
                this.checkPercents();
            } ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.TeacherService";
    "chlk.services.PersonService";
    "chlk.services.FinalGradeService";
    "chlk.services.CalendarService";
    "chlk.services.ClassService";
    "chlk.services.PreferenceService";
    "chlk.activities.setup.HelloPage";
    "chlk.activities.setup.VideoPage";
    "chlk.activities.setup.StartPage";
    "chlk.activities.setup.TeacherSettingsPage";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.people.User";
    "chlk.models.setup.TeacherSettings";
    "chlk.models.grading.Final";
    "chlk.models.settings.Preference";
    "chlk.models.grading.AnnouncementTypeFinal";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.SetupController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SetupController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.TeacherService, "teacherService", [ ria.mvc.Inject ], chlk.services.PersonService, "personService", [ ria.mvc.Inject ], chlk.services.FinalGradeService, "finalGradeService", [ ria.mvc.Inject ], chlk.services.CalendarService, "calendarService", [ ria.mvc.Inject ], chlk.services.ClassService, "classService", [ ria.mvc.Inject ], chlk.services.PreferenceService, "preferenceService", [ [ chlk.models.people.User ] ], function prepareProfileData(model) {
                var phones = model.getPhones(), addresses = model.getAddresses(), phonesValue = [], addressesValue = [];
                phones.forEach(function(item) {
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        isPrimary: item.isIsPrimary(),
                        value: item.getValue()
                    };
                    phonesValue.push(values);
                    if (item.isIsPrimary() && !model.getPrimaryPhone()) {
                        model.setPrimaryPhone(item);
                    } else {
                        if (!model.getHomePhone()) model.setHomePhone(item);
                    }
                });
                addresses.forEach(function(item) {
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        value: item.getValue()
                    };
                    addressesValue.push(values);
                });
                model.setPhonesValue(JSON.stringify(phonesValue));
                model.setAddressesValue(JSON.stringify(addressesValue));
                return model;
            }, [ [ chlk.models.id.SchoolPersonId ] ], function helloAction(personId_) {
                var result = this.teacherService.getInfo(personId_ || this.getContext().getSession().get("currentPerson").getId()).attach(this.validateResponse_()).then(function(model) {
                    return this.prepareProfileData(model);
                }.bind(this));
                return this.PushView(chlk.activities.setup.HelloPage, result);
            }, [ [ chlk.models.id.SchoolPersonId ] ], function videoAction(personId_) {
                var result = this.preferenceService.getPublic(chlk.models.settings.PreferenceEnum.VIDEO_GETTING_INFO_CHALKABLE.valueOf()).attach(this.validateResponse_()).then(function(model) {
                    model = model || new chlk.models.settings.Preference();
                    var classes = this.classService.getClassesForTopBar();
                    model.setType(classes.length);
                    return model;
                }.bind(this));
                return this.PushView(chlk.activities.setup.VideoPage, result);
            }, [ [ chlk.models.id.SchoolPersonId ] ], function startAction(personId_) {
                var model = chlk.models.settings.Preference();
                var classes = this.classService.getClassesForTopBar();
                model.setType(classes.length);
                var result = ria.async.DeferredData(model);
                return this.PushView(chlk.activities.setup.StartPage, result);
            }, [ [ Number ] ], function teacherSettingsAction(index) {
                var classes = this.classService.getClassesForTopBar();
                var classId = classes[index].getId();
                var result = ria.async.wait([ this.finalGradeService.getFinalGrades(classId, false), this.calendarService.getTeacherClassWeek(classId) ]).then(function(result) {
                    var model = new chlk.models.setup.TeacherSettings();
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(true);
                    topModel.setSelectedItemId(classId);
                    model.setTopData(topModel);
                    model.setCalendarInfo(result[1]);
                    var gradesInfo = result[0].getFinalGradeAnnType(), sum = 0;
                    gradesInfo.forEach(function(item, index) {
                        item.setIndex(index);
                        sum += item.getValue() || 0;
                    });
                    gradesInfo.sort(function(a, b) {
                        return b.getValue() > a.getValue();
                    });
                    result[0].setNextClassNumber(++index);
                    this.getContext().getSession().set("settingsModel", result[0]);
                    sum += result[0].getAttendance() || 0;
                    sum += result[0].getParticipation() || 0;
                    sum += result[0].getDiscipline() || 0;
                    model.setPercentsSum(sum);
                    model.setGradingInfo(result[0]);
                    return model;
                }.bind(this));
                return this.PushView(chlk.activities.setup.TeacherSettingsPage, result);
            }, [ [ chlk.models.grading.Final ] ], function teacherSettingsEditAction(model) {
                var index = model.getNextClassNumber();
                var submitType = model.getSubmitType();
                var backToVideo = index == 1;
                if (submitType == "back") {
                    var action = backToVideo ? "video" : "teacherSettings";
                    var params = backToVideo ? [] : [ index - 2 ];
                    if (model.isChanged()) {
                        return this.ShowMsgBox("Are you sure you want to go?", null, [ {
                            text: Msg.OK,
                            controller: "setup",
                            action: action,
                            params: params,
                            color: chlk.models.common.ButtonColor.RED.valueOf()
                        }, {
                            text: Msg.Cancel,
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                        } ], "center");
                    } else {
                        return this.redirect_("setup", action, params);
                    }
                } else {
                    if (submitType == "message") {
                        if (index > 1) this.ShowMsgBox("To make things easier we copy and\npaste your choices from the last page.\n\n" + "Click any number to change it.", "fyi.", [ {
                            text: Msg.GOT_IT.toUpperCase()
                        } ]);
                    } else {
                        var finalGradeAnnouncementTypes = [], item, ids = model.getFinalGradeAnnouncementTypeIds().split(","), percents = model.getPercents().split(","), dropLowest = model.getDropLowest().split(","), gradingStyle = model.getGradingStyleByType().split(",");
                        ids.forEach(function(id, i) {
                            item = {};
                            item.finalGradeAnnouncementTypeId = id;
                            item.percentValue = JSON.parse(percents[i]);
                            item.dropLowest = JSON.parse(dropLowest[i]);
                            item.gradingStyle = JSON.parse(gradingStyle[i]);
                            finalGradeAnnouncementTypes.push(item);
                        });
                        var classes = this.classService.getClassesForTopBar();
                        this.finalGradeService.update(model.getId(), model.getParticipation(), model.getAttendance(), model.getDropLowestAttendance(), model.getDiscipline(), model.getDropLowestDiscipline(), model.getGradingStyle(), finalGradeAnnouncementTypes, model.isNeedsTypesForClasses()).then(function(model) {
                            if (index < classes.length) {
                                this.redirect_("setup", "teacherSettings", [ index ]);
                            } else {
                                this.redirect_("setup", "start", []);
                            }
                        }.bind(this));
                        return this.StartLoading(chlk.activities.setup.TeacherSettingsPage);
                    }
                }
            }, [ [ chlk.models.people.User ] ], function infoEditAction(model) {
                this.personService.changePassword(model.getId(), model.getPassword()).then(function(changed) {
                    this.teacherService.updateInfo(model.getId(), model.getAddressesValue(), model.getEmail(), model.getFirstName(), model.getLastName(), model.getGender(), model.getPhonesValue(), model.getSalutation(), model.getBirthDate()).attach(this.validateResponse_()).then(function(model) {
                        this.StopLoading(chlk.activities.setup.HelloPage);
                        return this.redirect_("setup", "video", [ model.getId().valueOf() ]);
                    }.bind(this));
                }.bind(this));
                return this.StartLoading(chlk.activities.setup.HelloPage);
            } ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.id.ClassId";
    "chlk.models.id.SchoolPersonId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.StudentService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StudentService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ chlk.models.id.ClassId, String, Boolean, Boolean, Number, Number ] ], ria.async.Future, function getStudents(classId_, filter_, myStudentsOnly_, byLastName_, start_, count_) {
                return this.getPaginatedList("Student/GetStudents.json", chlk.models.people.User, {
                    classId: classId_ && classId_.valueOf(),
                    filter: filter_,
                    myStudentsOnly: myStudentsOnly_,
                    byLastName: byLastName_,
                    start: start_,
                    count: count_
                });
            }, [ [ chlk.models.id.SchoolPersonId ] ], ria.async.Future, function getInfo(personId) {
                return this.get("Student/Info.json", chlk.models.people.User, {
                    personId: personId.valueOf()
                });
            }, [ [ chlk.models.id.SchoolPersonId, String, String, String, String, String, String, String, chlk.models.common.ChlkDate ] ], ria.async.Future, function updateInfo(personId, addresses, email, firstName, lastName, gender, phones, salutation, birthDate) {
                return this.post("Student/UpdateInfo.json", chlk.models.people.User, {
                    personId: personId.valueOf(),
                    addresses: addresses && JSON.parse(addresses),
                    email: email,
                    firstName: firstName,
                    lastName: lastName,
                    gender: gender,
                    phones: phones && JSON.parse(phones),
                    salutation: salutation,
                    birthdayDate: birthDate && JSON.stringify(birthDate.getDate()).slice(1, -1)
                });
            } ]);
        })();
    })();
    __ASSETS._32u82zj56k73nmi = function anonymous(locals) {
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
    "chlk.models.people.UsersList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).people = chlk.templates.people || {};
        (function() {
            chlk.templates.people.UsersList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_32u82zj56k73nmi") ], [ ria.templates.ModelBind(chlk.models.people.UsersList) ], "UsersList", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.common.PaginatedList, "users", [ ria.templates.ModelPropertyBind ], Number, "selectedIndex", [ ria.templates.ModelPropertyBind ], Boolean, "byLastName", [ ria.templates.ModelPropertyBind ], String, "filter", [ ria.templates.ModelPropertyBind ], Number, "start" ]);
        })();
    })();
    __ASSETS._tecdw4kface1xlxr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        (function() {
            var $$obj = self.getItems();
            if ("number" == typeof $$obj.length) {
                for (var i = 0, $$l = $$obj.length; i < $$l; i++) {
                    var item = $$obj[i];
                    index = item.getIndex ? item.getIndex() || i : i;
                    buf.push("<div" + jade.attrs({
                        index: index,
                        "class": "row" + " " + (index == 0 ? "selected" : "")
                    }, {
                        "class": true,
                        index: true
                    }) + ">");
                    jade.globals.ActionLink_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push("<img" + jade.attrs({
                                src: item.getPictureUrl(),
                                "class": "avatar" + " " + "avatar47" + " " + item.getGender()
                            }, {
                                "class": true,
                                src: true
                            }) + "/>");
                        },
                        attributes: {
                            "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                        },
                        escaped: {}
                    }, "students", "info", item.getId().valueOf());
                    buf.push('<p class="left">' + jade.escape(null == (jade.interp = item.getFullName()) ? "" : jade.interp) + '</p><div class="right">');
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push("Message");
                        },
                        attributes: {
                            "class": "message-button" + " " + "blue-gradient-btn"
                        },
                        escaped: {}
                    });
                    buf.push('</div><div class="clear"></div></div>');
                }
            } else {
                var $$l = 0;
                for (var i in $$obj) {
                    $$l++;
                    var item = $$obj[i];
                    index = item.getIndex ? item.getIndex() || i : i;
                    buf.push("<div" + jade.attrs({
                        index: index,
                        "class": "row" + " " + (index == 0 ? "selected" : "")
                    }, {
                        "class": true,
                        index: true
                    }) + ">");
                    jade.globals.ActionLink_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push("<img" + jade.attrs({
                                src: item.getPictureUrl(),
                                "class": "avatar" + " " + "avatar47" + " " + item.getGender()
                            }, {
                                "class": true,
                                src: true
                            }) + "/>");
                        },
                        attributes: {
                            "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                        },
                        escaped: {}
                    }, "students", "info", item.getId().valueOf());
                    buf.push('<p class="left">' + jade.escape(null == (jade.interp = item.getFullName()) ? "" : jade.interp) + '</p><div class="right">');
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push("Message");
                        },
                        attributes: {
                            "class": "message-button" + " " + "blue-gradient-btn"
                        },
                        escaped: {}
                    });
                    buf.push('</div><div class="clear"></div></div>');
                }
            }
        }).call(this);
        return buf.join("");
    };
    "chlk.templates.PaginatedList";
    "chlk.models.common.PaginatedList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).people = chlk.templates.people || {};
        (function() {
            chlk.templates.people.UsersForGrid = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_tecdw4kface1xlxr") ], [ ria.templates.ModelBind(chlk.models.common.PaginatedList) ], "UsersForGrid", ria.__SYNTAX.EXTENDS(chlk.templates.PaginatedList), []);
        })();
    })();
    __ASSETS._f21a6zhhej7tlnmi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="loader"></div><input' + jade.attrs({
            type: "hidden",
            name: "start",
            value: self.getStart() || 0
        }, {
            type: true,
            name: true,
            value: true
        }) + '/><Button type="submit" name="submitType" value="scroll" class="scroll-start-button x-hidden"></Button>');
        jade.globals.ListView_mixin.call({
            buf: buf,
            block: function() {
                jade.globals.RenderWith_mixin.call({
                    buf: buf
                }, self.getUsers(), chlk.templates.people.UsersForGrid);
            },
            attributes: {
                selectedIndex: selectedIndex,
                infiniteScroll: true,
                "class": "people-list"
            },
            escaped: {
                selectedIndex: true,
                infiniteScroll: true
            }
        }, self.getUsers());
        return buf.join("");
    };
    "chlk.templates.people.UsersList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).people = chlk.templates.people || {};
        (function() {
            chlk.templates.people.UsersGrid = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_f21a6zhhej7tlnmi") ], [ ria.templates.ModelBind(chlk.models.people.UsersList) ], "UsersGrid", ria.__SYNTAX.EXTENDS(chlk.templates.people.UsersList), []);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.people.UsersList";
    "chlk.templates.people.UsersForGrid";
    "chlk.templates.people.UsersGrid";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).person = chlk.activities.person || {};
        (function() {
            chlk.activities.person.PersonGrid = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.person." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.people.UsersList) ], "PersonGrid", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("mouseover mouseleave", ".people-search-img") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function imgHover(node, event) {
                this.dom.find(".people-search").toggleClass("hovered");
            }, [ ria.mvc.DomEventBind("focus blur", ".people-search") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function filterFocus(node, event) {
                node.toggleClass("hovered");
            }, [ [ ria.dom.Dom ] ], ria.__SYNTAX.Modifiers.VOID, function submitFormWithStart(node) {
                var form = node.parent("form");
                form.find("[name=start]").setValue(0);
                form.triggerEvent("submit");
            }, ria.__SYNTAX.Modifiers.VOID, function clearSearch() {
                var node = this.dom.find(".people-search");
                node.setValue("");
                this.submitFormWithStart(node);
                this.dom.find(".people-search-img").removeClass("opacity0");
                this.dom.find(".people-search-close").addClass("opacity0");
            }, [ ria.mvc.DomEventBind("keyup", ".people-search") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function filterKeyUp(node, event) {
                var value = node.getValue();
                if (value.length > 1) {
                    this.submitFormWithStart(node);
                    this.dom.find(".people-search-img").addClass("opacity0");
                    this.dom.find(".people-search-close").removeClass("opacity0");
                } else {
                    if (!value.length) this.clearSearch();
                }
            }, [ ria.mvc.DomEventBind("click", ".first-last:not(.pressed)") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function firstLastClick(node, event) {
                var value = node.getData("value");
                this.dom.find("input[name=byLastName]").setValue(value);
                this.dom.find(".first-last.pressed").removeClass("pressed");
                node.addClass("pressed");
                this.submitFormWithStart(node);
            }, [ ria.mvc.DomEventBind("click", ".people-search-close") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function closeSearchClick(node, event) {
                this.clearSearch();
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.__SYNTAX.Modifiers.VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);
                if (model.getUsers) {
                    var count = model.getUsers().getTotalCount();
                    this.dom.find(".total-count").setHTML(count + Msg.Person(count != 1));
                }
            } ]);
        })();
    })();
    __ASSETS._kaf49n1ohkf0f6r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="students-list">');
        topData = self.getTopData();
        topItems = topData.getTopItems();
        selectedId = topData.getSelectedItemId();
        jade.globals.LeftRightToolbar_mixin.call({
            buf: buf,
            attributes: {
                selectedItemId: selectedId ? selectedId.valueOf() : "",
                "class": "classes-bar"
            },
            escaped: {
                selectedItemId: true
            }
        }, topItems, chlk.templates.class.TopBar, "students", self.isMy() ? "my" : "all", []);
        buf.push('<div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="right">');
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push(jade.escape(null == (jade.interp = Msg.My_Students) ? "" : jade.interp));
            },
            attributes: {
                "class": self.isMy() ? "pressed" : ""
            },
            escaped: {
                "class": true
            }
        }, "students", "my", []);
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push(jade.escape(null == (jade.interp = Msg.Whole_School) ? "" : jade.interp));
            },
            attributes: {
                "class": self.isMy() ? "" : "pressed"
            },
            escaped: {
                "class": true
            }
        }, "students", "all", []);
        buf.push("</div></div></div>");
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push("<input" + jade.attrs({
                    type: "hidden",
                    name: "my",
                    value: self.isMy()
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + "/><input" + jade.attrs({
                    type: "hidden",
                    name: "classId",
                    value: self.getTopData().getSelectedItemId()
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/><div class="users-list-container">');
                jade.globals.RenderWith_mixin.call({
                    buf: buf
                }, self.getUsersPart(), chlk.templates.people.UsersList);
                buf.push("</div>");
            },
            attributes: {
                id: "people-list-form"
            },
            escaped: {}
        }, "students", "updateList");
        buf.push("</div>");
        return buf.join("");
    };
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.UsersListSubmit = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("UsersListSubmit", [ Number, "selectedIndex", Boolean, "byLastName", String, "filter", Number, "start", String, "submitType" ]);
        })();
    })();
    "chlk.models.people.UsersList";
    "chlk.models.id.ClassId";
    "chlk.models.people.UsersListSubmit";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).teacher = chlk.models.teacher || {};
        (function() {
            "use strict";
            chlk.models.teacher.StudentsList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.teacher." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StudentsList", ria.__SYNTAX.EXTENDS(chlk.models.people.UsersListSubmit), [ chlk.models.people.UsersList, "usersPart", chlk.models.class.ClassesForTopBar, "topData", Boolean, "my", chlk.models.id.ClassId, "classId" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.teacher.StudentsList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).teacher = chlk.templates.teacher || {};
        (function() {
            "use strict";
            chlk.templates.teacher.StudentsList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.teacher." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_kaf49n1ohkf0f6r") ], [ ria.templates.ModelBind(chlk.models.teacher.StudentsList) ], "StudentsList", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.people.UsersList, "usersPart", [ ria.templates.ModelPropertyBind ], chlk.models.class.ClassesForTopBar, "topData", [ ria.templates.ModelPropertyBind ], Boolean, "my" ]);
        })();
    })();
    "chlk.activities.person.PersonGrid";
    "chlk.templates.teacher.StudentsList";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).person = chlk.activities.person || {};
        (function() {
            chlk.activities.person.ListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.person." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.teacher.StudentsList) ], [ ria.mvc.PartialUpdateRule(chlk.templates.people.UsersGrid, "", ".grid-container", ria.mvc.PartialUpdateRuleActions.Replace) ], [ ria.mvc.PartialUpdateRule(chlk.templates.people.UsersForGrid, window.noLoadingMsg, ".people-list", ria.mvc.PartialUpdateRuleActions.Append) ], "ListPage", ria.__SYNTAX.EXTENDS(chlk.activities.person.PersonGrid), []);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.StudentService";
    "chlk.services.PersonService";
    "chlk.services.ClassService";
    "chlk.activities.person.ListPage";
    "chlk.models.id.ClassId";
    "chlk.models.teacher.StudentsList";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.StudentsController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("StudentsController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.StudentService, "studentService", [ ria.mvc.Inject ], chlk.services.PersonService, "personService", [ ria.mvc.Inject ], chlk.services.ClassService, "classService", [ [ chlk.models.common.PaginatedList, Number, Boolean, String ] ], chlk.models.people.UsersList, function prepareUsersModel(users, selectedIndex, byLastName, filter_) {
                users = this.prepareUsers(users);
                var usersModel = new chlk.models.people.UsersList();
                usersModel.setSelectedIndex(selectedIndex);
                usersModel.setByLastName(byLastName);
                filter_ && usersModel.setFilter(filter_);
                usersModel.setUsers(users);
                return usersModel;
            }, [ [ chlk.models.common.PaginatedList, Number ] ], chlk.models.common.PaginatedList, function prepareUsers(usersData, start_) {
                var start = start_ || 0;
                usersData.getItems().forEach(function(item, index) {
                    item.setIndex(start_ + index);
                    item.setPictureUrl(this.personService.getPictureURL(item.getId(), 47));
                }.bind(this));
                return usersData;
            }, [ [ Boolean, chlk.models.id.ClassId ] ], function showStudentsList(isMy, classId_) {
                var result = this.studentService.getStudents(classId_, null, isMy, true, 0, 10).then(function(users) {
                    var model = new chlk.models.teacher.StudentsList();
                    var usersModel = this.prepareUsersModel(users, 0, true);
                    var classes = this.classService.getClassesForTopBar(true);
                    model.setUsersPart(usersModel);
                    model.setMy(isMy);
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(false);
                    classId_ && topModel.setSelectedItemId(classId_);
                    model.setTopData(topModel);
                    return model;
                }.bind(this));
                return this.PushView(chlk.activities.person.ListPage, result);
            }, [ chlk.controllers.SidebarButton("people") ], [ [ chlk.models.id.ClassId ] ], function myAction(classId_) {
                return this.showStudentsList(true, classId_);
            }, [ chlk.controllers.SidebarButton("people") ], [ [ chlk.models.id.ClassId ] ], function allAction(classId_) {
                return this.showStudentsList(false, classId_);
            }, [ [ chlk.models.teacher.StudentsList ] ], function updateListAction(model) {
                var submitType = model.getSubmitType();
                var isScroll = submitType == "scroll", start = model.getStart();
                var result = this.studentService.getStudents(model.getClassId(), model.getFilter(), model.isMy(), model.isByLastName(), start, 10).then(function(usersData) {
                    if (isScroll) {
                        usersData = this.prepareUsers(usersData, start);
                        return usersData;
                    } else {
                        var usersModel = this.prepareUsersModel(usersData, 0, model.isByLastName(), model.getFilter());
                        return usersModel;
                    }
                }.bind(this));
                return this.UpdateView(chlk.activities.person.ListPage, result, isScroll ? window.noLoadingMsg : "");
            }, [ [ chlk.models.id.SchoolPersonId ] ], function infoAction(personId) {} ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.people.User";
    "chlk.templates.people.User";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).profile = chlk.templates.profile || {};
        (function() {
            chlk.templates.profile.InfoView = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.profile." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_xvrtoehzmlutmx6r") ], [ ria.templates.ModelBind(chlk.models.people.User) ], "InfoView", ria.__SYNTAX.EXTENDS(chlk.templates.people.User), [ [ ria.templates.ModelPropertyBind ], Boolean, "ableEdit" ]);
        })();
    })();
    __ASSETS._1mqbjxjh908jjor = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        (function() {
            var $$obj = self.getItems();
            if ("number" == typeof $$obj.length) {
                for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                    var address = $$obj[$index];
                    buf.push("<textarea" + jade.attrs({
                        "data-id": address.getId() ? address.getId().valueOf() : null,
                        "data-type": 0,
                        "class": "home-address"
                    }, {
                        "class": true,
                        "data-id": true,
                        "data-type": true
                    }) + ">" + jade.escape(null == (jade.interp = address.getValue()) ? "" : jade.interp) + "</textarea>");
                }
            } else {
                var $$l = 0;
                for (var $index in $$obj) {
                    $$l++;
                    var address = $$obj[$index];
                    buf.push("<textarea" + jade.attrs({
                        "data-id": address.getId() ? address.getId().valueOf() : null,
                        "data-type": 0,
                        "class": "home-address"
                    }, {
                        "class": true,
                        "data-id": true,
                        "data-type": true
                    }) + ">" + jade.escape(null == (jade.interp = address.getValue()) ? "" : jade.interp) + "</textarea>");
                }
            }
        }).call(this);
        return buf.join("");
    };
    "chlk.models.people.Address";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.Addresses = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Addresses", [ ria.__API.ArrayOf(chlk.models.people.Address), "items" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.people.Addresses";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).people = chlk.templates.people || {};
        (function() {
            chlk.templates.people.Addresses = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_1mqbjxjh908jjor") ], [ ria.templates.ModelBind(chlk.models.people.Addresses) ], "Addresses", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], ria.__API.ArrayOf(chlk.models.people.Address), "items" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.profile.InfoView";
    "chlk.templates.people.Addresses";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).profile = chlk.activities.profile || {};
        (function() {
            var serializer = new ria.serialize.JsonSerializer();
            chlk.activities.profile.InfoViewPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.profile." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ chlk.activities.lib.PageClass("profile") ], [ ria.mvc.PartialUpdateRule(chlk.templates.people.Addresses, "", ".adresses", ria.mvc.PartialUpdateRuleActions.Replace) ], [ ria.mvc.PartialUpdateRule(chlk.templates.profile.InfoView, "", null, ria.mvc.PartialUpdateRuleActions.Replace) ], [ ria.mvc.TemplateBind(chlk.templates.profile.InfoView) ], "InfoViewPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("click", ".add-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function addAddressClick(node, event) {
                var addressesValue = this.getAddresses();
                addressesValue.push({
                    id: null,
                    type: 0,
                    value: ""
                });
                var addressesModel = serializer.deserialize({
                    items: addressesValue
                }, chlk.models.people.Addresses);
                this.onPartialRender_(addressesModel);
            }, Array, function getAddresses() {
                var addressesNodes = this.dom.find(".home-address").valueOf();
                var len = addressesNodes.length, i, addressesValue = [], node;
                for (i = 0; i < len; i++) {
                    node = new ria.dom.Dom(addressesNodes[i]);
                    addressesValue.push({
                        id: node.getData("id"),
                        value: node.getValue(),
                        type: node.getData("type")
                    });
                }
                return addressesValue;
            }, Array, function getPhones() {
                var primaryNode = this.dom.find(".primary-phone");
                var homeNode = this.dom.find(".home-phone");
                var res = [];
                res.push({
                    id: primaryNode.getData("id"),
                    value: primaryNode.getValue(),
                    type: primaryNode.getData("type"),
                    isPrimary: true
                });
                res.push({
                    id: homeNode.getData("id"),
                    value: homeNode.getValue(),
                    type: homeNode.getData("type"),
                    isPrimary: false
                });
                return res;
            }, [ ria.mvc.DomEventBind("click", "#submit-info-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function submitClick(node, event) {
                var addressesNode = this.dom.find('input[name="addressesValue"]');
                var phonesNode = this.dom.find('input[name="phonesValue"]');
                addressesNode.setValue(JSON.stringify(this.getAddresses()));
                phonesNode.setValue(JSON.stringify(this.getPhones()));
            }, [ ria.mvc.DomEventBind("click", "#edit-info-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function editClick(node, event) {
                this.dom.find(".view-mode").removeClass("view-mode").addClass("edit-mode");
            }, [ ria.mvc.DomEventBind("click", "#cancell-edit-info-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function cancelClick(node, event) {
                this.dom.find(".edit-mode").removeClass("edit-mode").addClass("view-mode");
            } ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.services.TeacherService";
    "chlk.services.PersonService";
    "chlk.activities.profile.InfoViewPage";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.people.User";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.TeachersController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TeachersController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.TeacherService, "teacherService", [ ria.mvc.Inject ], chlk.services.PersonService, "personService", [ [ chlk.models.people.User ] ], function prepareProfileData(model) {
                var bDate = model.getBirthDate(), res = "";
                if (bDate) {
                    var day = parseInt(bDate.format("d"), 10), str;
                    switch (day) {
                      case 1:
                        str = "st";
                        break;

                      case 2:
                        str = "n&#100;";
                        break;

                      case 3:
                        str = "r&#100;";
                        break;

                      default:
                        str = "st";
                    }
                    res = "M d" + str + " y";
                }
                var phones = model.getPhones(), addresses = model.getAddresses(), phonesValue = [], addressesValue = [];
                phones.forEach(function(item) {
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        isPrimary: item.isIsPrimary(),
                        value: item.getValue()
                    };
                    phonesValue.push(values);
                    if (item.isIsPrimary() && !model.getPrimaryPhone()) {
                        model.setPrimaryPhone(item);
                    } else {
                        if (!model.getHomePhone()) model.setHomePhone(item);
                    }
                });
                addresses.forEach(function(item) {
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        value: item.getValue()
                    };
                    addressesValue.push(values);
                });
                model.setPhonesValue(JSON.stringify(phonesValue));
                model.setAddressesValue(JSON.stringify(addressesValue));
                var roleName = model.getRole().getName(), roles = chlk.models.common.RoleEnum;
                var currentPerson = this.getContext().getSession().get("currentPerson");
                model.setAbleEdit(roleName != roles.STUDENT.valueOf() && model.getId() == currentPerson.getId() || roleName == roles.ADMINEDIT.valueOf() || roleName == roles.ADMINGRADE.valueOf());
                bDate && model.setBirthDateText(bDate.toString(res).replace(/&#100;/g, "d"));
                var gt = model.getGender() ? model.getGender().toLowerCase() == "m" ? "Male" : "Female" : "";
                model.setGenderFullText(gt);
                model.setPictureUrl(this.personService.getPictureURL(model.getId(), 128));
                return model;
            }, [ [ chlk.models.id.SchoolPersonId ] ], function infoAction(personId) {
                var result = this.teacherService.getInfo(personId).attach(this.validateResponse_()).then(function(model) {
                    var res = this.prepareProfileData(model);
                    this.getContext().getSession().set("userModel", res);
                    return res;
                }.bind(this));
                return this.PushView(chlk.activities.profile.InfoViewPage, result);
            }, [ [ chlk.models.people.User ] ], function infoEditAction(model) {
                var result;
                result = this.teacherService.updateInfo(model.getId(), model.getAddressesValue(), model.getEmail(), model.getFirstName(), model.getLastName(), model.getGender(), model.getPhonesValue(), model.getSalutation(), model.getBirthDate()).attach(this.validateResponse_()).then(function(model) {
                    return this.prepareProfileData(model);
                }.bind(this));
                return this.UpdateView(chlk.activities.profile.InfoViewPage, result);
            }, [ [ chlk.models.id.SchoolPersonId, Object ] ], function uploadPictureAction(personId, files) {
                var result = this.personService.uploadPicture(personId, files).then(function(loaded) {
                    return this.getContext().getSession().get("userModel");
                }.bind(this));
                return this.UpdateView(chlk.activities.profile.InfoViewPage, result);
            } ]);
        })();
    })();
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).id = chlk.models.id || {};
        (function() {
            "use strict";
            chlk.models.id.MessageId =             function wrapper() {
                var values = {};
                function MessageId(value) {
                    return values.hasOwnProperty(value) ? values[value] : values[value] = new MessageIdImpl(value);
                }
                ria.__API.identifier(MessageId, "chlk.models.id.MessageId");
                function MessageIdImpl(value) {
                    this.valueOf = function() {
                        return value;
                    };
                    this.toString = function toString() {
                        return "[chlk.models.id.MessageId#" + value + "]";
                    };
                }
                ria.__API.extend(MessageIdImpl, MessageId);
                return MessageId;
            }();
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.people.User";
    "chlk.models.people.Phone";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).people = chlk.models.people || {};
        (function() {
            "use strict";
            chlk.models.people.Person = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.people." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Person", ria.__SYNTAX.EXTENDS(chlk.models.people.User), []);
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.id.MessageId";
    "chlk.models.people.Person";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).messages = chlk.models.messages || {};
        (function() {
            "use strict";
            chlk.models.messages.Message = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("Message", [ chlk.models.id.MessageId, "id", chlk.models.common.ChlkDate, "sent", String, "subject", String, "body", Boolean, "read", [ ria.serialize.SerializeProperty("deletebysender") ], Boolean, "deleteBySender", [ ria.serialize.SerializeProperty("deletebyrecipient") ], Boolean, "deleteByRecipient", chlk.models.people.Person, "sender", chlk.models.people.Person, "recipient" ]);
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.id.SchoolPersonId";
    "chlk.models.people.Person";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).messages = chlk.models.messages || {};
        (function() {
            "use strict";
            chlk.models.messages.SendMessage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SendMessage", [ chlk.models.id.SchoolPersonId, "recipientId", String, "subject", String, "body" ]);
        })();
    })();
    "chlk.models.common.ChlkDate";
    "chlk.models.id.MessageId";
    "chlk.models.messages.Message";
    "chlk.models.people.Person";
    "chlk.models.common.PaginatedList";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).messages = chlk.models.messages || {};
        (function() {
            "use strict";
            chlk.models.messages.MessageList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("MessageList", [ chlk.models.common.PaginatedList, "messages", Boolean, "inbox", String, "role", String, "keyword", String, "selectedIds", String, "submitType" ]);
        })();
    })();
    __ASSETS._ug51na493mzpvi = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="dialog gray"><div class="x-window-header"><span>Send Message</span></div><div class="x-window-body"><div class="general-info">');
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="x-form-field"><label>To:</label>');
                jade.globals.SearchBox_mixin.call({
                    buf: buf,
                    attributes: {
                        id: "recipientId",
                        name: "recipientId",
                        value: self.sender ? self.sender.getFirstName() + " " + self.sender.getLastName() : "",
                        "default-value": self.sender ? self.sender.getId() : ""
                    },
                    escaped: {
                        name: true,
                        value: true,
                        "default-value": true
                    }
                }, chlk.services.PersonService, "getPersons", chlk.templates.messages.RecipientAutoComplete);
                buf.push('</div><div class="x-form-field"><label>Subject:</label><input' + jade.attrs({
                    type: "text",
                    name: "subject",
                    value: self.subject ? "Re: " + self.subject : ""
                }, {
                    type: true,
                    name: true,
                    value: true
                }) + '/></div><div class="x-form-field"><label>Body:</label><input type="text" name="body"/></div>');
                if (self.body) {
                    buf.push('<div class="x-form-field"><label>' + jade.escape(null == (jade.interp = "send: " + self.sent.toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + "</label><input" + jade.attrs({
                        type: "text",
                        name: "prev",
                        value: self.body
                    }, {
                        type: true,
                        name: true,
                        value: true
                    }) + "/></div>");
                }
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
        }, "message", "send");
        buf.push("</div></div></div>");
        return buf.join("");
    };
    __ASSETS._mrjis57ouco3l3di = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push("<li" + jade.attrs({
            "data-title": self.firstName + " " + self.lastName,
            "data-value": self.id.valueOf(),
            style: "background: white"
        }, {
            "data-title": true,
            "data-value": true,
            style: true
        }) + "><a><img" + jade.attrs({
            src: self.getPictureURL(self.id, 47),
            "class": "avatar" + " " + "avatar47" + " " + self.gender
        }, {
            "class": true,
            src: true
        }) + "/><div>" + jade.escape(null == (jade.interp = self.firstName) ? "" : jade.interp) + "</div><div>" + jade.escape(null == (jade.interp = self.lastName) ? "" : jade.interp) + "</div></a></li>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.people.User";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).messages = chlk.templates.messages || {};
        (function() {
            chlk.templates.messages.RecipientAutoComplete = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_mrjis57ouco3l3di") ], [ ria.templates.ModelBind(chlk.models.people.User) ], "RecipientAutoComplete", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], String, "displayName", [ ria.templates.ModelPropertyBind ], String, "email", [ ria.templates.ModelPropertyBind ], String, "firstName", [ ria.templates.ModelPropertyBind ], String, "fullName", [ ria.templates.ModelPropertyBind ], String, "gender", [ ria.templates.ModelPropertyBind ], chlk.models.id.SchoolPersonId, "id", [ ria.templates.ModelPropertyBind ], String, "lastName", [ ria.templates.ModelPropertyBind ], String, "salutation" ]);
        })();
    })();
    "chlk.templates.JadeTemplate";
    "chlk.models.messages.Message";
    "chlk.templates.messages.RecipientAutoComplete";
    "chlk.models.common.ChlkDate";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).messages = chlk.templates.messages || {};
        (function() {
            chlk.templates.messages.AddDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_ug51na493mzpvi") ], [ ria.templates.ModelBind(chlk.models.messages.Message) ], "AddDialog", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.MessageId, "id", [ ria.templates.ModelPropertyBind ], String, "subject", [ ria.templates.ModelPropertyBind ], String, "body", [ ria.templates.ModelPropertyBind ], chlk.models.people.Person, "sender", [ ria.templates.ModelPropertyBind ], chlk.models.people.Person, "recipient", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "sent" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.messages.AddDialog";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).messages = chlk.activities.messages || {};
        (function() {
            chlk.activities.messages.AddDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.ActivityGroup("MessagePopup") ], [ ria.mvc.TemplateBind(chlk.templates.messages.AddDialog) ], "AddDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), []);
        })();
    })();
    __ASSETS._mzp7xdar9tacerk9 = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push('<div class="dialog gray"><div class="x-window-header"><span>' + jade.escape(null == (jade.interp = self.getSender().getFirstName() + self.getSender().getLastName()) ? "" : jade.interp) + '</span></div><div class="x-window-body"><div class="general-info"><div>' + jade.escape(null == (jade.interp = self.subject) ? "" : jade.interp) + "</div><div>" + jade.escape(null == (jade.interp = self.body) ? "" : jade.interp) + "</div>");
        jade.globals.ActionLink_mixin.call({
            buf: buf,
            block: function() {
                buf.push("Replay");
            }
        }, "message", "sendPage", self.id);
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
        buf.push("</div></div></div>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.messages.Message";
    "chlk.models.id.MessageId";
    "chlk.models.people.Person";
    "chlk.models.common.ChlkDate";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).messages = chlk.templates.messages || {};
        (function() {
            chlk.templates.messages.ViewDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_mzp7xdar9tacerk9") ], [ ria.templates.ModelBind(chlk.models.messages.Message) ], "ViewDialog", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.id.MessageId, "id", [ ria.templates.ModelPropertyBind ], String, "subject", [ ria.templates.ModelPropertyBind ], String, "body", [ ria.templates.ModelPropertyBind ], chlk.models.people.Person, "sender", [ ria.templates.ModelPropertyBind ], chlk.models.people.Person, "recipient", [ ria.templates.ModelPropertyBind ], chlk.models.common.ChlkDate, "sent" ]);
        })();
    })();
    "chlk.activities.lib.TemplateDialog";
    "chlk.templates.messages.ViewDialog";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).messages = chlk.activities.messages || {};
        (function() {
            chlk.activities.messages.ViewDialog = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#chlk-dialogs") ], [ ria.mvc.ActivityGroup("MessagePopup") ], [ ria.mvc.TemplateBind(chlk.templates.messages.ViewDialog) ], "ViewDialog", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplateDialog), []);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.messages.Message";
    "chlk.models.messages.SendMessage";
    "chlk.models.id.MessageId";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.MessageService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("MessageService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ Number, Boolean, Boolean, String, String ] ], ria.async.Future, function getMessages(start_, read_, income_, role_, keyword_) {
                return this.getPaginatedList("PrivateMessage/List.json", chlk.models.messages.Message, {
                    start: start_,
                    count: 10,
                    read: read_,
                    income: income_ !== false,
                    role: role_ ? role_ : "",
                    keyword: keyword_
                });
            }, [ [ String, Boolean ] ], ria.async.Future, function markAs(ids, read) {
                return this.get("PrivateMessage/MarkAsRead.json", null, {
                    ids: ids,
                    read: read
                });
            }, [ [ String ] ], ria.async.Future, function del(ids) {
                return this.get("PrivateMessage/Delete.json", null, {
                    ids: ids
                });
            }, [ [ chlk.models.messages.SendMessage ] ], ria.async.Future, function send(model) {
                return this.get("PrivateMessage/Send.json", null, {
                    personId: model.getRecipientId().valueOf(),
                    subject: model.getSubject(),
                    body: model.getBody()
                });
            } ]);
        })();
    })();
    __ASSETS._xxgdyeknk3ohto6r = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        jade.globals.ActionForm_mixin.call({
            buf: buf,
            block: function() {
                buf.push('<div class="action-bar not-transparent buttons"><div class="container panel-bg"><div class="left">');
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("+new message");
                    }
                }, "message", "sendPage", []);
                buf.push('</div><div class="right">');
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("inbox");
                    },
                    attributes: {
                        "class": self.isInbox() ? "pressed" : ""
                    },
                    escaped: {
                        "class": true
                    }
                }, "message", "page", [ true, true, self.getRole(), self.getKeyword() ]);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("sent");
                    },
                    attributes: {
                        "class": self.isInbox() ? "" : "pressed"
                    },
                    escaped: {
                        "class": true
                    }
                }, "message", "page", [ true, false, self.getRole(), self.getKeyword() ]);
                buf.push("<input" + jade.attrs({
                    name: "role",
                    type: "hidden",
                    value: self.getRole()
                }, {
                    name: true,
                    type: true,
                    value: true
                }) + "/><input" + jade.attrs({
                    name: "inbox",
                    type: "hidden",
                    value: self.isInbox()
                }, {
                    name: true,
                    type: true,
                    value: true
                }) + "/><input" + jade.attrs({
                    name: "keyword",
                    type: "text",
                    value: self.getKeyword()
                }, {
                    name: true,
                    type: true,
                    value: true
                }) + "/>");
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Ok");
                    },
                    attributes: {
                        type: "submit",
                        submitType: "search"
                    },
                    escaped: {
                        type: true,
                        submitType: true
                    }
                });
                buf.push('</div></div></div><div class="action-bar buttons filter"><div class="container panel-bg"><div class="left">');
                jade.globals.Checkbox_mixin.call({
                    buf: buf
                }, "checkboxall", false);
                jade.globals.Button_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Delete");
                    },
                    attributes: {
                        type: "submit",
                        name: "submitType",
                        value: "delete",
                        id: "delete-button",
                        "class": "special-button"
                    },
                    escaped: {
                        type: true,
                        name: true,
                        value: true
                    }
                });
                if (self.isInbox()) {
                    buf.push("Mark");
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push("Read");
                        },
                        attributes: {
                            type: "submit",
                            name: "submitType",
                            value: "markAsRead",
                            id: "mark-read-button",
                            "class": "special-button"
                        },
                        escaped: {
                            type: true,
                            name: true,
                            value: true
                        }
                    });
                    jade.globals.Button_mixin.call({
                        buf: buf,
                        block: function() {
                            buf.push("Unread");
                        },
                        attributes: {
                            type: "submit",
                            name: "submitType",
                            value: "markAsUnread",
                            id: "mark-unread-button",
                            "class": "special-button"
                        },
                        escaped: {
                            type: true,
                            name: true,
                            value: true
                        }
                    });
                }
                buf.push('</div><div class="right">View');
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("All");
                    }
                }, "message", "page", [ true, self.isInbox(), "", self.getKeyword() ]);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Parent");
                    }
                }, "message", "page", [ true, self.isInbox(), "parent", self.getKeyword() ]);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Student");
                    }
                }, "message", "page", [ true, self.isInbox(), "student", self.getKeyword() ]);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Teacher");
                    }
                }, "message", "page", [ true, self.isInbox(), "teacher", self.getKeyword() ]);
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("Admin");
                    }
                }, "message", "page", [ true, self.isInbox(), "admingrade,adminedit,adminview", self.getKeyword() ]);
                buf.push('</div></div></div><input name="selectedIds" type="hidden" id="selectedIds"/>');
                jade.globals.Grid_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("<!--");
                        jade.globals.GridHead_mixin.call({
                            buf: buf,
                            block: function() {
                                buf.push('<div class="th"></div><div class="th">Read</div><div class="th">Person</div><div class="th">Subject</div><div class="th">Sent</div>');
                            }
                        });
                        buf.push("-->");
                        jade.globals.GridBody_mixin.call({
                            buf: buf,
                            block: function() {
                                (function() {
                                    var $$obj = self.getMessages().getItems();
                                    if ("number" == typeof $$obj.length) {
                                        for (var $index = 0, $$l = $$obj.length; $index < $$l; $index++) {
                                            var item = $$obj[$index];
                                            jade.globals.GridRow_mixin.call({
                                                buf: buf,
                                                block: function() {
                                                    jade.globals.ActionLink_mixin.call({
                                                        buf: buf,
                                                        block: function() {
                                                            jade.globals.Checkbox_mixin.call({
                                                                buf: buf,
                                                                attributes: {
                                                                    name: "ch"
                                                                },
                                                                escaped: {
                                                                    name: true
                                                                }
                                                            }, item.getId().valueOf(), false);
                                                            buf.push("<span>" + jade.escape(null == (jade.interp = item.isRead()) ? "" : jade.interp) + "</span>");
                                                            if (self.isInbox()) {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getSender().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getSender().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            } else {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getRecipient().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getRecipient().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            }
                                                            buf.push("<span>" + jade.escape(null == (jade.interp = item.getSubject()) ? "" : jade.interp) + "</span><span>" + jade.escape(null == (jade.interp = item.getSent().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + "</span>");
                                                        }
                                                    }, "message", "viewPage", item.getId().valueOf());
                                                    buf.push('<!--<div class="td">');
                                                    jade.globals.Checkbox_mixin.call({
                                                        buf: buf,
                                                        attributes: {
                                                            name: "ch"
                                                        },
                                                        escaped: {
                                                            name: true
                                                        }
                                                    }, item.getId().valueOf(), false);
                                                    buf.push('</div><div class="td">' + jade.escape(null == (jade.interp = item.isRead()) ? "" : jade.interp) + '</div><div class="td">');
                                                    if (self.isInbox()) {
                                                        jade.globals.ActionLink_mixin.call({
                                                            buf: buf,
                                                            block: function() {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getSender().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getSender().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            },
                                                            attributes: {
                                                                "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                                                            },
                                                            escaped: {}
                                                        }, "teachers", "info", item.getSender().getId().valueOf());
                                                    } else {
                                                        jade.globals.ActionLink_mixin.call({
                                                            buf: buf,
                                                            block: function() {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getRecipient().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getRecipient().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            },
                                                            attributes: {
                                                                "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                                                            },
                                                            escaped: {}
                                                        }, "teachers", "info", item.getRecipient().getId().valueOf());
                                                    }
                                                    buf.push('</div><div class="td">' + jade.escape(null == (jade.interp = item.getSubject()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSent().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + "</div>-->");
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
                                                    jade.globals.ActionLink_mixin.call({
                                                        buf: buf,
                                                        block: function() {
                                                            jade.globals.Checkbox_mixin.call({
                                                                buf: buf,
                                                                attributes: {
                                                                    name: "ch"
                                                                },
                                                                escaped: {
                                                                    name: true
                                                                }
                                                            }, item.getId().valueOf(), false);
                                                            buf.push("<span>" + jade.escape(null == (jade.interp = item.isRead()) ? "" : jade.interp) + "</span>");
                                                            if (self.isInbox()) {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getSender().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getSender().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            } else {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getRecipient().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getRecipient().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            }
                                                            buf.push("<span>" + jade.escape(null == (jade.interp = item.getSubject()) ? "" : jade.interp) + "</span><span>" + jade.escape(null == (jade.interp = item.getSent().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + "</span>");
                                                        }
                                                    }, "message", "viewPage", item.getId().valueOf());
                                                    buf.push('<!--<div class="td">');
                                                    jade.globals.Checkbox_mixin.call({
                                                        buf: buf,
                                                        attributes: {
                                                            name: "ch"
                                                        },
                                                        escaped: {
                                                            name: true
                                                        }
                                                    }, item.getId().valueOf(), false);
                                                    buf.push('</div><div class="td">' + jade.escape(null == (jade.interp = item.isRead()) ? "" : jade.interp) + '</div><div class="td">');
                                                    if (self.isInbox()) {
                                                        jade.globals.ActionLink_mixin.call({
                                                            buf: buf,
                                                            block: function() {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getSender().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getSender().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            },
                                                            attributes: {
                                                                "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                                                            },
                                                            escaped: {}
                                                        }, "teachers", "info", item.getSender().getId().valueOf());
                                                    } else {
                                                        jade.globals.ActionLink_mixin.call({
                                                            buf: buf,
                                                            block: function() {
                                                                buf.push("<img" + jade.attrs({
                                                                    src: self.getPictureURL(item.getRecipient().getId(), 47),
                                                                    "class": "avatar" + " " + "avatar47" + " " + item.getRecipient().getGender()
                                                                }, {
                                                                    "class": true,
                                                                    src: true
                                                                }) + "/>");
                                                            },
                                                            attributes: {
                                                                "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                                                            },
                                                            escaped: {}
                                                        }, "teachers", "info", item.getRecipient().getId().valueOf());
                                                    }
                                                    buf.push('</div><div class="td">' + jade.escape(null == (jade.interp = item.getSubject()) ? "" : jade.interp) + '</div><div class="td">' + jade.escape(null == (jade.interp = item.getSent().toString("dd/m/yy hh:min:ss")) ? "" : jade.interp) + "</div>-->");
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
                }, "message", "page", self.getMessages(), [ true ]);
            }
        }, "message", "do");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.messages.MessageList";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).messages = chlk.templates.messages || {};
        (function() {
            chlk.templates.messages.MessageList = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_xxgdyeknk3ohto6r") ], [ ria.templates.ModelBind(chlk.models.messages.MessageList) ], "MessageList", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], chlk.models.common.PaginatedList, "messages", [ ria.templates.ModelPropertyBind ], Boolean, "inbox", [ ria.templates.ModelPropertyBind ], String, "role", [ ria.templates.ModelPropertyBind ], String, "keyword" ]);
        })();
    })();
    "chlk.activities.lib.TemplatePage";
    "chlk.templates.messages.MessageList";
    (function() {
        ((chlk = chlk || {}).activities = chlk.activities || {}).messages = chlk.activities.messages || {};
        (function() {
            chlk.activities.messages.MessageListPage = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.activities.messages." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.mvc.DomAppendTo("#main") ], [ ria.mvc.TemplateBind(chlk.templates.messages.MessageList) ], "MessageListPage", ria.__SYNTAX.EXTENDS(chlk.activities.lib.TemplatePage), [ [ ria.mvc.DomEventBind("click", "#delete-button, #mark-read-button, #mark-unread-button") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function deleteMessages(node, event) {
                var s = "";
                var first = true;
                this.dom.find("[name=ch]:checked:visible").forEach(function(element) {
                    if (first) first = false; else s += ",";
                    s += element.getAttr("id");
                });
                this.dom.find("[name=selectedIds]").setValue(s);
                return true;
            }, [ ria.mvc.DomEventBind("change", "#checkboxall") ], [ [ ria.dom.Dom, ria.dom.Event ] ], ria.__SYNTAX.Modifiers.VOID, function checkAll(node, event) {
                this.dom.find("[name=ch]:visible").forEach(function(element) {
                    element.valueOf()[0].checked = node.is(":checked");
                    element.setAttr("checked", node.is(":checked"));
                });
            } ]);
        })();
    })();
    "chlk.controllers.BaseController";
    "chlk.models.messages.Message";
    "chlk.models.messages.SendMessage";
    "chlk.models.messages.MessageList";
    "chlk.activities.messages.AddDialog";
    "chlk.activities.messages.ViewDialog";
    "chlk.models.id.MessageId";
    "chlk.services.MessageService";
    "chlk.services.PersonService";
    "chlk.activities.messages.MessageListPage";
    "chlk.models.common.PaginatedList";
    (function() {
        (chlk = chlk || {}).controllers = chlk.controllers || {};
        (function() {
            chlk.controllers.MessageController = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.controllers." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("MessageController", ria.__SYNTAX.EXTENDS(chlk.controllers.BaseController), [ [ ria.mvc.Inject ], chlk.services.MessageService, "messageService", [ ria.mvc.Inject ], chlk.services.PersonService, "personService", [ chlk.controllers.SidebarButton("message") ], [ [ Boolean, Boolean, String, String, Number ] ], function pageAction(postback_, inbox_, role_, keyword_, start_) {
                inbox_ = inbox_ !== false;
                role_ = role_ || null;
                keyword_ = keyword_ || null;
                var result = this.messageService.getMessages(start_ | 0, null, inbox_, role_, keyword_).attach(this.validateResponse_()).then(function(model) {
                    this.getContext().getSession().set("currentMessages", model.getItems());
                    return this.convertModel(model, inbox_, role_, keyword_);
                }.bind(this));
                return postback_ ? this.UpdateView(chlk.activities.messages.MessageListPage, result) : this.PushView(chlk.activities.messages.MessageListPage, result);
            }, [ chlk.controllers.SidebarButton("message") ], [ [ chlk.models.messages.MessageList ] ], function doAction(model) {
                if (model.getSubmitType() == "search") return this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                if (model.getSubmitType() == "delete") this.messageService.del(model.getSelectedIds()).then(function(x) {
                    this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                }.bind(this));
                if (model.getSubmitType() == "markAsRead") this.messageService.markAs(model.getSelectedIds(), true).then(function(x) {
                    this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                }.bind(this));
                if (model.getSubmitType() == "markAsUnread") this.messageService.markAs(model.getSelectedIds(), false).then(function(x) {
                    this.pageAction(true, model.isInbox(), model.getRole(), model.getKeyword(), 0);
                }.bind(this));
            }, [ [ chlk.models.common.PaginatedList, Boolean, String, String ] ], function convertModel(list_, inbox_, role_, keyword_) {
                var result = new chlk.models.messages.MessageList();
                result.setMessages(list_);
                result.setInbox(inbox_);
                result.setRole(role_);
                result.setKeyword(keyword_);
                return new ria.async.DeferredData(result);
            }, [ [ String ] ], function sendPageAction(replayOnId_) {
                var message;
                if (replayOnId_) {
                    message = this.getMessageFromSession(replayOnId_);
                } else message = new chlk.models.messages.Message();
                model = ria.async.DeferredData(message);
                return this.ShadeView(chlk.activities.messages.AddDialog, model);
            }, [ [ chlk.models.messages.SendMessage ] ], function sendAction(model) {
                this.messageService.send(model).then(function(x) {
                    this.view.getCurrent().close();
                    this.pageAction(true);
                }.bind(this));
            }, [ [ String ] ], function viewPageAction(id) {
                var message = this.getMessageFromSession(id);
                var model = ria.async.DeferredData(message);
                return this.ShadeView(chlk.activities.messages.ViewDialog, model);
            }, function getMessageFromSession(id) {
                return this.getContext().getSession().get("currentMessages", []).filter(function(message) {
                    return message.getId().valueOf() == id;
                })[0];
            } ]);
        })();
    })();
    "chlk.models.common.ChlkDate";
    (function() {
        ((chlk = chlk || {}).models = chlk.models || {}).search = chlk.models.search || {};
        (function() {
            "use strict";
            chlk.models.search.SearchItem = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.models.search." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SearchItem", [ String, "id", String, "description", String, "gender", [ ria.serialize.SerializeProperty("searchtype") ], Number, "searchType", [ ria.serialize.SerializeProperty("roleid") ], Number, "roleId" ]);
        })();
    })();
    "chlk.services.BaseService";
    "ria.async.Future";
    "chlk.models.search.SearchItem";
    (function() {
        (chlk = chlk || {}).services = chlk.services || {};
        (function() {
            "use strict";
            chlk.services.SearchService = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.services." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("SearchService", ria.__SYNTAX.EXTENDS(chlk.services.BaseService), [ [ [ String ] ], ria.async.Future, function search(query_) {
                return this.get("Search/Search.json", ria.__API.ArrayOf(chlk.models.search.SearchItem), {
                    query: query_
                });
            } ]);
        })();
    })();
    __ASSETS._1wpomd4zzspaxlxr = function anonymous(locals) {
        var buf = [];
        var self = locals || {};
        buf.push("<li" + jade.attrs({
            "data-title": self.description,
            "data-value": self.id.valueOf() + "|" + self.searchType.valueOf(),
            style: "background: white"
        }, {
            "data-title": true,
            "data-value": true,
            style: true
        }) + "><div>");
        if (self.searchType == 0) {
            if (self.roleId == 2) {
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("<img" + jade.attrs({
                            src: self.getPictureURL(self.id, 47),
                            "class": "avatar" + " " + "avatar47" + " " + self.gender
                        }, {
                            "class": true,
                            src: true
                        }) + "/><div>" + jade.escape(null == (jade.interp = self.description) ? "" : jade.interp) + "</div>");
                    },
                    attributes: {
                        "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                    },
                    escaped: {}
                }, "teachers", "info", self.id);
            }
            if (self.roleId == 3) {
                jade.globals.ActionLink_mixin.call({
                    buf: buf,
                    block: function() {
                        buf.push("<img" + jade.attrs({
                            src: self.getPictureURL(self.id, 47),
                            "class": "avatar" + " " + "avatar47" + " " + self.gender
                        }, {
                            "class": true,
                            src: true
                        }) + "/><div>" + jade.escape(null == (jade.interp = self.description) ? "" : jade.interp) + "</div>");
                    },
                    attributes: {
                        "class": "left" + " " + "image-container" + " " + "white" + " " + "shadow"
                    },
                    escaped: {}
                }, "students", "info", self.id);
            }
        } else {
            buf.push("<div>" + jade.escape(null == (jade.interp = self.description) ? "" : jade.interp) + "</div>");
        }
        buf.push("</div></li>");
        return buf.join("");
    };
    "chlk.templates.JadeTemplate";
    "chlk.models.search.SearchItem";
    (function() {
        ((chlk = chlk || {}).templates = chlk.templates || {}).search = chlk.templates.search || {};
        (function() {
            chlk.templates.search.SiteSearch = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk.templates.search." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }([ ria.templates.TemplateBind("_1wpomd4zzspaxlxr") ], [ ria.templates.ModelBind(chlk.models.search.SearchItem) ], "SiteSearch", ria.__SYNTAX.EXTENDS(chlk.templates.JadeTemplate), [ [ ria.templates.ModelPropertyBind ], String, "id", [ ria.templates.ModelPropertyBind ], String, "description", [ ria.templates.ModelPropertyBind ], Number, "searchType", [ ria.templates.ModelPropertyBind ], Number, "roleId", [ ria.templates.ModelPropertyBind ], String, "gender" ]);
        })();
    })();
    "chlk.BaseApp";
    "chlk.controllers.AnnouncementController";
    "chlk.controllers.FeedController";
    "chlk.controllers.AccountController";
    "chlk.controllers.CalendarController";
    "chlk.controllers.SettingsController";
    "chlk.controllers.SetupController";
    "chlk.controllers.StudentsController";
    "chlk.controllers.TeachersController";
    "chlk.controllers.MessageController";
    "chlk.services.SearchService";
    "chlk.templates.search.SiteSearch";
    (function() {
        chlk = chlk || {};
        (function() {
            chlk.TeacherApp = function ClassCompilerImpl() {
                var def = ria.__SYNTAX.parseClassDef(new ria.__SYNTAX.Tokenizer([].slice.call(arguments)));
                ria.__SYNTAX.validateClassDecl(def, ria.__API.Class);
                var name = "chlk." + def.name;
                return ria.__SYNTAX.compileClass(name, def);
            }("TeacherApp", ria.__SYNTAX.EXTENDS(chlk.BaseApp), [ ria.__SYNTAX.Modifiers.OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();
                dispatcher.setDefaultControllerId("feed");
                dispatcher.setDefaultControllerAction("list");
                return dispatcher;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set("role", new chlk.models.common.Role(chlk.models.common.RoleEnum.TEACHER, "Teacher"));
                return session;
            }, ria.__SYNTAX.Modifiers.OVERRIDE, ria.async.Future, function onStart_() {
                return BASE().then(function(data) {
                    new ria.dom.Dom().fromHTML(ASSET("_ihgenbsa8l6ry66r")()).appendTo("#sidebar");
                    return data;
                });
            } ]);
        })();
    })();
    (function() {
        new chlk.TeacherApp().session(ria.__CFG["#mvc"].settings || {}).run();
    })();
})(jQuery, jade);