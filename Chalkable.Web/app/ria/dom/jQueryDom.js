/**
 * Usage:
 * window.$$ = ria.dom.Dom;
 * $$('A')
 *      .on('click', function(node, event) {
 *          return true;
 *      })
 *      .off('click', handler)
 *      .on('click', 'span', function (node, event) {
 *
 *      })
 *      .off('click', 'span', handler)
 */

REQUIRE('ria.dom.Dom');

NAMESPACE('ria.dom', function () {
    "use strict";

    if ('undefined' === typeof jQuery)
        throw Error('jQuery is not defined.');

    var global = ('undefined' !== typeof window ? window.document : null);

    ria.dom.Event = Object; // jQuery modifies event


    /** @class ria.dom.jQueryDom */
    CLASS(
        'jQueryDom', EXTENDS(ria.dom.Dom), [
            function $(dom_) {
                BASE();
                VALIDATE_ARG('dom_', [Node, String, ArrayOf(Node), SELF, jQuery], dom_);

                this._dom = jQuery(global);

                if ('string' === typeof dom_) {
                    this._dom = jQuery(dom_);
                } else if (Array.isArray(dom_)) {
                    this._dom = jQuery(dom_);
                } else if (dom_ instanceof Node) {
                    this._dom = jQuery(dom_);
                } else if (dom_ instanceof NodeList) {
                    this._dom = jQuery(dom_);
                } else if (dom_ instanceof SELF) {
                    this._dom = jQuery(dom_.valueOf());
                } else if (dom_ instanceof jQuery) {
                    this._dom = dom_;
                }
            },

            OVERRIDE, Boolean, function exists() {
                return !!this._dom.valueOf()[0];
            },

            String, function getSelector() {
                return this._dom.selector;
            },

            /* DatePicker */
            [[Object, Date]],
            SELF, function datepicker(options, value_){
                this._dom.datepicker(options);
                value_ && this._dom.datepicker('setDate', value_);
                return this;
            },

            /* Search tree */

            [[String]],
            OVERRIDE, SELF, function find(selector) {
                return new ria.dom.Dom(jQuery(selector, this._dom));
            },

            /* Events */

            // TODO: need a good way of implementing off()

            /*OVERRIDE, SELF, function on(event, selector, handler_) {
                var old_dom = this._dom;
                this._dom = this.valueOf();
                BASE(event, selector, handler_);
                this._dom = old_dom;
                return this;
            }, */

            [[Number]],
            SELF, function slideDown(time_){
                time_ ? this._dom.slideDown(time_) : this.slideDown.show();
                return this;
            },

            [[Number]],
            SELF, function slideUp(time_){
                time_ ? this._dom.slideUp(time_) : this._dom.slideUp();
                return this;
            },

            [[Number]],
            SELF, function show(time_){
                time_ ? this._dom.show(time_) : this._dom.show();
                return this;
            },

            [[Number]],
            SELF, function hide(time_){
                time_ ? this._dom.hide(time_) : this._dom.hide();
                return this;
            },

            [[Function]],
            SELF, function fadeIn(callback_){
                callback_ ? this._dom.fadeIn(callback_) : this._dom.fadeIn();
                return this;
            },

            [[Function]],
            SELF, function fadeOut(callback_){
                callback_ ? this._dom.fadeOut(callback_) : this._dom.fadeOut();
                return this;
            },

            SELF, function clone(){
                return new SELF(this._dom.clone());
            },

            [[String]],
            SELF, function wrap(html){
                this._dom.wrap(html);
                return this;
            },

            [[String]],
            Boolean, function isOrInside(selector){
                return this.is(selector) || this.parent(selector).exists();
            },

            String, function text(){
                return this._dom.text();
            },

            OVERRIDE, SELF, function on(event, selector, handler_) {
                VALIDATE_ARGS(['event', 'selector', 'handler_'], [String, [String, ria.dom.DomEventHandler], ria.dom.DomEventHandler], arguments);
                if(!handler_){
                    handler_ = selector;
                    selector = undefined;
                }

                this._dom.on(event, selector, handler_.__wrapper__ || (handler_.__wrapper__ = function () {
                    var args = [].slice.call(arguments);
                    args.unshift(new ria.dom.Dom(this));
                    return handler_.apply(this, args);
                }));
                return this;
            },

            OVERRIDE, SELF, function off(event, selector_, handler_) {
                VALIDATE_ARGS(['event', 'selector_', 'handler_'], [String, [String, ria.dom.DomEventHandler], ria.dom.DomEventHandler], arguments);
                this._dom.off(event, selector_, handler_ && handler_.__wrapper__);
                return this;
            },

            /*OVERRIDE, SELF, function off(event, selector, handler_) {
                var old_dom = this._dom;
                this._dom = this.valueOf();
                BASE(event, selector, handler_);
                this._dom = old_dom;
                return this;
            },*/

            /* append/prepend */

            OVERRIDE, SELF, function appendTo(dom) {
                VALIDATE_ARG('dom', [SELF, String, Node], dom);

                if(typeof dom == "string")
                    dom = new SELF(dom);

                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                if(dest){
                    VALIDATE_ARG('dom', [Node], dest);

                    this._dom.appendTo(dest);
                }
                return this;
            },

            SELF, function insertAfter(dom) {
                VALIDATE_ARG('dom', [SELF, String, Node], dom);

                if(typeof dom == "string")
                    dom = new SELF(dom);

                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                if(dest){
                    VALIDATE_ARG('dom', [Node], dest);

                    this._dom.insertAfter(dest);
                }
                return this;
            },

            OVERRIDE, SELF, function insertBefore(dom) {
                VALIDATE_ARG('dom', [SELF, String, Node], dom);

                if(typeof dom == "string")
                    dom = new SELF(dom);

                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                if(dest){
                    VALIDATE_ARG('dom', [Node], dest);

                    this._dom.insertBefore(dest);
                }
                return this;
            },

            SELF, function appendChild(dom) {
                VALIDATE_ARG('dom', [SELF, String, Node], dom);

                if(typeof dom == "string")
                    dom = new SELF(dom);

                var el = dom instanceof Node ? dom : dom.valueOf().shift();
                if(el){
                    VALIDATE_ARG('dom', [Node], el);

                    this._dom.append(el);
                }
                return this;
            },

            OVERRIDE, SELF, function prependTo(dom) {
                VALIDATE_ARG('dom', [SELF, String, Node], dom);

                if(typeof dom == "string")
                    dom = new SELF(dom);

                var dest = dom instanceof Node ? dom : dom.valueOf().shift();
                if(dest){
                    VALIDATE_ARG('dest', [Node], dest);

                    this._dom.prependTo(dest);
                }
                return this;
            },

            /* parseHTML - make static */

            [[String]],
            OVERRIDE, SELF, function fromHTML(html) {
                this._dom = jQuery(jQuery.parseHTML(html));
                return this;
            },

            [[String]],
            SELF, function setHTML(html) {
                this._dom.html(html);
                return this;
            },

            function getHTML() {
                return this._dom.html();
            },

            /* DOM manipulations & navigation */

            OVERRIDE, SELF, function empty() {
                this._dom.empty();
                return this;
            },

            [[ria.dom.Dom]],
            OVERRIDE, SELF, function remove(node_) {
                node_ ? node_.remove() : this._dom.remove();
                return this;
            },

            // reference https://github.com/julienw/dollardom

            [[String]],
            OVERRIDE, SELF, function descendants(selector__) {},
            [[String]],
            OVERRIDE, SELF, function parent(selector_) {
                return selector_ ? new ria.dom.Dom(this._dom.parents(selector_)) : new ria.dom.Dom(this._dom.parent());
            },
            [[String]],
            OVERRIDE, SELF, function next(selector_) {
                return new ria.dom.Dom(this._dom.next(selector_));
            },
            [[String]],
            OVERRIDE, SELF, function previous(selector_) {
                return new ria.dom.Dom(this._dom.prev(selector_));
            },
            [[String]],
            OVERRIDE, SELF, function first(selector_) {},
            [[String]],
            OVERRIDE, SELF, function last(selector_) {},
            [[String]],
            OVERRIDE, Boolean, function is(selector) {
                return this._dom.is(selector);
            },
            [[Object]],
            OVERRIDE, Boolean, function contains(node) {
                VALIDATE_ARG('node', [Node, SELF, ArrayOf(Node)], node);

                var nodes = [];
                if (node instanceof Node) {
                    nodes = [node];
                } else if (Array.isArray(node)) {
                    nodes = node;
                } else if (node instanceof SELF) {
                    nodes = node.valueOf();
                }

                var res = true;
                nodes.forEach(function(node){
                    if(res && !jQuery.contains(this._dom[0], node))
                        res =  false;
                }.bind(this));
                return res;
            },

            /* attributes */

            OVERRIDE, Object, function getAllAttrs() {},
            [[String]],
            OVERRIDE, Object, function getAttr(name) {
                return this._dom[0] && this._dom[0].getAttribute ? this._dom[0].getAttribute(name) || null : null;
            },
            OVERRIDE, Object, function getValue() {
                return this._dom.val() || null;
            },
            [[Object]],
            OVERRIDE, SELF, function setAllAttrs(obj) {},
            [[String, Object]],
            OVERRIDE, SELF, function setAttr(name, value) {
                this._dom[0] && this._dom.each(function(){
                    if(this.setAttribute ){
                        value ? this.setAttribute(name, value) : this.removeAttribute(name);
                    }
                });
                return this;
            },


            [[String]],
            OVERRIDE, SELF, function removeAttr(name) {
                this._dom.removeAttr(name);
                return this;
            },


            [[Object]],
            OVERRIDE, SELF, function setValue(value) {
                this._dom.val(value);
                if(this.getAttr('type') == 'checkbox'){
                    var node = this.parent().find('.hidden-checkbox');
                    node.setValue(value);
                    node.setData('value', value);
                }
                if(this._dom.is('select')){
                    this._dom.trigger('change');
                }
                return this;
            },

            [[Object]],
            OVERRIDE, Number, function height(value_) {
                return value_ ? this._dom.height(value_) : this._dom.height();
            },
            [[Object]],
            OVERRIDE, Number, function width(value_) {
                return value_ ? this._dom.width(value_) : this._dom.width();
            },

            /* data attributes */

            OVERRIDE, Object, function getAllData() {},
            [[String]],
            OVERRIDE, Object, function getData(name) {
                return this._dom.data(name) === undefined ? null : this._dom.data(name);
            },
            [[Object]],
            OVERRIDE, SELF, function setAllData(obj) {},
            [[String, Object]],
            OVERRIDE, SELF, function setData(name, value) {
                this.setAttr('data-' + name, value);
                this._dom.data(name, value);
                return this;
            },

            /* classes */

            [[String]],
            OVERRIDE, Boolean, function hasClass(clazz) {
                return this._dom.hasClass(clazz);
            },
            [[String]],
            OVERRIDE, SELF, function addClass(clazz) {
                return this.toggleClass(clazz, true);
            },
            [[String]],
            OVERRIDE, SELF, function removeClass(clazz) {
               return this.toggleClass(clazz, false);
            },

            [[String, Boolean]],
            OVERRIDE, SELF, function toggleClass(clazz, toggleOn_) {
                this._dom.toggleClass(clazz, toggleOn_);
                return this;
            },

            /* css */

            [[String]],
            OVERRIDE, function getCss(property) {
                return this._dom.css(property);
            },
            [[String, Object]],
            OVERRIDE, SELF, function setCss(property, value) {
                this._dom.css(property, value);
                return this;
            },
            [[Object]],
            OVERRIDE, SELF, function updateCss(props) {},

            /* iterator */

            [[ria.dom.DomIterator]],
            OVERRIDE, SELF, function forEach(iterator) {
                this._dom.each(function () {
                    iterator(SELF(this));
                });
                return this;
            },

            [[ria.dom.DomIterator]],
            OVERRIDE, SELF, function filter(iterator) {
                this._dom = this._dom.filter(function () {
                    return iterator(SELF(this));
                });
                return this;
            },

            OVERRIDE, Number, function count() {
                return this._dom.length;
            },

            /* raw nodes */

            OVERRIDE, ArrayOf(Node), function valueOf() {
                return ria.__API.clone(this._dom);
            },

            [[String, Object]],
            SELF, function trigger(event, params_) {
                this._dom.trigger(event, params_);
                return this;
            },

            Boolean, function checked() {
                return !!(this.parent().find('.hidden-checkbox').getData('value')) || false;
            },

            Number, function index() {
                return this._dom.index();
            },

            [[Number]],
            OVERRIDE, function scrollTop(top_) {
                return top_ || top_ == 0 ? this._dom.scrollTop(top_) : this._dom.scrollTop();
            },

            /* Form */

            Object, function serialize(noArray_){
                var o = {};
                var array = this._dom.serializeArray();
                array.forEach(function(item) {
                    if (o[item.name] !== undefined && !noArray_) {
                        if (!o[item.name].push) {
                            o[item.name] = [o[item.name]];
                        }
                        o[item.name].push(item.value || '');
                    } else {
                        o[item.name] = item.value || '';
                    }
                });
                return o;
            }
        ]);

    ria.dom.Dom.SET_IMPL(ria.dom.jQueryDom);
});