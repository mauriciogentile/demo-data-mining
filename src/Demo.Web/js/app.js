var app = angular.module("Zupla", ["ng"]);

app.controller("newPropertyCtrl", function($scope) {
    $scope.bedrooms = 1;
    $scope.bathrooms = 1;
    $scope.operationType = "Sale";
    $scope.propertyType = "Apartment";
    $scope.distanceToStation = 100;
    $scope.age = 10;
    $scope.zoneReputation = "Good";
    $scope.hasParking = false;
    $scope.withFurniture = false;
    $scope.hasBackyard = false;
    $scope.price = 0;
});