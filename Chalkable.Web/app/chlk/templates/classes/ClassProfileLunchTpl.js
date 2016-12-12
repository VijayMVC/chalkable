REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes', function () {
    "use strict";

    /** @class chlk.templates.classes.LunchCountItemType*/
    ENUM('LunchCountItemType', {
        STAFF: 0,
        GUEST: 1,
        STUDENT: 2
    });

    /** @class chlk.templates.classes.ClassProfileLunchTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileLunch.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileLunchTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [
            function getTableInfo(){
                var lunchInfo = this.getClazz().getLunchCountInfo(),
                    mealItems = lunchInfo.getMealItems(),
                    topTable = [], bottomTable = [];

                (lunchInfo.getStaffs() || []).forEach(function(item){
                    var rowItem = {}, mealsArr = [];
                    rowItem.info = item;
                    rowItem.type = chlk.templates.classes.LunchCountItemType.STAFF;
                    mealItems.forEach(function(mealItem){
                        var mealCountItem = mealItem.getMealCountItems().filter(function(mealCountItem){
                            return mealCountItem.getPersonId() == item.getId()
                        })[0];
                        mealsArr.push(mealCountItem || new chlk.models.lunchCount.MealCountItem(item.getId(), false, 0));
                    });
                    rowItem.mealItems = mealsArr;
                    topTable.push(rowItem);
                });

                if(lunchInfo.isIncludeGuest()){
                    var rowItem = {}, mealsArr = [];
                    rowItem.info = {name : "Guest"};
                    rowItem.type = chlk.templates.classes.LunchCountItemType.GUEST;
                    mealItems.forEach(function(mealItem){
                        var mealCountItem = mealItem.getMealCountItems().filter(function(mealCountItem){
                            return mealCountItem.isGuest()
                        })[0];
                        mealsArr.push(mealCountItem || new chlk.models.lunchCount.MealCountItem(null, true, 0));
                    });
                    rowItem.mealItems = mealsArr;
                    topTable.push(rowItem);
                }

                (lunchInfo.getStudents() || []).forEach(function(item){
                    var rowItem = {}, mealsArr = [];
                    rowItem.info = item;
                    rowItem.type = chlk.templates.classes.LunchCountItemType.STUDENT;
                    mealItems.forEach(function(mealItem){
                        var mealCountItem = mealItem.getMealCountItems().filter(function(mealCountItem){
                            return mealCountItem.getPersonId() == item.getId()
                        })[0];
                        mealsArr.push(mealCountItem || new chlk.models.lunchCount.MealCountItem(item.getId(), false, 0));
                    });
                    rowItem.mealItems = mealsArr;
                    bottomTable.push(rowItem);
                });

                return {
                    mealItems : mealItems || [],
                    topTable : topTable,
                    bottomTable : bottomTable
                }
            }
        ])
});