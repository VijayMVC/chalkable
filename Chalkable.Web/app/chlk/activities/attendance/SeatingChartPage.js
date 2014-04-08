REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.SeatingChartTpl');
REQUIRE('chlk.templates.attendance.SeatingChartPeopleTpl');
REQUIRE('chlk.templates.attendance.ClassAttendanceWithSeatPlaceTpl');

NAMESPACE('chlk.activities.attendance', function () {
    "use strict";

    var draggableOptions = {
        revert: true,
        revertDuration: 0,
        cancel: '.empty-box'
    };

    var activeDragging = false;

    var droppableOptions = {
        activeClass: "active",
        hoverClass: "hover",

        drop: function(event, ui) {
            var droppable = $(this);
            var draggable = ui.draggable;
            if(droppable.find('.empty')[0]){
                droppable.html(draggable.html()).removeClass('empty-box');
                if(!droppable.next('.absolute')[0])
                    jQuery('<div class="empty-box student-block absolute"></div>').insertAfter(droppable);
                if(draggable.parents('.seating-chart-people')[0]){
                    draggable.css('opacity', 0);
                    draggable.animate({
                        width: 0
                    }, 250, function(){
                        draggable.remove();
                        checkPadding();
                    });
                }else{
                    setEmptyBoxHtml(new ria.dom.Dom(draggable));
                }
            }else{
                var html = droppable.html();
                droppable.html(draggable.html());
                draggable.html(html);
            }
        }
    };

    function setEmptyBoxHtml(node){
        var model = new chlk.models.attendance.ClassAttendance();
        var tpl = new chlk.templates.attendance.ClassAttendanceWithSeatPlaceTpl();
        tpl.assign(model);
        node.setHTML(tpl.render()).addClass('empty-box');
        var next = node.next('.absolute');
        next.exists() && next.remove();
    }

    function checkPadding(){
        var body = jQuery('body');
        var padding = parseInt(body.css('padding-bottom'), 10);
        var height = body.find('.seating-chart-people').height();
        if(!padding || padding != height){
            body.css('padding-bottom', height);
        }
    }

    /** @class chlk.activities.attendance.SeatingChartPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.SeatingChartTpl)],
        'SeatingChartPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            chlk.models.attendance.SeatingChart, 'model',

            Array, function getAttendances_(){
                var res = [];
                var attendancesNodes = new ria.dom.Dom('.attendance-data');
                var len = attendancesNodes.length, i, node;
                attendancesNodes.forEach(function(node){
                    res.push({
                        personId: node.getData('id'),
                        type: node.getData('type'),
                        attendanceReasonId: node.getData('reason-id')
                    });
                });
                return res;
            },

            [ria.mvc.DomEventBind('click', '.add-remove-students')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addRemoveStudentsClick(node, event){
                if(!new ria.dom.Dom('.seating-chart-people:visible').exists()){
                    var chart = new ria.dom.Dom('.seating-chart-people');
                    chart.show();
                    this.dom.addClass('dragging-on');
                    $(".draggable").draggable(draggableOptions);
                    $(".droppable").droppable(droppableOptions);
                    activeDragging = true;
                    setTimeout(function(){
                        var body = jQuery('body');
                        var padding = parseInt(body.css('padding-bottom'), 10);
                        var height = body.find('.seating-chart-people').height();
                        if(!padding || padding < height){
                            if(padding)
                                body.data('padding', padding);
                            body.css('padding-bottom', height);
                        }
                    }, 1);
                }
            },

            [ria.mvc.DomEventBind('click', '.remove-student')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function removeStudentClick(node, event){
                var parent = node.parent();
                var clone = parent.clone();
                parent.addClass('empty-box');
                setEmptyBoxHtml(parent);
                var container = new ria.dom.Dom('.seating-chart-people')
                    .find('.people-container');
                clone.appendTo(container)
                    .removeClass('droppable')
                    .removeClass('ui-droppable');
                jQuery(clone.valueOf()).draggable(draggableOptions);
                checkPadding();
            },

            [ria.mvc.DomEventBind('click', '#all-present-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allPresentClick(node, event){
                new ria.dom.Dom('.student-block').forEach(function(node){
                    var container = node.find('.not-empty');
                    if(container.exists()){
                        container
                            .removeClass('absent')
                            .removeClass('late')
                            .addClass('present')
                    }
                });
            },

            function recalculateChartInfo(){
                var rows = this.dom.find('.seating-row').count();
                var columns = this.dom.find('.seating-row:eq(0)').find('.student-block[data-index]').count();
                var classId = this.dom.find('.class-id').getValue();
                var seatingList = [];
                this.dom.find('.seating-row').forEach(function(items){
                    var seatings = [];
                    items.find('.student-block[data-index]').forEach(function(node){
                        var node2 = node.find('.image-container');
                        seatings.push({
                            index: node.getData('index'),
                            row: node.getData('row'),
                            column: node.getData('column'),
                            studentId: node2.exists() ? node2.getData('id') : null
                        })
                    });
                    seatingList.push(seatings);
                }.bind(this));

                var res = {
                    rows: rows,
                    columns: columns,
                    classId: classId,
                    seatingList: seatingList
                };
                this.dom.find('.text-for-post').setValue(JSON.stringify(res));
            },

            [ria.mvc.DomEventBind('click', '#submit-attendance-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function postButtonClick(node, event){
                this.dom.find('.attendances-json').setValue(JSON.stringify(this.getAttendances_()));
                this.dom.find('.save-attendances-form').trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.update-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function updateGridClick(node, event){
                this.recalculateChartInfo();
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_){
                BASE(model, msg_);
                this.stopDragging();
                this.setModel(model);
            },

            function stopDragging(remove_){
                var chart = new ria.dom.Dom('.seating-chart-people');
                if(remove_)
                    chart.remove();
                else
                    chart.hide();
                this.dom.removeClass('dragging-on');
                if(activeDragging){
                    $( ".droppable" ).droppable( "destroy" );
                    $( ".draggable" ).draggable( "destroy" );
                    activeDragging = false;
                }
                setTimeout(function(){
                    var body = jQuery('body');
                    var padding = body.data('padding');
                    body.css('padding-bottom', padding || 'inherit');
                }, 1);
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setModel(model);

                var tpl = new chlk.templates.attendance.SeatingChartPeopleTpl();
                tpl.assign(model);
                tpl.renderTo(new ria.dom.Dom('body'));

                new ria.dom.Dom().on('click.seating', '.close-people, #submit-chart', function(node, event){
                    this.stopDragging();
                }.bind(this));

                new ria.dom.Dom().on('click.seating', '#submit-chart', function(){
                    this.recalculateChartInfo();
                    this.dom.find('.save-chart-form').trigger('submit');
                }.bind(this));

                jQuery(window).on('resize.seating', function(){
                    if(new ria.dom.Dom('.seating-chart-people').exists())
                        checkPadding();
                });

            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                this.stopDragging(true);
                new ria.dom.Dom().off('click.seating');
                jQuery(window).off('resize.seating');
            }
        ]);
});