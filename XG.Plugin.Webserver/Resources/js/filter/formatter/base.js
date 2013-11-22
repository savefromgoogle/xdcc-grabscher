//
//  base.js
//  This file is part of XG - XDCC Grabscher
//  http://www.larsformella.de/lang/en/portfolio/programme-software/xg
//
//  Author:
//       Lars Formella <ich@larsformella.de>
//
//  Copyright (c) 2012 Lars Formella
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//


define(['./module'], function (filter) {
	'use strict';

	filter.filter('formatIcon', function ()
	{
		return function (icon, iconClass, overlay, overlayClass, overlayStyle)
		{
			if (icon == undefined)
			{
				return "";
			}

			iconClass = "glyphicon glyphicon-" + icon + " " + iconClass;
			overlayClass = "icon-overlay glyphicon glyphicon-" + overlay + " " + overlayClass;

			var str = "";
			if (overlay != undefined && overlay != "")
			{
				str += "<i class='" + overlayClass + "'";
				if (overlayStyle != undefined && overlayStyle != "")
				{
					str += " style='" + overlayStyle + "'";
				}
				str += "></i>";
			}
			str += "<i class='" + iconClass + "'></i>";

			return str;
		}
	});

	filter.filter('speed2Overlay', function ()
	{
		return function (speed)
		{
			var opacity = (speed / (1024 * 1500)) * 0.4;
			return (opacity > 0.4 ? 0.4 : opacity) + 0.6;
		}
	});
});
