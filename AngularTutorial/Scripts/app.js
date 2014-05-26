var TodoApp = angular.module("TodoApp", ["ngResource", "ngRoute"]).config(function ($routeProvider) {
    $routeProvider.
        when('/', { controller: ListCtrl, templateUrl: 'list.html' }).
        when('/new', { controller: CreateCtrl, templateUrl: 'details.html' }).
        when('/edit/:editId', {controller: EditCtrl, templateUrl: 'details.html'}).
        otherwise({ redirectTo: '/' });
});

TodoApp.factory('Todo', function ($resource) {
    return $resource('api/Todo/:id', { id: '@id' }, { update: { method: 'PUT' } });
});

var CreateCtrl = function ($scope, $location, Todo) {
    $scope.action = "Add";
    $scope.save = function () {
        Todo.save($scope.item, function () {
            $location.path('/');
        });
    }
};

var EditCtrl = function ($scope, $location, $routeParams, Todo) {
    var Id = $routeParams.editId;
    $scope.item = Todo.get({ id: Id });
    $scope.action = "Edit";

    $scope.save = function () {
        Todo.update({ id: Id }, $scope.item, function () {
            $location.path('/');
        });
    }
}

var ListCtrl = function ($scope, $location, Todo) {
    $scope.search = function () {
        Todo.query({
            q: $scope.query,
            sort: $scope.sort_order,
            desc: $scope.is_descending,
            limit: $scope.limit,
            offset: $scope.offset
        }, function (data) {
            $scope.todos = $scope.todos.concat(data);
            if (data.length < 20) {
                $scope.has_more = false;
            }
        });
    }



    $scope.show_more = function () {
        $scope.offset += $scope.limit;
        $scope.search();
    }

    $scope.sort = function (column) {
        if (column == $scope.sort_order) {
            $scope.is_descending = !$scope.is_descending;
        } else {
            $scope.sort_order = column;
            $scope.is_descending = false;
        }
        $scope.reset();
    };

    $scope.reset = function () {
        $scope.limit = 20;
        $scope.offset = 0;
        $scope.todos = [];
        $scope.has_more = true;
        $scope.search();
    }

    $scope.delete = function () {
        var id = this.todo.Id;
        Todo.delete({id: id}, function() {
            $('#todo_' + id).fadeOut();
        });
    }

    $scope.edit = function () {

    }

    $scope.sort_order = "Priority";
    $scope.is_descending = false;

    $scope.reset();

    
};