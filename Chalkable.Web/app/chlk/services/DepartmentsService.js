REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.departments.Department');
REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.DepartmentsService*/
    CLASS(
        'DepartmentsService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getDepartments(pageIndex_) {
                return this.getPaginatedList('ChalkableDepartment/List.json', chlk.models.departments.Department, {
                    start: pageIndex_|0
                });
            },

            [[String, String, FileList]],
            ria.async.Future, function addDepartment(name, keywords, files) {
                return this.uploadFiles('ChalkableDepartment/create.json', files, chlk.models.departments.Department, {
                    name: name,
                    keywords: keywords
                });
            },

            [[chlk.models.id.DepartmentId, String, String, FileList]],
            ria.async.Future, function updateDepartment(id, name, keywords, files) {

                return this.uploadFiles('ChalkableDepartment/update.json', files, chlk.models.departments.Department, {
                    chalkableDepartmentId: id.valueOf(),
                    name: name,
                    keywords: keywords
                });
            },

            [[chlk.models.id.DepartmentId, String, String, FileList]],
            ria.async.Future, function saveDepartment(id_, name, keywords, files) {
                if (id_ && id_.valueOf()) return this.updateDepartment(id_, name, keywords, files);
                return this.addDepartment(name, keywords, files);
            },

            [[chlk.models.id.DepartmentId]],
            ria.async.Future, function removeDepartment(id) {
                return this.post('ChalkableDepartment/Delete.json', chlk.models.departments.Department, {
                    chalkableDepartmentId: id.valueOf()
                });
            },
            [[chlk.models.id.DepartmentId]],
            ria.async.Future, function getDepartment(id) {
                return this.post('ChalkableDepartment/Info.json', chlk.models.departments.Department, {
                    chalkableDepartmentId: id.valueOf()
                });
            }
        ])
});