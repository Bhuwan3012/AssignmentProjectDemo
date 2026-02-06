var app = angular.module("taskApp", []);

app.controller("taskController", function ($scope, $http) {

    $scope.isSignup = false;

    var loginApiUrl = "https://localhost:7246/api/Auth/Login";
    var signupApiUrl = "https://localhost:7246/api/Auth/Signup";

    var apiBaseUrl = "https://localhost:7246/api/TaskApi";

    $scope.login = {};
    $scope.signup = {};

    $scope.tasks = [];
    $scope.task = {};
    $scope.isEdit = false;

    
    function getLoggedUser() {
        return JSON.parse(localStorage.getItem("loggedUser"));
    }

 
    function getAuthHeader() {
        var token = localStorage.getItem("token");
        return {
            headers: {
                "Authorization": "Bearer " + token
            }
        };
    }

  
    function setCreatedBy() {
        var loggedUser = getLoggedUser();
        if (loggedUser) {
            $scope.task.createdByName = loggedUser.fullName;
            $scope.task.createdById = loggedUser.uniqueId;
        }
    }


    setCreatedBy();


    $scope.showSignup = function () {
        $scope.isSignup = true;
        $scope.login = {};
    };


    $scope.showLogin = function () {
        $scope.isSignup = false;
        $scope.signup = {};
    };


    $scope.doSignup = function () {

        if (!$scope.signup.fullName || !$scope.signup.username || !$scope.signup.password) {
            alert("Full Name, Username, Password required!");
            return;
        }

        $http.post(signupApiUrl, $scope.signup)
            .then(function (res) {

                alert("Signup Successfully! Your ID: " + res.data.uniqueId);
                $scope.showLogin();

            }, function (err) {
                alert(err.data.message || "Signup Failed!");
                console.log(err);
            });
    };


    $scope.doLogin = function () {

        if (!$scope.login.username || !$scope.login.password) {
            alert("Username and Password required!");
            return;
        }

        $http.post(loginApiUrl, $scope.login)
            .then(function (res) {

                localStorage.setItem("token", res.data.token);
                localStorage.setItem("username", res.data.username);
                localStorage.setItem("fullName", res.data.fullName);
                localStorage.setItem("uniqueId", res.data.uniqueId);

                localStorage.setItem("loggedUser", JSON.stringify({
                    fullName: res.data.fullName,
                    uniqueId: res.data.uniqueId
                }));

                alert("Login Successfully!");

                window.location.href = "/Home/Privacy";

            }, function (err) {
                alert(err.data.message || "Invalid Username or Password!");
                console.log(err);
            });
    };


    $scope.loadTasks = function () {
        $http.get(apiBaseUrl + "/GetAll", getAuthHeader())
            .then(function (res) {
                $scope.tasks = res.data;
            }, function (err) {
                alert("Error loading tasks");
                console.log(err);
            });
    };

    $scope.createTask = function () {

        if (!$scope.task.taskTitle || !$scope.task.taskDescription || !$scope.task.taskDueDate || !$scope.task.taskStatus) {
            alert("Title, Description, Due Date, Status are required!");
            return;
        }

        setCreatedBy();

        $http.post(apiBaseUrl + "/Create", $scope.task, getAuthHeader())
            .then(function (res) {
                alert("Task Created Successfully!");
                $scope.loadTasks();
                $scope.clearForm();
            }, function (err) {
                alert("Error creating task");
                console.log(err);
            });
    };


    $scope.editTask = function (t) {

        var loggedUser = getLoggedUser();

        if (!loggedUser) {
            alert("User not logged in!");
            return;
        }

        $scope.task = angular.copy(t);

        $scope.task.lastUpdatedByName = loggedUser.fullName;
        $scope.task.lastUpdatedById = loggedUser.uniqueId;

        $scope.isEdit = true;
    };

    $scope.updateTask = function () {

        var loggedUser = getLoggedUser();

        if (!loggedUser) {
            alert("User not logged in!");
            return;
        }

        if (!$scope.task.taskId) {
            alert("TaskId missing!");
            return;
        }

        $scope.task.updatedByName = loggedUser.name;
        $scope.task.updatedById = loggedUser.uniqueId;

        $http.put(apiBaseUrl + "/Update/" + $scope.task.taskId, $scope.task, getAuthHeader())
            .then(function (res) {
                alert("Task Updated Successfully!");
                $scope.loadTasks();
                $scope.clearForm();
            }, function (err) {
                alert("Error updating task");
                console.log(err);
            });
    };

  
    $scope.deleteTask = function (taskId, createdById) {

        if (!confirm("Are you sure you want to delete?")) {
            return;
        }

        $http.delete(apiBaseUrl + "/Delete/" + taskId + "?createdById=" + createdById, getAuthHeader())
            .then(function (res) {
                alert("Task Deleted Successfully!");
                $scope.loadTasks();
            }, function (err) {
                alert("Error deleting task");
                console.log(err);
            });
    };

    $scope.searchTask = function () {

        if (!$scope.searchText || $scope.searchText.trim() === "") {
            $scope.loadTasks();
            return;
        }

        $http.get(apiBaseUrl + "/Search/" + $scope.searchText, getAuthHeader())
            .then(function (res) {
                $scope.tasks = res.data;
            }, function (err) {
                alert("Error searching task");
                console.log(err);
            });
    };

    $scope.clearForm = function () {
        $scope.task = {};
        $scope.isEdit = false;
        setCreatedBy();
    };

    $scope.logout = function () {

        window.location.href = "/Home/Index";
    };

});
