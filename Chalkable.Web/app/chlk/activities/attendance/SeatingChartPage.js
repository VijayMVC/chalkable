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
            new ria.dom.Dom('#submit-chart').show();
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
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        'SeatingChartPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, 'savedChart')],
            VOID, function updateSeatingChart(tpl, model, msg_) {

            },

            chlk.models.attendance.SeatingChart, 'model',

            Array, function getAttendances_(){
                var res = [];
                var attendancesNodes = new ria.dom.Dom('.attendance-data');
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

            function closePopUp(){
                this.dom.find('.seating-chart-popup')
                    .hide()
                    .removeClass('absent')
                    .removeClass('late')
                    .setCss('top', 'auto');
                this.dom.find('.active-student').removeClass('active-student')
            },

            [ria.mvc.DomEventBind('click', '.attendance-data')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentClick(node, event){
                if(!this.dom.hasClass('dragging-on') && this.dom.find('.page-content').hasClass('can-post')){
                    var popUp = this.dom.find('.seating-chart-popup');
                    var main = this.dom.parent('#main');
                    var bottom = main.height() + main.offset().top - node.offset().top + 71;
                    var left = node.offset().left - main.offset().left - 54;
                    popUp.setCss('bottom', bottom);
                    popUp.setCss('left', left);
                    var reasonId = node.getData('reason-id') || -1;
                    var reasonText = node.getData('reason-text') || '';
                    var type = node.getData('type');
                    popUp.find('.selected').removeClass('selected');
                    popUp.find('.reason-text').setHTML('');
                    popUp.find('.first-part .item[data-type=' + type + ']')
                        .addClass('selected')
                        .find('.reason-text')
                        .setHTML(' - ' + reasonText);
                    popUp.find('.type-part[data-type=' + type + ']')
                        .find('.item[data-id=' + reasonId + ']')
                        .addClass('selected');
                    setTimeout(function(){
                        node.addClass('active-student');
                        popUp.show();
                        this.checkPopUp();
                    }.bind(this), 1);
                }
            },

            function checkPopUp(){
                var popUp = this.dom.find('.seating-chart-popup');
                if(popUp.offset().top < 0){
                    popUp.setCss('bottom', 'auto');
                    popUp.setCss('top', 0);
                }
            },

            [ria.mvc.DomEventBind('click', '.first-part .item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function popUpClick(node, event){
                var typeId = node.getData('type');
                var popUp = node.parent('.seating-chart-popup');
                switch(typeId){
                    case chlk.models.attendance.AttendanceTypeEnum.ABSENT.valueOf():
                        popUp.addClass('absent'); this.checkPopUp(); break;
                    case chlk.models.attendance.AttendanceTypeEnum.LATE.valueOf():
                        popUp.addClass('late'); this.checkPopUp(); break;
                    default:
                        this.setTypeByNode(node);
                }
            },

            function setTypeByNode(node){
                var reasonId = node.getData('id');
                var typeId = node.getData('type');
                var reasonText = node.find('.text').getHTML();
                var studentData = this.dom.find('.active-student');
                studentData.setData('type', typeId);
                studentData.setData('reason-id', reasonId);
                studentData.setData('reason-text', reasonText);
                var parent = studentData.parent('.not-empty');
                parent.removeClass('absent')
                    .removeClass('present')
                    .removeClass('late');
                switch(typeId){
                    case chlk.models.attendance.AttendanceTypeEnum.ABSENT.valueOf():
                        parent.addClass('absent'); break;
                    case chlk.models.attendance.AttendanceTypeEnum.LATE.valueOf():
                        parent.addClass('late'); break;
                    default:
                        parent.addClass('present');
                }
                parent.find('.reason-text').setHTML(reasonText);
                this.closePopUp();
            },

            [ria.mvc.DomEventBind('click', '.reason-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function reasonClick(node, event){
                this.setTypeByNode(node);
            },

            [ria.mvc.DomEventBind('click', '.remove-student')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function removeStudentClick(node, event){
                new ria.dom.Dom('#submit-chart').show();
                var parent = node.parent('.student-block ');
                var clone = parent.clone();
                parent.addClass('empty-box');
                setEmptyBoxHtml(parent);
                var container = new ria.dom.Dom('.seating-chart-people')
                    .find('.people-container');
                clone.insertBefore(container.find('#submit-chart'))
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
                var seatingList = [], minRows = 0, minColumns = 0;
                this.dom.find('.seating-row').forEach(function(items){
                    var seatings = [];
                    items.find('.student-block[data-index]').forEach(function(node){
                        var node2 = node.find('.image-container'),
                            row = node.getData('row'),
                            column = node.getData('column');
                        seatings.push({
                            index: node.getData('index'),
                            row: row,
                            column: column,
                            studentId: node2.exists() ? node2.getData('id') : null
                        });
                        if(node2.exists()){
                            if(row > minRows)
                                minRows = row;
                            if(column > minColumns)
                                minColumns = column;
                        }
                    });
                    seatingList.push(seatings);
                }.bind(this));

                this.dom.find('#min-rows').setValue(minRows);
                this.dom.find('#min-columns').setValue(minColumns);

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
                node.setHTML('SAVING....');
            },

            Boolean, 'ableRePost',

            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, 'saved')],
            VOID, function updateGradingPart(tpl, model, msg_) {
                if(this.isAbleRePost()){
                    this.dom.find('#submit-attendance-button').setHTML('POST IT');
                }else{
                    this.dom.find('.save-attendances-buttons').remove();
                }
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
                    if($( ".droppable" )[0])
                        $( ".droppable" ).droppable( "destroy" );
                    if($( ".draggable" )[0])
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
                this.setAbleRePost(model.isAbleRePost());

                var tpl = new chlk.templates.attendance.SeatingChartPeopleTpl();
                tpl.assign(model);
                tpl.renderTo(new ria.dom.Dom('body'));

                new ria.dom.Dom().on('click.seating', '.close-people, #submit-chart', function(node, event){
                    this.stopDragging();
                }.bind(this));

                new ria.dom.Dom().on('click.seating', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.seating-chart-popup') && this.dom.find('.seating-chart-popup:visible').exists())
                        this.closePopUp();
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