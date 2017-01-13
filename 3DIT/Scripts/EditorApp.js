(function (angular) {
	app = angular.module("EditorApp", ["ngMaterial", "ngSanitize"])
		.controller("MainController", MainController)
		.service("SongService", SongService)
		.service("FileValidator", FileValidator)
		.service("SearchService", SearchService)
		.directive("uploadSongInput", UploadSongInput)
		.directive("uploadImageInput", UploadImageInput);

	FileValidator.$inject = [];
	function FileValidator() {
		this.validate = function (file, type) {
			var ext = file.name.split('.').pop();
			var fileType = getType(ext);
			if (fileType != type) {
				return { Status: false, Message: "Formato inválido" };
			}

			var validator = getValidator(ext);

			if (file.size > validator.maxSize) {
				return { Status: false, Message: "Tamanho maior que o permitido(" + parseInt(validator.maxSize / 1048576) + "MB)" };
			}

			return { Status: true };
		};

		var validators = [{
			for: "Audio",
			maxSize: 20971520,
			extensions: ["mp3"]
		}, {
			for: "Image",
			maxSize: 5242880,
			extensions: ["png", "jpg"]
		}];

		function getValidator(ext) {
			return validators.find(function (a) {
				return !!a.extensions.find(function (b) {
					return b.toUpperCase() == ext.toUpperCase();
				});
			});
		}

		function getType(ext) {
			switch (ext) {
				case "mp3":
					return "Audio";
				case "png":
				case "jpg":
					return "Image";
				default:
					return "Other";
			}
		}
	};

	SongService.$inject = ["$http", "$rootScope", "FileValidator"];
	function SongService($http, $rootScope, FileValidator) {
		this.reset = function (obj) {
			if (obj) {
				this.Title = obj.Title;
				this.Album = obj.Album;
				this.AlbumDiscLenght = obj.AlbumDiscLenght;
				this.AlbumTrackLenght = obj.AlbumTrackLenght;
				this.Artist = obj.Artist;
				this.Composer = obj.Composer;
				this.DiscNumber = obj.DiscNumber;
				this.Genre = obj.Genre;
				this.TrackNumber = obj.TrackNumber;
				this.Year = obj.Year;
				this.AlbumArt = obj.AlbumArt;
			} else {
				this.Title = null;
				this.Album = null;
				this.AlbumDiscLenght = null;
				this.AlbumTrackLenght = null;
				this.Artist = null;
				this.Composer = null;
				this.DiscNumber = null;
				this.Genre = null;
				this.TrackNumber = null;
				this.Year = null;
				this.AlbumArt = null;
			}

		}

		this.getSongFromURL = function (url) {
			var $this = this;
			return $http.post("/Home/GetSongFromURL", {
				url: url
			}).then(function (r) {
				if (r.data.Status) {
					$this.reset(r.data.Objects[0]);
				}

				return r.data;
			});
		};

		this.uploadSong = function (file) {
			var v = FileValidator.validate(file, "Audio");
			if (v.Status) {
				var $this = this;
				var fd = new FormData();
				fd.append('file', file);

				return $.ajax({
					xhr: function () {
						var xhr = new XMLHttpRequest();

						xhr.upload.addEventListener("progress", function (evt) {
							if (evt.lengthComputable) {
								var completed = parseInt((evt.loaded / evt.total) * 100);
								$this.qp = completed;
								$rootScope.$apply();
							}
						}, false);

						return xhr;
					},
					url: "/Home/UploadSong",
					type: "POST",
					data: fd,
					processData: false,
					contentType: false,
					success: function (r) {
						if (r.Status) {
							$this.reset(r.Objects[0]);
						}

						return r;
					}
				});
			} else {

			}
		};

		this.save = function (download) {
			var $this = this;
			return $http.post("/Home/Save", {
				song: $this
			}).then(function (r) {
				if (download) {
					var hiddenElement = document.createElement('a');
					hiddenElement.href = window.location.href + "Home/DownloadFile";
					hiddenElement.click();
				}

				$this.reset();
				return r.data;
			});
		};

		this.reset();
	};

	MainController.$inject = ["$scope", "$http", "$mdDialog", "SongService", "FileValidator"];
	function MainController($scope, $http, $mdDialog, SongService, FileValidator) {

		$scope.song = SongService;

		$scope.saveSong = function (ev) {
			SongService.save(true).then(function (r) {
				$scope.fileReady = false;
			});
		};

		$scope.next = function () {
			$scope.q = true;
			SongService.qp = 0;
			if ($scope.fromUrl) {
				SongService.getSongFromURL($scope.songUrl).then(function (r) {
					if (r.Status) {
						$scope.q = false;
						$scope.fileReady = true;
					} else {
						var alert = $mdDialog.alert({
							title: 'Erro',
							textContent: r.Message,
							ok: 'Ok'
						});

						$mdDialog.show(alert);
						$scope.q = false;
					}
				});
			} else {
				var v = FileValidator.validate($scope.file, "Audio");
				if (v.Status) {
					SongService.uploadSong($scope.file).then(function (r) {
						if (r.Status) {
							$scope.q = false;
							$scope.fileReady = true;
						} else {
							showAlert("Erro", r.Message);
							$scope.q = false;
						}
						$scope.$apply();
					});
				} else {
					showAlert("Erro", v.Message);
					$scope.q = false;
				}

			}


		};

		function showAlert(title, message) {
			var alert = $mdDialog.alert({
				title: title,
				textContent: message,
				ok: 'Ok'
			});

			$mdDialog.show(alert);
		}

		$scope.searchInfo = function () {
			$mdDialog.show({
				controller: SearchController,
				templateUrl: 'Home/SearchSongPopup',
				parent: angular.element(document.body),
				clickOutsideToClose: true,
				locals: {
					title: SongService.Title,
					artist: SongService.Artist
				}
			});
		};
	};

	SearchController.$inject = ["$scope", "$http", "$mdDialog", "title", "artist", "SongService", "SearchService"];
	function SearchController($scope, $http, $mdDialog, title, artist, SongService, SearchService) {
		$scope.title = title;
		$scope.artist = artist;

		$scope.search = function () {
			$scope.q = true;
			SearchService.search($scope.title, $scope.artist).then(function (list) {
				$scope.list = list;
				$scope.q = false;
			});
		};

		$scope.select = function (item) {
			$scope.q = true;

			SearchService.select(item).then(function (song) {
				$mdDialog.hide();
				$scope.q = false;
			});
		};
	};
	
	SearchService.$inject = ["$http", "SongService"];
	function SearchService($http, SongService) {

		this.search = function (title, artist) {
			return $http.post("Home/SearchSong", {
				title: title,
				artist: artist
			}).then(function (r) {
				return r.data;
			});
		};

		this.select = function (item) {
			return $http.post("Home/SelectSearchSong", {
				item: item
			}).then(function (r) {
				SongService.reset(r.data);
				return r.data;
			});

		};
	};

	UploadSongInput.$inject = ["$rootScope"];
	function UploadSongInput($rootScope) {
		return {
			restrict: "A",
			link: function ($scope, $element, attr) {
				$element.bind("change", function () {
					var file = $element[0].files[0];
					if (file) {
						$rootScope.file = file;
						$rootScope.$apply();
					}
				});
			}
		}
	};

	UploadImageInput.$inject = ["$mdDialog", "$http", "SongService", "FileValidator"];
	function UploadImageInput($mdDialog, $http, SongService, FileValidator) {
		return {
			restrict: "A",
			link: function ($scope, $element, attr) {
				$element.bind("change", function () {
					var file = $element[0].files[0];
					var v = FileValidator.validate(file, "Image");
					if (v.Status) {
						if (file) {
							var fd = new FormData();
							fd.append('file', file);
							$http.post("/Home/ChangeAlbumArt", fd, {
								transformRequest: angular.identity,
								headers: { 'Content-Type': undefined }
							}).then(function (r) {
								if (r.data.Status) {
									SongService.AlbumArt = r.data.Objects[0];
								} else {
									var alert = $mdDialog.alert({
										title: "Erro",
										textContent: r.data.Message,
										ok: 'Ok'
									});

									$mdDialog.show(alert);
								}
							});
						}
					} else {
						var alert = $mdDialog.alert({
							title: "Erro",
							textContent: v.Message,
							ok: 'Ok'
						});

						$mdDialog.show(alert);
					}
				});
			}
		}
	};

})(angular);