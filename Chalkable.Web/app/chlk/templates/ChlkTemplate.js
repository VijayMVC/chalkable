REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.people.User');
REQUIRE('lib/autolinker.min.js');

NAMESPACE('chlk.templates', function () {
    "use strict";

    /** @class chlk.templates.ChlkTemplate */
    CLASS(
        'ChlkTemplate', EXTENDS(chlk.templates.JadeTemplate), [
            [[Object, Number]],
            String, function getStudentPictureURL(id, sizeW_, sizeH_, notDepartmentSpecific_){
                if(!id)
                    return null;
                var url = this.isDemoSchool() ? window.demoAzurePictureUrl : window.azurePictureUrl;

                var districtId = this.isDemoSchool() ? window.DEMO_SCHOOL_PICTURE_DISTRICT : window.school.districtid;

                if (notDepartmentSpecific_ == null)
                    url += districtId + '_';
                url += id.valueOf();

                return this.formatPictureURL_(url, sizeW_, sizeH_);
            },

            [[String, Number, Number]],
            String, function formatPictureURL_(url, sizeW_, sizeH_)
            {
                if (sizeW_ && sizeH_)
                    return url + '-' + sizeW_ + 'x' + sizeH_;
                if (sizeW_)
                    return url + '-' + sizeW_ + 'x' + sizeW_;
                return url;
            },

            [[Object, Number]],
            String, function getPictureURL(id, sizeW_, sizeH_, notDepartmentSpecific_){
                if(!id)
                    return null;
                var url = window.azurePictureUrl + id.valueOf();
                return this.formatPictureURL_(url, sizeW_, sizeH_);
            },

            [[Object, Number]],
            String, function getAppPictureURL(id, sizeW_, sizeH_){
               return this.getPictureURL(id, sizeW_, sizeH_);
            },

            Boolean, function isDemoSchool(){
                return !!window.DEMO_SCHOOL || false;
            },

            [[Number]],
            String, function getSerial(number){
                return window.getSerial(number);
            },

            [[String]],
            String, function getRoleController(role){
                var controller = role.toLowerCase() + 's';
                if (controller.indexOf('admin') > -1)
                    controller = 'admins';
                return controller;
            },

            chlk.models.common.Role, 'userRole',

            chlk.models.people.User, 'currentUser',

            Date, function getServerDate(str_, a_, b_){
                return chlk.models.common.ChlkDate.GET_SERVER_DATE(str_, a_, b_);
            },

            Date, function getSchoolYearServerDate(str_, a_, b_){
                return chlk.models.common.ChlkSchoolYearDate.GET_SCHOOL_YEAR_SERVER_DATE(str_, a_, b_);
            },

            [[String]],
            String, function linkify(text) {
                return Autolinker.link(String(text || ''), {
                    "newWindow": true,
                    truncate: 80,
                    className: 'extenal'
                });
            }
        ])
});