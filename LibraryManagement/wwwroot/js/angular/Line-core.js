var ProgressCount = 1;
$(document).ajaxStart(function () {
    ShowLoading();
});

//document.addEventListener("mousemove", function () {
//    var x = getRandomArbitrary(1, 1.3);
//    var x2 = x.toFixed(2) + "px";
//    //alert(x2);
//    debugger
//    jQuery("#CVModel .toolbar").each(function () {
//        debugger
//        $(".toolbar").css("padding", x2);
//    })
    
//});
function getRandomArbitrary(min, max) {
    return Math.random() * (max - min) + min;
}
(function () {
    var _templatesUrl = '/js/angular/templates/';
    var _apiPostUrl = area;
    var _apiGetUrl = area;
    var modelToFromData = function (obj, formData, preindex, attachOnly) {
        var index;
        for (var prop in obj) {
            index = preindex ? preindex + "[" + prop + "]" : prop;
            if (obj[prop] instanceof File) {
                formData.append(prop + "[]", obj[prop]);
            }
            else if (angular.isObject(obj[prop]))
                modelToFromData(obj[prop], formData, index);
            else if (!attachOnly) {
                formData.append(index, obj[prop]);
            }
        }
        return formData;
    };

    var app = angular.module('ngLine', ['pascalprecht.translate', 'ngSanitize']);

    app.run(function ($rootScope, $timeout) {
        $rootScope.messages = [];
        $rootScope.notifications = [];
        $rootScope.showbox = {};
        $rootScope.setMessages = function (messages) {
            angular.forEach(messages, function (message) {
                if (message.message_type === 2) {
                    $rootScope.notifications.push(message);
                    $timeout(function () {
                        $rootScope.notifications.splice($rootScope.notifications.indexOf(message), 1);
                    }, 5000);
                } else if (message.message_type === 1) {
                    $rootScope.messages.push(message);
                }

            });
        };
    });

    app.config(['$locationProvider', function ($locationProvider) {
        $locationProvider.html5Mode({ enabled: true, requireBase: false, rewriteLinks: false });
    }]);

    app.config(['$translateProvider', function ($translateProvider) {
        $translateProvider.useSanitizeValueStrategy('escape', 'sanitizeParameters');
        $translateProvider.translations('all', angular.translations);
        $translateProvider.preferredLanguage('all');
    }]);

    app.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.transformResponse.push(function (responseData) {
            var regexIso8601 = /^(\d\d\d\d)-(\d?\d)-(\d?\d)T(\d\d):(\d\d):(\d\d)$/g;
            function getLocaleDate(input) {
                if (typeof input !== "object") return input;
                for (var key in input) {
                    if (!input.hasOwnProperty(key)) continue;
                    var value = input[key];
                    var match;
                    if (typeof value === "string" && (match = value.match(regexIso8601))) {
                        input[key] = moment(match[0], moment.ISO_8601).format('L');
                    } else if (typeof value === "object") {
                        getLocaleDate(value);
                    }
                }
            }
            getLocaleDate(responseData);
            return responseData;
        });
        $httpProvider.interceptors.push(function ($q, $rootScope) {
            return {
                'request': function (config) {
                    if (config.url.indexOf("GenericGets") >= 0)
                        RemoveLoading();
                    else// (!config.url.split('GenericGets').length > 1)
                        ShowLoading();//$('#loading-wrapper').fadeIn('slow');
                    if (config.data && config.data.token) {
                        config.headers.__RequestVerificationToken = config.data.token;
                        delete config.data.token;
                    } else if (config.data && config.data.__RequestVerificationToken) {
                        config.headers.__RequestVerificationToken = config.data.__RequestVerificationToken;
                    } else if ($rootScope.token) {
                        config.headers.__RequestVerificationToken = $rootScope.token;
                    }
                    config.headers.XRequestedWith = "ajax";
                    return config;
                },
                'response': function (response) {
                    var result = response.data != undefined ? response.data.dataController == undefined ? response.data : response.data.dataController : undefined;

                    if (result.success && result.Msg.length == 1) {
                        //$rootScope.setMessages([{ message_type: 2, message_header: "Notification", message_body: response.data.Msg[0] }]);
                        //showNotification('success', '', result.Msg[0]);
                        md.showNotification('top', 'center', 'success', result.Msg[0])
                    }
                    if (result.success === false && result.Msg.length == 1) {
                        //$rootScope.setMessages([{ message_type: 2, message_header: "Notification", message_body: response.data.Msg[0] }]);
                        //showNotification('error', '', result.Msg[0]);
                        md.showNotification('top', 'center', 'danger', result.Msg[0])
                    }

                    if (result.success === false && angular.isArray(result.Msg) && result.Msg.length > 1) {
                        $rootScope.setMessages([{ message_type: 1, message_header: "Business Validation Messages", message_body: result.Msg }]);
                    }

                    if (result.url && result.url != "") {
                        GoToUrl(result.url);
                        return;
                    }
                    RemoveLoading();
                    return response;
                },
                'responseError': function (rejection) {
                    RemoveLoading()
                    //console.error("request error",rejection);
                    //$rootScope.setMessages([{ message_type: 1, message_header: "Request Error: " + rejection.status + " " + rejection.statusText, message_body: [rejection.config.url] }]);
                    //showNotification('error', resources.SystemErrorTittle, resources.SystemErrorMsg)
                    md.showNotification('top', 'center', 'danger', resources.SystemErrorMsg)
                    return $q.resolve({ data: { success: false }, rejection: rejection });
                }
            };
        });
    }]);

    app.controller('null', function () {/*dummy ctrl*/ });

    var sgFormDirective = function ($http, $timeout, $parse) {
        var link = function (scope, elem, attrs, ctrl, transclude) {
            elem.addClass('sg-form');
            scope.moment = moment;
            transclude(scope, function (clone) {
                elem.append(clone);
            });
            scope.form = ctrl;
            scope.token = attrs.token;
            var IsEdit =
			scope.vm = {};
            scope.vm.ctrl = attrs.ctrl;
            if (attrs.view != "False") {
                scope.vm.getData = scope.getData || function (dataToModel) {
                    scope.vm.editLock = true;
                    if (typeof scope.beforeGet === "function") scope.beforeGet();
                    if (dataToModel != undefined) {
                        switch (dataToModel.Name) {
                            case "Project":
                                scope.id = dataToModel.ProjectId;
                                break;
                            case "Task":
                                scope.id = dataToModel.TaskId;
                                break;
                            case "CV":
                                scope.id = dataToModel.id;
                                break;
                          
                        }
                    }
                    var _url = "";
                    if (typeof attrs.urlAction != "undefined") {
                        try {
                            _url = JSON.parse(attrs.urlAction).getdata;
                        } catch (ex) {
                            _url = attrs.urlAction.getdata
                        }

                    }
                    else {
                        _url = _apiGetUrl + attrs.ctrl + "/CreateOrEditModel"
                    }
                    ShowLoading(); 
                    $http.post(_url, scope.id).then(function (response) { //, token: attrs.token
                        if (response.data.success == true) {
                            scope.model = response.data.data;
                            switch (attrs.ctrl) {
                                case "TheatersTemplate":
                                    buildTheater(response.data.data);
                                    break;
                                case "Theaters":
                                    buildTheater(response.data.data);
                                    break;
                                case "RaphaelTemplate":
                                    buildTheaterRaphael(response.data.data);
                                    break;
                                case "Raphael":
                                    buildTheaterRaphael(response.data.data);
                                    break;

                                case "Show": {
                                    if (response.data.Id != 0)
                                        buildTheater(response.data.data.TheaterData);
                                    break;
                                }
                            }
                            if (dataToModel != undefined) {
                                switch (dataToModel.Name) {
                                    case "Project":
                                        scope.model.StrategicGoalId = dataToModel.StrategicGoalId;
                                        scope.model.ManagementId = dataToModel.ManagementId;
                                        break;
                                    case "Task":
                                        scope.model.ProjectId = dataToModel.ProjectdId;
                                        break;
                                    case "CV":
                                        scope.model.id = dataToModel.id;
                                        break;
                                }
                                RemoveLoading();
                            }
                            if (typeof scope.afterGet === "function") scope.afterGet(response);
                            $timeout(function () {
                                ctrl.$setPristine();
                                ctrl.$setUntouched();
                                scope.vm.editLock = false;
                                scope.FormErrors = {};
                                RemoveLoading();
                            }, 10);
                        }
                    });
                };

                scope.vm.validateForm = function () {
                    scope.FormErrors = {};
                    ctrl.$setSubmitted();
                    if (ctrl.$invalid) {
                        var elemClass = elem.find(".sg-input-container.ng-invalid [name]:first");
                        var errorsList = [];
                        $(elemClass).closest('sg-tabs').find('ul.nav-tabs:first>li').removeClass("active");
                        $(elemClass).closest('sg-tabs').find('ul.nav-tabs:first>li[tab-toggle=' + $(elemClass).closest('.tab-pane').attr("id") + ']').addClass("active");
                        $(elemClass).closest('sg-tabs').find('.tab-content:first>.tab-pane').css("display", "none");
                        $(elemClass).closest('.tab-pane').css("display", "block");

                        if (elemClass.is(":visible")) {
                            try {
                                elem.find(elemClass)[0].focus();
                                elem.find('.sg-input-container.ng-invalid:visible')[0].scrollIntoView({ behavior: "smooth", viewPadding: { y: 10 } });
                                //$('.sg-input-container.ng-invalid:visible')[0].scrollIntoView({duration: 2500, direction: "vertical",viewPadding: { y: 10 }});
                                //scope.$root.messages.push({ 'message_type': 'error', message_header: 'Form Error', 'message_body': ["لم يتم حفظ البيانات بسبب وجود أخطاء , قم بالتأكد من البيانات المدخلة والحفظ مجددا"] });
                            } catch (e) { }
                        } else {

                            angular.forEach(ctrl.$error, function (errors, errorType) {
                                angular.forEach(errors, function (error) {
                                    if (error.messages) {
                                        errorsList.push(error.messages[errorType]);
                                    }
                                });
                            });
                            if (errorsList.length == 0) return true;
                            scope.$root.messages.push({ 'message_type': 'error', message_header: 'Form Error', 'message_body': errorsList });
                        }
                        return false;
                    }
                    return true;
                };
                scope.$on('RunAdhocValidate', function () {
                    scope.vm.validateForm();
                });
                scope.vm.showModalForm = function (id) {
                    var tempScope = angular.element(document.getElementById(id)).scope();
                    tempScope.buildModel(id);
                }
                scope.vm.submit = function (continueEditing, saveAsDraft, tstatus) {
                    if (!saveAsDraft)
                        if (!scope.vm.validateForm())
                            return;
                    scope.vm.editLock = true;
                    //Loading Ajax Busy
                    ShowLoading();//$('#loading-wrapper').fadeIn('slow');
                    var postData = angular.copy(scope.model);
                    if (typeof scope.afterGet === "function") postData = scope.beforeSubmit(scope.model);
                    json = { model: postData, continueEditing: continueEditing, token: attrs.token, status: tstatus != undefined ? tstatus : "" };
                    if (saveAsDraft) json['saveAsDraft'] = true;
                  
                    var _url = "";
                    if (typeof attrs.urlAction != "undefined") {
                        try {
                            _url = JSON.parse(attrs.urlAction).submitdata;
                        } catch (ex) {
                            _url = attrs.urlAction.submitdata
                        }

                    }
                    else {
                        _url = _apiPostUrl + attrs.ctrl + "/CreateOrEdit"
                    }
                    ShowLoading();
                    $http.post(_url, json).then(function (response) {
                        if (response.data.success) {
                            if (scope.model._upload) {
                                var formData = new FormData();
                                formData = modelToFromData(scope.model._upload, formData, null, true);
                                formData.append("Id", response.data.data.Id);
                                formData.append("__RequestVerificationToken", $('input[name=__RequestVerificationToken]:first').val());
                                $http.post(_apiPostUrl + attrs.ctrl + "/uploadFiles", formData, {
                                    transformRequest: angular.identity,
                                    headers: { 'Content-Type': undefined, 'Process-Data': false }
                                }).then(function () {
                                    scope._upload = [{}];
                                });
                            }
                            scope.model = response.data
                            //Remove Loading Ajax Busy
                            RemoveLoading();
                            if (attrs.popup != undefined) {
                                try {
                                    $('#' + attrs.popup).modal('toggle');
                                } catch (ex) {
                                }
                                try {
                                    var tempScope = angular.element(document.getElementById('tableProject_' + postData.ManagementId + "_" + postData.StrategicGoalId)).scope();
                                    try {
                                        tempScope.$broadcast('Import');
                                    }
                                    catch (ex) {
                                    }
                                    try {
                                        tempScope = angular.element(document.getElementById('Task_' + postData.ProjectId)).scope();
                                        tempScope.$broadcast('Import');
                                    }
                                    catch (ex) {
                                    }
                                    try {
                                        tempScope = angular.element(document.getElementById('VacancyPostTable')).scope();
                                        tempScope.$broadcast('Import');
                                    }
                                    catch (ex) {
                                    }
                                    
                                } catch (ex) {
                                }
                                

                                return;
                            }
                            $timeout(function () {
                                ctrl.$setPristine();
                            }, 10);

                        } else if (response.data.success === false && response.data.FormErrors) {
                            ctrl.$setUntouched();
                            ctrl.$setPristine();
                            scope.FormErrors = response.data.FormErrors;
                        }
                        scope.vm.editLock = false;
                    });
                };
                scope.$watch('id', function (val) {
                    if (val >= 0)
                        scope.vm.getData();
                })
            }

            scope.sum = function (items, prop1, prop2) {
                if (items)
                    return items.reduce(function (a, b) {
                        if (b[prop1]) a = a + b[prop1];
                        if (b[prop2]) a = a + b[prop2];
                        return a;
                    }, 0);
            };
        };
        return {
            restrict: 'EA',
            name: 'formController',
            require: '^form',
            template: '',
            scope: { model: '=?formModel', id: '=' },
            transclude: true,
            link: link,
            controller: '@'
        };
    };

    var sgTableDirective = function ($filter) {
        var templateUrl = _templatesUrl + 'sg-table.html';
        var ctrlName;
        var controller = function ($scope, $http, $element) {
            var vm = this;
            $scope.currentRow = {};
            $scope.col_search = {};
            $scope.col_search_operator = {};
            $scope.cso_color = {};
            $scope.tableParams = {};
            vm.binding = [];
            vm.delete_msg = {
                header: "header",
                bodyMsg: "bodyMsg",
            };
            vm.transaction = { meta: [], data: [] };
            vm.cols = [];
            vm.childcols = [];
            vm.sg_cols = [];
            vm.sg_childcols = [];
            vm.fields = [];
            vm.showEditForm = false;
            vm.defualtValues = { Id: 0 };
            vm.limit_list = [{ 'title': 10, 'val': 10 }, { 'title': 25, 'val': 25 }, { 'title': 50, 'val': 50 }, { title: 100, val: 100 }, { 'title': 'all', val: 10000000 }];
            vm.filter_operators = [
				{ 'title': 'mdi-code-not-equal', 'color': 'text-danger', 'val': 'nq', filter: 'text,number', tooltip: $filter('translate')('seagull.operators.NQ') },
				{ 'title': 'mdi-code-equal', 'color': 'text-success', 'val': 'eq', filter: 'text,number', tooltip: $filter('translate')('seagull.operators.EQ') },
				{ 'title': 'mdi-code-array', 'color': 'text-primary', 'val': 'in', filter: 'text', tooltip: $filter('translate')('seagull.operators.IN') }

            ];
            vm.Date_filter_operators = [
				{ 'title': 'mdi-code-greater-than-or-equal', 'color': 'text-danger', 'val': 'gr', filter: 'date', tooltip: $filter('translate')('seagull.operators.greater') },
                { 'title': 'mdi-code-equal', 'color': 'text-success', 'val': 'eq', filter: 'date', tooltip: $filter('translate')('seagull.operators.EQ') },
				{ 'title': 'mdi-code-less-than-or-equal', 'color': 'text-primary', 'val': 'ls', filter: 'date', tooltip: $filter('translate')('seagull.operators.less') }

            ];
            $scope.page_no = 1;
            vm.limit = $scope.limit;



            $scope.$watch(function () {
                return [$scope.where, $scope.col_search, vm.limit, $scope.initialData];
            }, function (newVal, oldVal) {
                if (newVal != oldVal) {
                    $scope.page_no = 1;
                    vm.undo();
                }
            }, true);
            $scope.rowClass = function (row) {
                //debugger
                return "";//"red";
            };
            $scope.rowClassChild = function (row) {
                //debugger
                return "bg-CustomGreen !important";
            };
            vm.pick_filter = function (col_name, operator) {
                $scope.col_search_operator[col_name] = operator.val;
                $scope.cso_color[col_name] = operator.color;
                if ($scope.col_search[col_name] != null && $scope.col_search[col_name] != "") {
                    $scope.page_no = 1;
                    vm.undo();
                }
            };

            vm.selectRow = function (row) {
                $scope.currentRow = row;
            };

            vm.editRow = function (row) {
                if ($scope.viewAction) {
                    var url = $scope.viewAction.replace("0", row.Id);
                    window.location = url;
                    return;
                }
                if ($scope.createAction) {
                    //var url = $scope.createAction.replace("seagull=qHH63IV%2AyBM%3D", row.EncId);
                    var url = $scope.createAction.replace("0", row.Id);
                    window.location = url;
                    return;
                }
            };
            
            vm.addRow = function () {
                if ($scope.createAction) {
                    window.location = $scope.createAction;
                    return;
                }
            };
            vm.addCustomRow = function (functionName) {
                BuildCustomAdd($scope, $http, _apiPostUrl, functionName);
            };

            vm.deleteRow = function (row) {
                //Loading Ajax Busy
                ShowLoading();//$('#loading-wrapper').fadeIn('slow');
                var jsondelete = { Id: row.Id, token: $scope.token };
                $http.post(_apiPostUrl + ctrlName + '/Delete', jsondelete ).then(function (response) {
                    if (response.data.success === true) {
                        row.$operation = 'delete';
                    } else {
                        //Remove Loading Ajax Busy
                        RemoveLoading();
                    }
                });
            };

            vm.CustomViewObject = function (row, tablename) {
                BuildView($scope, $http, _apiPostUrl, row, tablename);
            };

            vm.sort = function (col) {
                if (col.name) {
                    vm.reverse = col.name === vm.orderby ? !vm.reverse : false;
                    vm.orderby = col.name;
                    vm.undo();
                }
            };

            vm.paging = function (action) {
                switch (action) {
                    case 'next':
                        if ($scope.page_no < vm.page_count) {
                            $scope.page_no = $scope.page_no + 1;
                        }
                        break;
                    case 'previous':
                        if ($scope.page_no > 1) {
                            $scope.page_no = $scope.page_no - 1;
                        }
                        break;
                    case 'first':
                        $scope.page_no = 1;
                        break;
                    case 'last':
                        $scope.page_no = vm.page_count;
                        break;
                }
                vm.undo();
            }
            vm.ShowChild = function (child, Id) {
                if (child.show == undefined)
                    child.show = true;
                else
                    child.show = !child.show;
            }

            //vm.toggleAll = function () {
            //    angular.forEach(vm.binding, function (item) {
            //        item.showHistory = !item.showHistory;
            //    });
            //}

            vm.undo = function (isexport, tabletype) {
                var additionalSearchParameter = "";
                var stringJson = "";
                var jsonArray = [];
                if (tabletype != undefined) {
                    debugger
                    switch (tabletype) {
                        case "OrderAdditionalSearch":
                            additionalSearchParameter = {};

                            if ($scope.$parent.searchName != undefined && $scope.$parent.searchName != "") {
                                additionalSearchParameter["Name"] = $scope.$parent.searchName;
                                additionalSearchParameter["CompareName"] = $scope.$parent.compareName;
                                jsonArray.push(additionalSearchParameter);
                            }

                            if ($scope.$parent.searchMobileNumber != undefined && $scope.$parent.searchMobileNumber != "") {
                                additionalSearchParameter = {};
                                additionalSearchParameter["MobileNumber"] = $scope.$parent.searchMobileNumber;
                                additionalSearchParameter["CompareMobileNumber"] = $scope.$parent.compareMobileNumber;
                                jsonArray.push(additionalSearchParameter);
                            }

                            if ($scope.$parent.searchDateFrom != undefined && $scope.$parent.searchDateFrom != "") {
                                additionalSearchParameter = {};
                                additionalSearchParameter["DateFrom"] = $scope.$parent.searchDateFrom;
                                additionalSearchParameter["CompareMobileNumber"] = $scope.$parent.compareMobileNumber;
                                jsonArray.push(additionalSearchParameter);
                            }
                            if ($scope.$parent.searchDateTo != undefined && $scope.$parent.searchDateTo != "") {
                                additionalSearchParameter = {};
                                additionalSearchParameter["DateTo"] = $scope.$parent.searchDateTo;
                                additionalSearchParameter["CompareMobileNumber"] = $scope.$parent.compareMobileNumber;
                                jsonArray.push(additionalSearchParameter);
                            }

                            stringJson = JSON.stringify(jsonArray)
                            break;
                        default:
                            break;
                    }
                }


                vm.showEditForm = false;
                vm.isLoading = true;
                var Id = $scope.initialData;
                var URl = _apiGetUrl + $scope.datasrc;
                var paging = {
                    //"token": $scope.token,
                    "pagination": {
                        "start": ($scope.page_no - 1) * vm.limit,
                        "Count": vm.limit
                    },
                    "sort": {
                        "predicate": vm.orderby,
                        "reverse": vm.reverse
                    },
                    "search": JSON.stringify($scope.col_search),
                    "search_operator": JSON.stringify($scope.col_search_operator),
                    "where": JSON.stringify($scope.where),
                    "id": Id,
                    "Export": isexport != undefined ? true : false,
                    "AdditionalParameter": additionalSearchParameter == "" ? null : stringJson
                };
                if ($scope.additionalpaging != undefined && $scope.additionalpaging != "") {
                    switch ($scope.additionalpaging.ListType) {
                        case "ListByPlanIdAndStrategicGoalIdAndManagementId":
                            paging.PlanId = $scope.additionalpaging.PlanId;
                            paging.StrategicGoalId = $scope.additionalpaging.StrategicGoalId;
                            paging.ManagementId = $scope.additionalpaging.ManagementId;
                            break;
                        case "ListSuitable" :
                        case "ListSecondSuitable" :
                        case "ListNotSuitable" :
                        case "ListSecondNotSuitable" :
                            paging.ServiceId = $scope.additionalpaging.ServiceId;
                            paging.id = $scope.additionalpaging.ServiceId;
                            break;
                    }
                }
                if (isexport != undefined) {
                    //vm.showEditForm = true;
                    vm.isLoading = false;
                    window.open(URl.replace("List", "Export"), '_blank');
                    return;
                }
                $http.post(URl, paging).then(function (data) {
                    if (typeof $scope.onDataFetch != "undefined") $scope.onDataFetch()(data);
                    angular.forEach(data.data.data, function (item) {
                        item.show = false;
                    });
                    vm.binding = data.data;

                    vm.page_count = data.data.page_count;
                    vm.data_count = data.data.data_count;
                    vm.transaction = { data: [] };

                    if ($scope.page_no > vm.page_count && vm.page_count > 0) {
                        $scope.page_no = angular.copy(vm.page_count);
                    }
                    //if ($scope.where) {
                    //    for (var key in $scope.where) {
                    //        vm.defualtValues[key] = $scope.where[key];
                    //    }
                    //}
                    //$scope.currentRow = vm.binding.data[0]; proplem when refresh then add 
                    vm.isLoading = false;
                    setTimeout(function () {
                        $element.find('.sg-table table.table').floatThead({
                            position: 'absolute',
                            top: 0
                        });
                    }, 100);
                });
            };

            $scope.$on('Import', function () {
                vm.undo();
            });

        }, link = function (scope, elem, attrs, ctrl, transclude) {
            if (typeof attrs.deleteMsg != "undefined") { scope.vm.delete_msg = scope.deleteMsg; } else { scope.deleteMsg = scope.vm.delete_msg; }
            if (typeof attrs.hasFilter == "undefined") scope.hasFilter = true;
            if (typeof attrs.ctrl != "undefined") ctrlName = attrs.ctrl;
            if (typeof attrs.hasOperation == "undefined") { scope.hasOperation = { 'add': true, 'edit': true, 'delete': false, customadd: { 'show': false, 'function': "" }, export: { 'excell': false, 'word': false },customview: { 'show': false, 'tablename': "" } }; } else { attrs.hasOperation = scope.hasOperation }
            if (typeof attrs.hasImport == "undefined") scope.hasImport = false;
            if (typeof attrs.limit == "undefined") ctrl.limit = 10;
            if (typeof attrs.canView == "undefined") scope.hasImport = false;
            if (typeof attrs.initialData == "undefined") scope.initialData = 0;
            if (typeof attrs.additionalpaging == "undefined") scope.additionalpaging = "";
            ctrl.undo();
        };
        return {
            restrict: 'E',
            transclude: true,
            scope: {
                datasrc: '@',
                token: '@',
                createAction: '@',
                viewAction: '@',
                limit: '=?',
                hasFilter: '=?',
                hasOperation: '=?',
                additionalpaging: '=',
                //additionalpaging: '=?',
                deleteMsg: '=?',
                hasImport: '=?',
                currentRow: '=?',
                initialData: '=?',
                where: '=?',
                onDataFetch: '&?'
            },
            controller: controller,
            controllerAs: 'vm',
            link: link,
            templateUrl: templateUrl
        };
    };

    var sgColDirective = function () {
        var link = function (scope, elem, attrs, sgTableController, transclude) {
            transclude(scope, function (content) {
                if (content[0]) {
                    var html = "";
                    angular.forEach(content, function (node) {
                        if (node.outerHTML) html = html + node.outerHTML;
                        else if (node.textContent) html = html + node.textContent;
                    });
                    attrs.tag = "<span>" + html + "</span>";
                }
            });
            sgTableController.cols.push(attrs);
            sgTableController.orderby = sgTableController.cols[0].name;
            if (attrs.name && attrs.type) {
                sgTableController.pick_filter(attrs.name, attrs.type == 'text' ? sgTableController.filter_operators[2] : attrs.type == 'date' ? sgTableController.Date_filter_operators[2] : sgTableController.filter_operators[1]);
            }
            //console.log("attrs", attrs);
        };
        return {
            restrict: 'E',
            transclude: true,
            require: '^sgTable',
            link: link
        };
    };

    var sgColChildDirective = function () {
        var link = function (scope, elem, attrs, sgTableController, transclude) {
            transclude(scope, function (content) {
                if (content[0]) {
                    var html = "";
                    angular.forEach(content, function (node) {
                        if (node.outerHTML) html = html + node.outerHTML;
                        else if (node.textContent) html = html + node.textContent;
                    });
                    attrs.tag = "<span>" + html + "</span>";
                }
            });
            //sgTableController.childcols = [];
            sgTableController.childcols.push(attrs);
            sgTableController.orderby = sgTableController.childcols[0].name;
            if (attrs.name && attrs.type) {
                sgTableController.pick_filter(attrs.name, attrs.type == 'text' ? sgTableController.filter_operators[2] : attrs.type == 'date' ? sgTableController.Date_filter_operators[2] : sgTableController.filter_operators[1]);
            }
            //console.log("attrs", attrs);
        };
        return {
            restrict: 'E',
            transclude: true,
            require: '^sgTable',
            link: link
        };
    };

    var sgOutputDirective = function ($compile) {
        var link = function (scope, element, attrs, sgTableCtrl) {
            var filter = "";
            if (scope.meta.tag) {
                var el = angular.element(scope.meta.tag);
                $compile(el)(scope.$parent);
                element.replaceWith(el);
                return;
            }

            if (scope.meta.type && scope.meta.type == "checkbox") {
                if (scope.value === true || scope.value == "true" || scope.value == 1)
                    element.html('<div class="text-center"><i class="fa fa-check text-primary"></i></div>');
                else
                    element.html('<div class="text-center"><i class="fa fa-times text-warning"></i></div>');
                return;
            }
            if (scope.meta.type && scope.meta.type == "progress") {
                if (scope.value == undefined)
                    scope.value = 0;
                var tempClass = scope.value == 0 ? 'whiteProgress' : '';
                element.html('<div id="progressBar_' + ProgressCount + '_' + scope.value + '" class="jquery-ui-like ' + tempClass + '"><div></div></div>');
                var id = 'progressBar_' + ProgressCount + '_' + scope.value;
                progressBar(scope.value, $('#' + id));
                ProgressCount = ProgressCount + 1;
                return;
            }
            if (scope.meta.type && scope.meta.type == "Image") {
                if (scope.value == undefined)
                    scope.value = 0;
                var tempClass = scope.value == 0 ? 'whiteProgress' : '';
                element.html('<img src="' + scope.value + '" alt="" height="100" width="200"> ');
                return;
            }
            element.html('<span ng-bind="::row[col.name] ' + filter + '"/>');

            scope.output = scope.value;
            $compile(element.contents())(scope.$parent);

            element.replaceWith(element.contents());

        };
        return {
            restrict: 'E',
            //template : '<span ng-bind="::value">',
            require: '^sgTable',
            scope: { value: '=', meta: '=' },
            link: link
        };
    };

    var sgOutputChildDirective = function ($compile) {
        var link = function (scope, element, attrs, sgTableCtrl) {
            var filter = "";
            if (scope.meta.tag) {
                var el = angular.element(scope.meta.tag);
                $compile(el)(scope.$parent);
                element.replaceWith(el);
                return;
            }

            if (scope.meta.type && scope.meta.type == "checkbox") {
                if (scope.value === true || scope.value == "true" || scope.value == 1)
                    element.html('<div class="text-center"><i class="fa fa-check text-primary"></i></div>');
                else
                    element.html('<div class="text-center"><i class="fa fa-times text-warning"></i></div>');
                return;
            }
            element.html('<span ng-bind="::child[ccol.name] ' + filter + '"/>');

            scope.output = scope.value;
            $compile(element.contents())(scope.$parent);

            element.replaceWith(element.contents());

        };
        return {
            restrict: 'E',
            //template : '<span ng-bind="::value">',
            require: '^sgTable',
            scope: { value: '=', meta: '=' },
            link: link
        };
    };

    var sgInputDirective = function ($parse, $filter, $translate, $sanitize) {
        link = function (scope, element, attrs, ctrl) {
            element.addClass('sg-input-container');
            scope.required = attrs.ngRequired;

            if (!attrs.label) {
                element.find('.control-label').remove();
                element.find('.form-control-wrapper').css({ width: "100%" });
            }
            //element.find('input').bind('change', function () {
            //    ctrl.$setDirty();
            //    scope.$apply(ctrl);
            //});

            element.find('[name].form-control').bind('blur', function () {
                ctrl.$setTouched();
                scope.$apply(ctrl);
            });
            var reqMessage = attrs.label || attrs.hiddenLabel || null;
            if (reqMessage)
                reqMessage = '"' + reqMessage + '"';

            $translate('seagull.validation.REQUIRED', { param: reqMessage }).then(function (d) {
                ctrl.messages.required = d;
            });
            $translate('seagull.validation.MINLENGTH', { param: attrs.ngMinlength }).then(function (d) {
                ctrl.messages.minlength = d;
            });
            $translate('seagull.validation.MAXLENGTH', { param: attrs.ngMaxlength }).then(function (d) {
                ctrl.messages.maxlength = d;
            });
          
            ctrl.messages = {
                required: "",
                minlength: "",
                maxlength: "",
                min: $filter('translate')('seagull.validation.MIN') + attrs.ngMin,
                max: $filter('translate')('seagull.validation.MAX') + attrs.ngMin,
                email: $filter('translate')('seagull.validation.EMAIL'),
                date: $filter('translate')('seagull.validation.DATE'),
            };

            function validateEmail(email) {
                var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(email);
            }

            scope.getMessage = function () {
                for (var i in ctrl.messages) {
                    if (ctrl.$error[i]) return ctrl.messages[i];
                }
            };


            var validator = function (value) {
               // if (attrs.type == 'date') ctrl.$setValidity('date', value == "" || value == null || moment(value, "DD/MM/YYYY", true).isValid());// && moment(value).format("DD/MM/YYYY") == value);
                if (attrs.ngMin) ctrl.$setValidity('min', value >= attrs.ngMin || value == "" || value == null);
                if (attrs.ngMax) ctrl.$setValidity('max', value <= attrs.ngMax || value == "" || value == null);
                if (attrs.ngMaxlength) ctrl.$setValidity('maxlength', value == "" || value == null || value.toString().length <= attrs.ngMaxlength);
                if (attrs.ngMinlength) ctrl.$setValidity('minlength', value == "" || value == null || value.toString().length >= attrs.ngMinlength);
                if (typeof attrs.ngEmail != "undefined") ctrl.$setValidity('email', value == "" || value == null || validateEmail(value));
                return value;
            };
            //ctrl.$formatters.push(validator);
            //ctrl.$parsers.push(validator);
            scope.$watch('ngModel', function (newVal, oldVal) {
                if (newVal != oldVal) {
                    validator(newVal);
                    //if (attrs.type == "percentage")
                    //    scope.ngModel = newVal.replace(/[\%,]/, '') + "%";
                    ctrl.$setDirty(); //if we set name for both directive and input $dirty can be read from input
                }
            });
        };
        return {
            restrict: 'E',
            require: 'ngModel',
            transclude: true,
            templateUrl: _templatesUrl + 'sg-input.html',
            scope: {
                ngModel: '=', datactrl: '@' ,kk:'@',change: '@', name: '@', type: '@', label: '@', disable: '@', keyup: '@', value: '@', checked: '@', class: '@', id: '@', placeholder: '@', step: '@'
            },
            link: link

        };
    };

    var sgTypeDirective = function ($compile) {
        
        return {
            link: function (scope, elem, attrs) {
                var html;
                switch (attrs.sgType) {

                    case 'checkbox':
                        html = '<input value="' + attrs.value + '" type="checkbox" name="' + attrs.name + '"  ng-model="ngModel" ng-change="' + attrs.change + '" ng-disabled="' + attrs.disable + '" ng-attr-id="' + attrs.id + '" ng-click="' + attrs.checked + '" />';
                        break;
                    case 'radio':
                        html = '<input value="' + attrs.value + '" type="radio" name="' + attrs.name + '" ng-model="ngModel"  ng-change="' + attrs.change + '" ng-disabled="' + attrs.disable + + '" ng-value="' + attrs.value + '" ng-checked="' + attrs.checked + '" />';
                        break;
                    case 'textarea':
                        html = '<div><textarea  value="' + attrs.value + '" style="' + attrs.style + '" name="' + attrs.name + '" class="form-control ' + attrs.class + '" ng-model="ngModel" ng-change="' + attrs.change + '" ng-disabled="' + attrs.disable + '"  tooltipview><textarea/></div>';
                        break;
                    case 'number':
                        html = '<input type="number" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-disabled="' + attrs.disable + '" ng-change="' + attrs.change + '" ng-keyup="' + attrs.keyup + '"  />';
                        break;
                    case 'float':
                        html = '<input type="text" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-disabled="' + attrs.disable + '" pattern="[0-9]+([\.,][0-9]+)*" format>';
                        break;
                    case 'date':
                        html = '<input type="text" ng-value="' + attrs.value + '" name="' + attrs.name + '" k-datepicker class="form-control k-datepicker" ng-change="' + attrs.change + '" ng-keyup="' + attrs.keyup + '" ng-model="ngModel" ng-disabled="' + attrs.disable + '" />';
                        break;
                    case 'percentage':
                        html = '<div><input type="text" placeholder="' + (attrs.placeholder == undefined || attrs.placeholder == "" ? '' : attrs.placeholder) + '" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" tooltipview/></div>';
                        break;
                    case 'file':
                        //debugger
                        html = '<div><input type="file" kk="' + scope.kk + '" datactrl="' + scope.datactrl + '"  value="' + attrs.value + '" name="' + attrs.name + '"  ng-model="ngModel" accept="image/*|video/*"  app-filereader/></div>';
                        break;
                    case 'slider':
                        html = '<div><input type="file" datactrl="' + scope.datactrl + '" value="' + attrs.value + '" name="' + attrs.name + '"  ng-model="ngModel" accept="image/*"  app-filereader multiple/></div>';
                        break;
                    case 'singlefile':
                        html = '<div><input type="file" datactrl="' + scope.datactrl + '" value="' + attrs.value + '" name="' + attrs.name + '"  ng-model="ngModel" accept="image/*" app-filesinglereader/></div>';
                        break;
                    case 'datetime':
                        html = '<input type="text" ng-value="' + attrs.value + '" name="' + attrs.name + '" k-datetimepicker class="form-control k-datetimepicker" ng-change="' + attrs.change + '" ng-keyup="' + attrs.keyup + '" ng-model="ngModel" ng-disabled="' + attrs.disable + '" />';
                        break;
                    case 'time':
                        html = '<input type="text" ng-value="' + attrs.value + '" name="' + attrs.name + '" k-timepicker class="form-control k-timepicker" ng-change="' + attrs.change + '" ng-keyup="' + attrs.keyup + '" ng-model="ngModel" ng-disabled="' + attrs.disable + '" />';
                        break;
                    case 'password':
                       html = '<input type="password" value="" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-disabled="' + attrs.disable + '" ng-change="' + attrs.change + '" ng-keyup="' + attrs.keyup + '"  />';
                       break;
                    //case 'mainimage':
                    //    html = '<div><input type="file"  value="' + attrs.value + '" name="' + attrs.name + '"  ng-model="ngModel" accept="image/*"  app-filereader/></div>';
                    //    break;
                    default:
                        html = '<div><input type="' + attrs.sgType + '" placeholder="' + (attrs.placeholder == undefined || attrs.placeholder == "" ? '' : attrs.placeholder) + '" value="' + attrs.value + '" name="' + attrs.name + '" class="form-control" ng-model="ngModel" ng-change="' + attrs.change + '" ng-disabled="' + attrs.disable + '" ng-keyup="' + attrs.keyup + '"  tooltipview/></div>';
                }
                elem.replaceWith($compile(html)(scope));
            }
        }
    }

    var sgSelectDirective = function ($http, $filter, $translate, $timeout) {
        var link = function (scope, elem, attrs, ctrl) {
            elem.addClass('sg-input-container');
            scope.vm = {};
            scope.isExternal = true;
            scope.attrs = attrs;
            scope.vm.search = {};
            scope.vm.currentRow = {};
            scope.vm.selectedRows = [];
            scope.vm.selectedIds = [];
            scope.vm.binding = [];
            scope.required = attrs.ngRequired;

            if (!attrs.label) {
                elem.find('.control-label').remove();
                elem.find('.form-control-wrapper').css({ width: "100%" });
            }
            if (typeof attrs.multi !== "undefined") {
                attrs.multi = true;
            }
            if (typeof attrs.returnCol === "undefined") {
                attrs.returnCol = "id";
            }
            if (typeof attrs.returnAs === "undefined") {
                attrs.returnAs = "name";
            }            
            scope.vm.selectRow = function (row) {
                scope.isExternal = false;
                scope.vm.closeModal();
                if (attrs.multi && row[attrs.returnCol]) {
                    scope.vm.selectedRows.push(row);
                } else {
                    scope.vm.currentRow = row;
                    ctrl.$setViewValue(row[attrs.returnCol]);
                }
            };
            scope.vm.unSelectRow = function (row) {
                scope.isExternal = false;
                scope.vm.selectedRows.splice(scope.vm.selectedRows.indexOf(row), 1);
            };
            scope.$watch('vm.selectedRows', function (newVal, oldVal) {
                if (attrs.multi && newVal !== oldVal) {
                    scope.vm.selectedIds = "";
                    angular.forEach(newVal, function (row) {
                        scope.vm.selectedIds += "," + row[attrs.returnCol];
                        //scope.vm.selectedIds.push(row[attrs.returnCol]);
                    });
                    scope.isExternal = false;
                    ctrl.$setViewValue(scope.vm.selectedIds);
                }
            }, true);

            elem.find('.search-box').bind('blur', function () {
                $timeout(function () {
                    ctrl.$setTouched();
                }, 100);
            });

            elem.find('.select-box').keydown(function (e) {
                if (e.keyCode == 9 || e.keyCode == 16 || e.keyCode == 27)
                    return;

                if (e.keyCode == 32) { // when space ignore keydown
                    e.preventDefault();
                }

                elem.find('.search-box').val("");
                elem.find('.search-box').keydown()[0].focus();
                openDropList();

            });
            elem.find('.sg-select').bind('click keydown keyup', function (e) {
                if (e.keyCode == 27 || e.keyCode == 9)
                    scope.vm.closeModal();
                e.stopPropagation();
            });
            elem.find('.search-box').bind('keydown', function (e) {
                if (e.keyCode == 13) {
                    scope.vm.selectRow(scope.vm.selectedRow);
                    scope.vm.closeModal();
                    scope.$apply();
                }
                if ((e.keyCode == 40 || e.keyCode == 38) && scope.vm.binding.length > 0) {
                    try {
                        if (!scope.vm.selectedRow[attrs.returnCol]) {
                            scope.vm.selectedRow = scope.vm.binding[0];
                            scope.$apply();
                            return;
                        }
                        var row = elem.find('tr.selected')[0];
                        var container = elem.find('.sg-table')[0];
                        var scrollHeight = container.scrollHeight;
                        var offset = 0;

                        var currIndex = scope.vm.binding.indexOf(scope.vm.selectedRow);
                        var length = scope.vm.binding.length;
                        if (row) {
                            offset = (row.rowIndex + 1) * row.clientHeight;
                        }
                        if (e.keyCode == 40) {
                            if (currIndex + 1 >= length)
                                return;
                            scope.vm.selectedRow = scope.vm.binding[currIndex + 1];
                            scope.$apply();
                            offset += row.clientHeight;
                        }
                        if (e.keyCode == 38) {
                            scope.vm.selectedRow = scope.vm.binding[currIndex - 1 < 0 ? 0 : currIndex - 1];
                            scope.$apply();
                            offset -= row.clientHeight;

                        }

                        if (offset > container.clientHeight + container.scrollTop) {
                            $(container).animate({ scrollTop: offset - container.clientHeight }, 100);
                        }
                        if (offset < container.scrollTop + row.clientHeight) {
                            $(container).animate({ scrollTop: offset - row.clientHeight }, 100);
                        }
                    } catch (e) {
                    }

                }

            });

            var openDropList = function () {
                $('.sg-select').removeClass('open');

                elem.find('.sg-table').scrollTop(0);
                var bodyRect = document.body.getBoundingClientRect(),
                        elemRect = elem.find('.sg-select')[0].getBoundingClientRect(),
                        offset = bodyRect.height - elemRect.bottom - window.scrollY;
                if (offset < 250)
                    elem.find('#droplist').css({ bottom: elemRect.height, top: "", display: "none" }).slideDown(200);
                else {
                    elem.find('#droplist').css({ bottom: "", top: elemRect.height, display: "none" }).slideDown(200);
                    //elem.find('#droplist').css({bottom: "", top: 0, display: "none"}).slideDown(200);
                }
                elem.find('.sg-select').addClass('open');
                elem.find('input:first').focus();
                scope.vm.selectedRow = {};
                scope.vm.search = {};
                scope.vm.undo(undefined, undefined, 1);
                //scope.$apply();
            };

            scope.vm.showModal = function () {
                if (elem.find('.sg-select').hasClass('open')) {
                    scope.vm.closeModal();
                    return;
                }
                openDropList();
            };
            scope.vm.closeModal = function () {
                elem.find('#droplist').slideUp(200, function () {
                    elem.find('.sg-select').removeClass('open');
                    $(this).css({ display: "block" });
                    //elem.find('.select-box').focus();
                });

            };

            scope.vm.undo = function (ids, callback, type) {
                var Id = 0;
                var excludedid = "";
                var KPIId = "";
                var compareType = "";
                if (typeof attrs.excludedid === "undefined") {
                    //do nothing
                } else {
                    try { attrs.excludedid = JSON.parse(attrs.excludedid);}catch (ex) {}
                    if (type != undefined) {
                        switch (attrs.excludedid.Name) {
                            case "StrategicGoalListManagement":
                                angular.forEach(scope.$parent.$parent.model['StrategicGoalList'], function (value, key) {
                                    if (attrs.excludedid.Index == key) {
                                        angular.forEach(value.ManagementList, function (valuep, keyp) {
                                            if (valuep.ManagementId != undefined && valuep.ManagementId != "") {
                                                excludedid += "," + valuep.ManagementId;
                                            }
                                        });
                                    }
                                });
                                break;
                            case "StrategicGoal":
                                angular.forEach(scope.$parent.$parent.model['StrategicGoalList'], function (value, key) {
                                    if (value.StrategicGoalId != undefined && value.StrategicGoalId != "") {
                                        excludedid += "," + value.StrategicGoalId;
                                    }
                                });
                                break;
                            default:
                                //angular.forEach(scope.$parent.$parent.model[attrs.excludedid], function (value, key) {
                                //        excludedid += "," + value.SelectedProjectId;
                                //});
                                break;
                        }
                       
                    }
                }
                
                scope.vm.binding = [];
                var Url = _apiGetUrl + attrs.datasrc;
                if (attrs.datasrc.includes("=")) {
                    Id = parseInt(attrs.datasrc.split('=')[1]);
                    Url = _apiGetUrl + attrs.datasrc.split('?')[0];
                }

                var search = scope.vm.search;
                if (ids) search = ids;
                var paging = {
                    "token": attrs.token || scope.$parent.token,
                    "pagination": {
                        "start": 0,
                        "Count": 20
                    },
                    "sort": {
                        "predicate": attrs.returnCol
                    },
                    "search": JSON.stringify(search),
                    "filter": JSON.stringify(scope.filter),
                    "search_operator": '{"' + attrs.returnAs + '":"in","' + attrs.returnCol + '":"eq"}',
                    "id": Id,
                    "excludedId": excludedid,
                    "KPIId": KPIId,
                    "compareType": attrs.comparetype
                };
                $http.post(Url, paging).then(function (response) {
                    if (response.data.data) {
                        scope.vm.binding = response.data.data;
                        if (response.data.data.length > 0 && scope.vm.selectedRows.length < response.data.data.length)
                            attrs.searchmessage = resources.SearchFound;
                        else
                            attrs.searchmessage = resources.NoSearchFound;
                        //if (scope.vm.binding.length < 20) {
                        //	scope.vm.undo = function (search,callback) {
                        //		if (typeof callback === "function") callback(scope.vm.binding);
                        //	}
                        //}
                    }

                    if (typeof callback === "function") callback(scope.vm.binding);

                });
            };
            if (attrs.datasrc.search('ListOfValues') !== -1) {
                scope.vm.undo = function (ids, callback) {
                    var listId = attrs.datasrc.split("/")[1];
                    var listOfValues = [];
                    angular.forEach(angular.ListOfValues[listId], function (value, key) {
                        listOfValues.push({ Id: key, Name: value });
                    });
                    scope.vm.binding = listOfValues;
                    if (typeof callback === "function") callback(scope.vm.binding);
                }
            }

            scope.$watch(function () {
                var status = true;
                angular.forEach(scope.filter, function (val, key) {
                    if (!val)
                        status = false;
                });
                if (!status)
                    return false;
                return scope.filter;
            }, function (newVal, oldVal) {
                if (oldVal !== false && newVal != oldVal) {
                    scope.ngModel = null;
                    scope.vm.currentRow = {};
                }
            }, true);
            scope.$watch('ngModel', function (newVal, oldVal) {
                if (angular.isFunction(scope.sgChange) && newVal !== oldVal) { scope.sgChange(); }
                if (newVal && scope.isExternal) {
                    var obj = {};
                    if (attrs.multi)
                        scope.vm.selectedRows = [];
                    if (angular.isArray(scope.ngModel) && scope.ngModel.length > 0) {
                        obj[attrs.returnCol] = scope.ngModel;

                    } else if (angular.isNumber(scope.ngModel)) {
                        obj[attrs.returnCol] = scope.ngModel;
                    } else {
                        obj[attrs.returnCol] = scope.ngModel;
                    }

                    scope.vm.undo(obj, function (data) {

                        angular.forEach(data, function (row) {
                            if (attrs.multi) {
                                scope.vm.selectedRows.push(row);
                            } else
                                scope.vm.currentRow = row;
                        });
                    });
                } else if (!newVal) {
                    scope.vm.currentRow = {};
                    if (attrs.multi)
                        scope.vm.selectedRows = [];
                } else {
                    scope.isExternal = true;
                }
            }, true);


            ctrl.messages = {};
            var reqMessage = attrs.label || attrs.hiddenLabel || null;
            if (reqMessage)
                reqMessage = '"' + reqMessage + '"';
            $translate('seagull.validation.REQUIRED', { param: reqMessage }).then(function (d) {
                ctrl.messages.required = d;
            });

            scope.getMessage = function () {
                for (var i in ctrl.messages) {
                    if (ctrl.$error[i]) {
                        return ctrl.messages[i];
                    }
                }
            };
            scope.getMessage();
            if (typeof attrs.disabled !== "undefined" && attrs.disabled != "false" && attrs.disabled) {
                scope.disabled = true;
                scope.vm.openDropList = null;
                scope.vm.closeModal = null;
                scope.vm.showModal = null;
                scope.vm.unSelectRow = null;
                elem.find('.form-control').attr('disabled', true);
            }
        };
        return {
            restrict: 'E',
            require: 'ngModel',
            templateUrl: function (elem, attrs) {
                attrs.comparetype = attrs.comparetype == undefined ? "" : attrs.comparetype;
                attrs.disabled = attrs.disabled == undefined ? false : attrs.disabled;
                attrs.searchmessage = attrs.searchmessage == undefined ? "Search" : attrs.searchmessage;
                if (typeof attrs.multi !== "undefined") return _templatesUrl + "sg-select-multi.html"
                return _templatesUrl + "sg-select.html"
            },
            scope: {
                ngModel: '=',
                returnModel: '=?',
                filter: '=?',
                sgChange: '&?'
            },
            link: link
        };
    }

    var sgValidateDirective = function ($parse) {
        var link = function (scope, element, attrs, sgInputCtrl) {
            //if (attrs.condition && attrs.condition.includes('moment')) scope.moment = moment;
            attrs.$observe('message', function (value) { sgInputCtrl.messages[attrs.name] = value; });
            attrs.$observe('condition', function (value) {
                sgInputCtrl.$setValidity(attrs.name, value == "true" ? true : false);
            });

            //scope.$watch(function () { return scope.$eval(attrs.condition); console.log("zz", scope.$interpolate(attrs.condition)) }, function (newVal, oldVal) {
            //    sgInputCtrl.$setValidity(attrs.name, newVal);
            //    if (newVal !== oldVal) {
            //        sgInputCtrl.$setTouched();
            //    }
            //});
        };
        return {
            restrict: 'E',
            require: "^^ngModel",
            link: link
        };
    };

    var sgConfirmDirective = function ($compile) {
        return {
            restrict: 'AE',
            scope: {
                sgConfirm: '@',
                modalHeader: '=',
                modalBody: '='
            },
            link: function (scope, element, attrs) {

                $(element).on('click', function (e) {
                    $(element).after($compile('<sg-modal modal-action="' + scope.sgConfirm + '" modal-header="' + scope.modalHeader + '" modal-body="' + scope.modalBody + '"></sg-modal>')(scope.$parent));
                });
            }
        };
    };

    var sgModalDirective = function () {
        return {
            templateUrl: _templatesUrl + 'sg-modal.html',
            link: function (scope, element, attrs, sgModalCtrl) {
                $('#SgModal').modal('show');
                element.find('[data-dismiss]').bind('click', function () {
                    $('#SgModal').modal('hide');
                    setTimeout(function () {
                        $('sg-modal').remove();
                    }, 300);
                })
                scope.action = function () {
                    scope.$parent.$eval(scope.modalAction);
                }
            },
            scope: {
                modalAction: '@',
                modalHeader: '@',
                modalBody: '@'
            }
        }
    };

    var sgFieldTypeDirective = function () {
        return {
            link: function (scope, element, attrs, modelCtrl) {
                element.bind("keypress", function ($event) {
                    var char = String.fromCharCode($event.keyCode);
                    if (!(char.match(/[\d.,-]/g) || $event.keyCode === 13 || $event.keyCode === 188 || $event.keyCode === 189 || $event.keyCode === 190) && attrs.sgFieldType == 'number') {
                        $event.preventDefault();
                    }
                });

            }
        };
    }

    var sgFormToolbarDirective = function () {
        return {
            
            template: function (elem, attrs) {
                var perms = {
                    save: true,
                    savec: false,
                    back: true,
                    savedraft: false,
                    undo: false,
                    backtoproject: false,
                    saven: "",
                    formname: "",
                    saveeditPlan: false,
                    saveandsendplan: false,
                    acceptplanandsendtomanagement: false,
                    saveliaisonofficer :false,
                    showmodel:"",
                    backurl:""

                }
                if (typeof attrs.formname != "undefined") {
                    perms.formname = attrs.formname;

                    if (typeof attrs.back != "undefined") {
                        perms.back = attrs.back;
                    }

                    if (typeof attrs.backurl != "undefined") {
                        perms.backurl = attrs.backurl;
                    }

                    if (typeof attrs.saveeditplan != "undefined") {
                        perms.saveeditplan = attrs.saveeditplan;
                    }
                    if (typeof attrs.saveandsendplan != "undefined") {
                        perms.saveandsendplan = attrs.saveandsendplan;
                    }
                    if (typeof attrs.acceptplanandsendtomanagement != "undefined") {
                        perms.acceptplanandsendtomanagement = attrs.acceptplanandsendtomanagement;
                    }
                    if (typeof attrs.acceptplanandsendtoliaisonofficer != "undefined") {
                        perms.acceptplanandsendtoliaisonofficer = attrs.acceptplanandsendtoliaisonofficer;
                    }
                    if (typeof attrs.saveliaisonofficer != "undefined") {
                        perms.saveliaisonofficer = attrs.saveliaisonofficer;
                    }
                    if (typeof attrs.showmodel != "undefined") {
                        perms.showmodel = attrs.showmodel;
                    }
                    
                    return '<div class="toolbar"><div class="btn-group">' +
                        //'<button ng-if ="' + perms.save + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(false,false)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="' + (perms.saven == '' ? 'seagull.buttons.SAVE' : perms.saven) + '">Save</span></button>' +
                        //'<button ng-if ="' + perms.saveeditplan + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(true,false,\'continueentering\')" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE_EDITPLAN">Save & Continue</span></button>' +
                        //'<button ng-if ="' + perms.saveandsendplan + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(true,false,\'sendtomanagement\')" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.Accept_senttomanagement">Save & Continue</span></button>' +
                        //'<button ng-if ="' + perms.acceptplanandsendtomanagement + '" type="button" class="btn btn-success btn-sm" ng-click="vm.submit(true,false,\'acceptplanandsendtodepartment\')" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE_SENDPLAN">Save & Continue</span></button>' +
                        //'<button ng-if ="' + perms.acceptplanandsendtoliaisonofficer + '" type="button" class="btn btn-success btn-sm" ng-click="vm.submit(true,false,\'acceptplanandsendtoliaisonofficer\')" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE_SENDPLANTOLiaisonOfficer">Save & Continue</span></button>' +
                        //'<button ng-if ="' + perms.saveliaisonofficer + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(false,false,\'saveliaisonofficer\')" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE">Save</span></button>' +
                        //'<button type="button" class="btn btn-success btn-sm" ng-click="vm.showModalForm(\'' + perms.showmodel + '\')" ><i class="fa fa-file-text"></i> <span class="hidden-md-down" translate="seagull.buttons.Show_Plan">Show_Plan</span></button>' +
                        '<button ng-if ="' + perms.undo + '"type="button" class="btn btn-warning btn-sm" ng-click="vm.getData()" ng-disabled="vm.editLock"><i class="fa fa-undo"></i> <span class="hidden-md-down" translate="seagull.buttons.UNDO">Undo</span></button>' +
                        '<a  href="' + perms.backurl+'" class="btn btn-warning btn-sm"><i class="fa fa-arrow-left"></i> <span class="hidden-md-down" translate="seagull.buttons.BACK">Back</span></a>' +
                        '</div></div>'
                }
                if (typeof attrs.save != "undefined") {
                    perms.save = attrs.save;
                }
                if (typeof attrs.savec != "undefined") {
                    perms.savec = attrs.savec;
                }
                if (typeof attrs.backtoproject != "undefined") {
                    perms.backtoproject = attrs.backtoproject;
                }
                if (typeof attrs.back != "undefined") {
                    perms.back = attrs.back;
                }
                if (typeof attrs.undo != "undefined") {
                    perms.undo = attrs.undo;
                }
                if (typeof attrs.savedraft != "undefined") {
                    perms.savedraft = attrs.savedraft;
                }
                if (typeof attrs.saven != "undefined") {
                    perms.saven = attrs.saven;
                }
                return '<div class="toolbar"><div class="btn-group">' +
							'<button ng-if ="' + perms.save + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(false,false)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="' + (perms.saven == '' ? 'seagull.buttons.SAVE' : perms.saven) + '">Save</span></button>' +
							'<button ng-if ="' + perms.savec + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(true,false)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE_CONTINUE">Save & Continue</span></button>' +
                    	     '<button ng-if ="' + perms.backtoproject + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(true,false)" ng-disabled="vm.editLock"> <span class="hidden-md-down" translate="seagull.buttons.Project_MainPage">back To Project</span></button>' +
                            '<button ng-if ="' + perms.savedraft + '" type="button" class="btn btn-primary btn-sm" ng-click="vm.submit(true,true)" ng-disabled="vm.editLock"><i class="fa fa-save"></i> <span class="hidden-md-down" translate="seagull.buttons.SAVE_DRAFT">Save As Draft</span></button>' +
							'<button ng-if ="' + perms.undo + '"type="button" class="btn btn-warning btn-sm" ng-click="vm.getData()" ng-disabled="vm.editLock"><i class="fa fa-undo"></i> <span class="hidden-md-down" translate="seagull.buttons.UNDO">Undo</span></button>' +
							'<a  ng-if ="' + perms.back + '" href="/{{vm.ctrl}}" class="btn btn-warning btn-sm"><i class="fa fa-arrow-left"></i> <span class="hidden-md-down" translate="seagull.buttons.BACK">Back</span></a>' +
					  '</div></div>'
            }
        }
    }

    var kDatepickerDirective = function () {
        return {
            require: 'ngModel',
            scope: { ngModel: '=' },
            link: function (scope, elem, attrs, ctrl) {
                $(elem).on('change', function () {
                    ctrl.$setViewValue($(this).val());
                    scope.$apply();
                });
                setTimeout(function () {
                    elem.kendoDatePicker({
                        format: "dd/MM/yyyy",
                        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
                    });
                    if (scope.ngModel != undefined)
                        elem.data("kendoDatePicker").value(scope.ngModel);
                }, 0)
            }
        }
    };

    var kDatetimepickerDirective = function () {
        return {
            require: 'ngModel',
            scope: { ngModel: '=' },
            link: function (scope, elem, attrs, ctrl) {
                $(elem).on('change', function () {
                    ctrl.$setViewValue($(this).val());
                    scope.$apply();
                });
                setTimeout(function () {
                    elem.kendoDateTimePicker({
                        format: "dd/MM/yyyy hh:mm tt", //format is used to format the value of the widget and to parse the input.
                        timeFormat: "hh:mm tt", //this format will be used to format the predefined values in the time list.
                    });
                    if (scope.ngModel != undefined)
                        elem.data("kendoDateTimePicker").value(scope.ngModel);
                }, 0)
            }
        }
    };
    var kTimepickerDirective = function () {
        return {
            require: 'ngModel',
            scope: { ngModel: '=' },
            link: function (scope, elem, attrs, ctrl) {
                $(elem).on('change', function () {
                    ctrl.$setViewValue($(this).val());
                    scope.$apply();
                });
                setTimeout(function () {
                    elem.kendoTimePicker({
                        //format: "dd/MM/yyyy hh:mm tt", //format is used to format the value of the widget and to parse the input.
                        //timeFormat: "hh:mm tt", //this format will be used to format the predefined values in the time list.
                        dateInput: true
                    });
                    if (scope.ngModel != undefined)
                        elem.data("kendoTimePicker").value(scope.ngModel);
                }, 0)
            }
        }
    };


    var bodyDirective = function () {
        return {
            link: function (scope, elem, attrs) {
                elem.bind('click keydown keyup', function (e) {
                    elem.find('.sg-select').removeClass('open');
                    scope.$root.showbox = {};
                    scope.$apply();
                });
            }
        };
    };

    var tooltipDirective = function ($timeout) {
        return {
            link: function (scope, elem, attrs) {
                var tooltip = $("");
                var status = "";
                elem.bind('mouseenter ', function () {
                    status = "mouseenter"
                    $timeout(function () {
                        if (status == "mouseenter") {
                            tooltip = $('<div class="tooltip-text">' + attrs.tooltip + '</div>');
                            $('body').prepend(tooltip);
                            $timeout(function () {
                                elemRect = elem[0].getBoundingClientRect();
                                tooltipRect = tooltip[0].getBoundingClientRect();
                                tooltip.css({ visibility: 'visible', opacity: 1, top: elemRect.top - tooltipRect.height - 5 + $(window).scrollTop(), left: elemRect.left - tooltipRect.width / 2 + elemRect.width / 2 });
                            }, 0);
                        }
                    }, 300);

                });

                elem.bind('mouseleave ', function () {
                    status = "mouseleave";
                    $('.tooltip-text').fadeOut(100, function () { $(this).remove(); });
                });
            }
        };
    };

    var sgPageDirective = function () {
        return {
            restrict: 'E',
            link: function (scope, elem, attrs, ctrl, transclude) {
                elem.wrapInner('<div class="sg-page-content"></div>');
                if (typeof attrs.title !== "undefined") {
                    elem.prepend('<h1 class="sg-page-title"><i class="fa fa-th-large"></i> <span>' + attrs.title + '</span></h1>');
                }
            }
        }
    }

    var sgTabsDirective = function () {
        var link = function (scope, elem, attrs, ctrl) {
            //css('display', "block")
            //debugger
            elem.find('.tab-content:first>.tab-pane#tab-1').css('display', "block");
            elem.find('ul.nav-tabs:first>li:first').addClass("active");
            elem.find('ul.nav-tabs:first>li').bind('click', function () {
                elem.find('ul.nav-tabs:first>li').removeClass('active');
                $(this).addClass("active");
                //debugger
                //$(".LineModel").fadeIn("slow");
                //debugger
                //elem.find('.tab-content:first>.tab-pane').css("display", "none");
                elem.find('.tab-content:first>.tab-pane').hide();
                //elem.find('.tab-content:first>.tab-pane#' + $(this).attr('tab-toggle')).css({ display: "block" });
                elem.find('.tab-content:first>.tab-pane#' + $(this).attr('tab-toggle')).show();
            });
        }, controller = function ($element) {
            var vm = this;
            var count = 0;
            $element.addClass("nav-tabs-custom").css("display", "block");
            $element.append('<ul class="nav nav-tabs"></ul>');
            $element.append('<div class="tab-content container "/>');
            vm.addTab = function (clone, title, show) {
                show = show == undefined ? 'true' : show;
                if (show == 'true') {
                    count++;
                    //display:none;
                    var c = $('<div class="tab-pane" id="tab-' + count + '" style=""></div>');
                    c.append(clone);
                    $element.find("ul.nav-tabs:first").append('<li tab-toggle="tab-' + count + '"><a class="nav-link">' + title + '</a></li>');
                    $element.find(".tab-content:first").append(c);
                }
            };
        };
        return {
            link: link,
            controller: controller,
            controllerAs: "vm"
        };
    };

    var sgTabDirective = function () {
        var link = function (scope, elem, attrs, sgTabsCtrl, transclude) {
            transclude(scope, function (clone) {
                console.log("sgTab", sgTabsCtrl);
                sgTabsCtrl.addTab(clone, attrs.title,attrs.show);
            });
            elem.remove();
        };
        return {
            require: '^sgTabs',
            transclude: true,
            link: link,
            template: ""
        };
    };

    var sgBoxDirective = function () {
        var link = function (scope, elem, attrs) {
            elem.wrapInner('<div class="box-body"/>');

            var header = $('<div class="box-header with-border clearfix">' +
                    '<div class="box-title"><i class="' + attrs.icon + '"></i> <span>' + attrs.label + '</span></div>' +
                    '<div class="box-tools pull-right"><button type="button" class="btn btn-box-tool" data-widget="collapsePanel"><text><i class="fa fa-plus"></i></text></button></div>' +
                    '</div>');
            elem.prepend(header);
            elem.wrapInner('<div class="box box-info"/>');
            elem.addClass('open');
            var body = elem.find('.box-body');
            elem.find("[data-widget='collapsePanel']").bind('click', function () {
                if (elem.hasClass('open')) {
                    body.slideUp();
                    elem.removeClass('open');
                    elem.find("[data-widget='collapsePanel'] .fa-plus").addClass('fa-minus');
                } else {
                    body.slideDown();
                    elem.addClass('open');
                    elem.find("[data-widget='collapsePanel'] .fa-plus").removeClass('fa-minus');
                }
            });

        };
        return {
            link: link
        };
    };

    var fileDirective = function () {
        return {
            scope: {
                file: '='
            },
            link: function (scope, el, attrs) {
                el.bind('change', function (event) {
                    var file = event.target.files[0];
                    scope.file = file ? file : undefined;
                    scope.$apply();
                });
            }
        };
    };

    app.directive('sgTable', sgTableDirective);

    app.directive('sgCol', sgColDirective);

    app.directive('sgColchild', sgColChildDirective);

    app.directive('sgOutput', sgOutputDirective);

    app.directive('sgChildoutput', sgOutputChildDirective);

    app.directive('sgForm', sgFormDirective);

    app.directive('sgFormToolbar', sgFormToolbarDirective);

    app.directive('sgInput', sgInputDirective);

    app.directive('sgValidate', sgValidateDirective);

    app.directive('sgFieldType', sgFieldTypeDirective);

    app.directive('sgType', sgTypeDirective);

    app.directive('sgSelect', sgSelectDirective);

    app.directive('sgMultiSelect', sgSelectDirective);

    app.directive('kDatepicker', kDatepickerDirective);

    app.directive('sgPage', sgPageDirective);

    app.directive('sgTabs', sgTabsDirective);

    app.directive('sgTab', sgTabDirective);

    app.directive('sgBox', sgBoxDirective);

    app.directive('sgModal', sgModalDirective);

    app.directive('body', bodyDirective);

    app.directive('file', fileDirective);

    app.directive('tooltip', tooltipDirective);

    app.directive('kDatetimepicker', kDatetimepickerDirective);

    app.directive('kTimepicker', kTimepickerDirective);

    app.directive('sgConfirm', sgConfirmDirective);

    app.directive('format', function ($filter, $browser) {
        'use strict';

        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                if (!ctrl) {
                    return;
                }
                
                var listener = function () {
                    var _value = elem.val();
                    var value = _value.match(/\d/g);
                    if (value != null) {
                        value = value.join("");
                        value = value.replace(/[\,\.]/g, '');
                    }
                    else
                        value = "0";
                    elem.val($filter('number')(value, false));
                }

                // This runs when we update the text field
                ctrl.$parsers.push(function (viewValue) {
                    var _value = elem.val();
                    var value = _value.match(/\d/g);
                    if (value != null) {
                        value = value.join("");
                        value = value.replace(/[\,\.]/g, '');
                    }
                    else
                        value = "0";
                    return value;
                })

                // This runs when the model gets updated on the scope directly and keeps our view in sync
                ctrl.$render = function () {
                    elem.val($filter('number')(ctrl.$viewValue, false))
                }

                elem.bind('change', listener)
                elem.bind('keydown', function (event) {
                    var key = event.keyCode
                    // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, do nothing.
                    // This lets us support copy and paste too
                    if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40))
                        return
                    $browser.defer(listener) // Have to do this or changes don't get picked up properly
                })

                elem.bind('paste cut', function () {
                    $browser.defer(listener)
                })
            }
        };
    });

    app.directive('tooltipview', function ($filter, $browser, $timeout) {
        'use strict';
        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                var divelem = elem.parent().closest('div');
                divelem.bind('mouseenter ', function () {
                    status = "mouseenter"
                    $timeout(function () {
                        if (status == "mouseenter") {
                            var tooltip = $('<div class="tooltip-text">' + elem.val() + '</div>');
                            $('body').prepend(tooltip);
                            $timeout(function () {
                                var elemRect = elem[0].getBoundingClientRect();
                                var tooltipRect = tooltip[0].getBoundingClientRect();
                                tooltip.css({ visibility: 'visible', opacity: 1, zIndex: 1100, top: elemRect.top - tooltipRect.height - 5 + $(window).scrollTop(), left: elemRect.left - tooltipRect.width / 2 + elemRect.width / 2 });
                            }, 300);
                        }
                    }, 300);
                });
                divelem.bind('mouseleave ', function () {
                    status = "mouseleave";
                    $('.tooltip-text').fadeOut(100, function () { $(this).remove(); });
                });
            }
        };
    });

    app.directive('sgRepeatDirective', function () {
        //Loading Ajax Busy
        ShowLoading();//$('#loading-wrapper').fadeIn('slow');
        return function (scope, element, attrs) {
            if (scope.$last) {
                // iteration is complete, do whatever post-processing

                //Remove Loading Ajax Busy
                //RemoveLoading();
            }
        };
    })

    app.directive('sgReady', function ($parse) {
        return {
            restrict: 'A',
            link: function ($scope, elem, attrs) {

            }
        }
    })

    app.directive('appFilereader', function ($q, $http) {
        var slice = Array.prototype.slice;
        return {
            restrict: 'A',
            require: '?ngModel',
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel) return;

                ngModel.$render = function () { };

                element.bind('change', function (e) {
                    if (scope.datactrl != undefined) {
                        var element = e.target;
                        var token = scope.$parent.$parent.myform.Token.$viewValue;
                        var Id = scope.$parent.$parent.myform.Id.$viewValue;
                        var file = new FormData();
                        var type = scope.kk;
                        //var result = document.getElementsByClassName("multi-files");
                        //var el = element.second
                        //var eq = element.eq(1);


                        file.append("token", token);
                        file.append("Id", Id);
                        file.append("IsMultiple", element.multiple);
                        file.append("Type", element.name);
                        file.append("type", type);

                        if (element.multiple) {
                            var tempFiles = [];
                            if (ngModel.$viewValue != undefined) {
                                tempFiles = ngModel.$viewValue;
                            }
                            if (element.files != undefined && element.files.length > 0)
                                $.each(element.files, function (index) {
                                    var obj = {};
                                    obj.Name = element.files[index].name;
                                    file.append('file_' + index, element.files[index]);
                                    tempFiles.push(obj);
                                });
                        } else {
                            var tempfiles;
                            if (ngModel.$viewValue !== undefined) {
                                tempfiles = ngModel.$viewValue;
                            }
                            file.append('file', element.files[0]);
                            tempfiles.Name = element.files[0].name;
                            ngModel.$setViewValue(tempfiles);
                        }

                        $http.post(_apiPostUrl + scope.datactrl + "/UplodeFile", file, {
                            transformRequest: angular.identity,
                            headers: { 'Content-Type': undefined, 'Process-Data': false }
                        }).then(function (result) {
                            if (element.multiple) {
                                var target = angular.element('#SliderDiv');
                            } else {
                                
                                if (element.name == "InstructionImage") {
                                    var target = angular.element('#InstructionImage');
                                } else {
                                    if (type == null)
                                        var target = angular.element('#imagesDiv');
                                    else
                                        var target = angular.element('#' + type);
                                }
                                target.empty();
                            }
                            if (result.count != 0) {
                                $.each(result.data, function (index, value) {
                                    target.append(`
                            <img class="img-responsive" style="max-width: 100%;" src="`+ value + `" />
                            `);
                                });
                            }
                        });
                    }
                    else {
                        $q.all(slice.call(element.files, 0).map(readFile))
                            .then(function (values) {
                                //element.multiple = true;
                                if (element.multiple) {
                                    var tempFiles = [];
                                    if (ngModel.$viewValue != undefined) {
                                        tempFiles = ngModel.$viewValue;
                                    }
                                    if (values.length > 0) {
                                        $.each(values, function (index, value) {
                                            var obj = {};
                                            obj.SRC = value;
                                            obj.Name = element.files[index].name;
                                            tempFiles.push(obj);
                                        });
                                    }
                                    ngModel.$setViewValue(tempFiles);
                                }
                                else {
                                    var tempFiles;
                                    if (ngModel.$viewValue != undefined) {
                                        tempFiles = ngModel.$viewValue;
                                    }
                                    if (values.length > 0) {
                                        $.each(values, function (index, value) {

                                            tempFiles.SRC = value;
                                            tempFiles.Name = element.files[index].name;
                                            ngModel.$setViewValue(tempFiles);
                                        });
                                    }
                                    ngModel.$setViewValue(tempFiles);
                                }
                                //ngModel.$setViewValue(values.length ? values[0] : null);
                            });

                        function readFile(file) {
                            var deferred = $q.defer();
                            var reader = new FileReader();
                            reader.onload = function (e) {
                                deferred.resolve(e.target.result);
                            };
                            reader.onerror = function (e) {
                                deferred.reject(e);
                            };
                            reader.readAsDataURL(file);

                            return deferred.promise;
                        }
                    }
                }); //change

            } //link
        }; //return
    });
    app.directive('appFilesinglereader', function ($q) {
        var slice = Array.prototype.slice;
        return {
            restrict: 'A',
            require: '?ngModel',
            link: function (scope, element, attrs, ngModel) {
                if (!ngModel) return;

                ngModel.$render = function () { };

                element.bind('change', function (e) {
                    var element = e.target;

                    $q.all(slice.call(element.files, 0).map(readFile))
                        .then(function (values) {

                            ngModel.$setViewValue(values.length ? values[0] : null);
                        });

                    function readFile(file) {
                        var deferred = $q.defer();
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            deferred.resolve(e.target.result);
                        };
                        reader.onerror = function (e) {
                            deferred.reject(e);
                        };
                        reader.readAsDataURL(file);

                        return deferred.promise;
                    }

                }); //change

            } //link
        }; //return
    });
})();

function isDate(value) {
    if (/^\d{1,2}\/\d{1,2}\/\d{4}$/.test(value))
        return true;
    else
        return false;
}

function RemoveLoading() {
    setTimeout(function () {
        $('#loading-wrapper').fadeOut('slow');
    }, 1000);
}

function ShowLoading() {
    try {
        $('#loading-wrapper').css('visibility', 'visible');
    }
    catch (e) {
    }
    try {
        $('#loading-wrapper')[0].hidden = false;
    } catch (e) {
    }
    try {
        $('#loading-wrapper').fadeIn('slow');

    } catch (e) {
    }
}

function GoToUrl(url) {
    //setTimeout(function () {
    //    window.location = url;
    //}, 5000);
    window.location = url;
}

function BuildView($scope, $http, _apiPostUrl, row, tablename) {
    switch (tablename) {
        case "EconomyPlanReport":
            $("#modelHeaderEconomyPlanReport").html(row.strEntityName + " : " + row.Month + " / " + row.Year)
            $http.post(_apiPostUrl + tablename + '/PartialPopupReportTask', { Id: row.Id, token: $scope.token }).then(function (response) {
                $('#EconomyPlanReportbody').html("");
                $('#EconomyPlanReportmodel').modal('toggle');
                $('#EconomyPlanReportmodel').modal('show');
                $('#EconomyPlanReportbody').html(response.data);
            });
            break;
        default:
            return;
    }
}

function BuildCustomAdd($scope, $http, _apiPostUrl, functionName) {
    switch (functionName) {
        case "AddProject":
            var input = "#Projectmodel";
            var _function = "angular.element($('" + input + "')).scope()." + functionName + "()";
            var fire = new Function("return (" + _function + ")")();
            break;
        default:
            return;
    }
}

function AdhocValidate($scope) {
    $scope.$broadcast('RunAdhocValidate');
    if ($scope.form.$invalid)
        return false;
    else
        return true;
}

$(document).ajaxStart(function () {
    ShowLoading();
});

$(document).ajaxComplete(function (event, request, settings) {
    RemoveLoading();
});
