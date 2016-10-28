userSwitcher.controller("HomeController", function ($scope, $http) {
    $scope.initialize = function () {
        $scope.users = [];
        $scope.selectedUser = {};
        $scope.getDefaultUserInfoFilePath();
        $scope.propertyName = null;
        $scope.reverse = false;
    };
    
    $scope.saveUser = function () {
        $scope.call({ api: '../Action/SaveUser', method: 'POST', data: { user: $scope.selectedUser } }, function (response) {
            if (response.Status) {
                $scope.getUserList();
                $scope.selectedUser = {};
            } else {
                alert(response.Message);
            }
        });
    };

    $scope.selectUser = function (user) {
        $scope.selectedUser = { UserId: user.UserId, Description: user.Description, NtLogin: user.NtLogin, IsDefault: user.IsDefault };
    };

    $scope.clearUserInfo = function () {
        $scope.selectedUser = {};
    };

    $scope.removeUser = function (userId) {
        $scope.call({ api: '../Action/RemoveUser', method: 'POST', data: { userId: userId } }, function (response) {
            if (response.Status) {
                $scope.getUserList();
            } else {
                alert(response.Message);
            }
        });
    };

    $scope.setUserDefault = function (userId) {
        $scope.call({ api: '../Action/SetDefaultUser', method: 'POST', data: { userId: userId, filePath: $scope.FilePath } }, function (response) {
            if (response.Status) {
                $scope.getUserList();
            } else {
                alert(response.Message);
            }
        });
    };

    $scope.getUserList = function () {
        $scope.call({ api: '../Data/GetUsers', method: 'POST' }, function (response) {
            if (response == undefined) {
                alert("Error Occurred while getting users!");
            } else {
                $scope.users = response;
            }
        });
    };

    $scope.getDefaultUserInfoFilePath = function () {
        $scope.call({ api: '../Data/GetDefaultUserInfoFilePath', method: 'POST' }, function (response) {
            if (response.Status) {
                $scope.FilePath = response.Data;
            } else {
                alert(response.Message);
            }
         
            $scope.getUserList();
            
        });
    };

    $scope.setDefaultUserInfoFilePath = function () {
        $scope.call({ api: '../Action/SetDefaultUserInfoFilePath', method: 'POST', data: { filePath: $scope.FilePath } }, function (response) {
            if (response.Status) {
                $scope.getDefaultUserInfoFilePath();
            } else {
                alert(response.Message);
            }
        });
    };

    $scope.sortBy = function (propertyName) {
        if ($scope.propertyName === propertyName) {
            if ($scope.reverse) {
                $scope.propertyName = null;
            } else {
                $scope.propertyName = propertyName;
            }

            $scope.reverse = !$scope.reverse;
        } else {
            $scope.reverse = false;
            $scope.propertyName = propertyName;
        }
    };

    // options = {api, data, method }
    $scope.call = function (options, onSuccess, onError) {
        var config = {
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            }
        };

        var request;
        if ('POST' == options.method) {
            request = function () { return $http.post(options.api, options.data, config) };
        }
        else {
            request = function () { return $http.get(options.api, config) };
        }

        request()
        .success(function (responseData, status, headers) {
            if (onSuccess) {
                onSuccess(responseData, status, headers);
            }
            else if (onError) {
                onError(responseData);
            }

        })
        .error(function (responseData) {
            if (onError) {
                onError(responseData);
            }
        });
    };
});