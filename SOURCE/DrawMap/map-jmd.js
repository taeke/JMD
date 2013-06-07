/*
 * Canvas World Map function (documentation: http://joncom.be/code/excanvas-world-map)
 *
 * Copyright (c) 2009 Jon Combe (http://joncom.be)
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

 function Map(oSettings) {
  // create variables
  var oSettings = (oSettings || {});
  oSettings.detail = (oSettings.detail || {});
  var sBGColor = (oSettings.bgcolor || "#ffffff");
  var sFGColor = (oSettings.fgcolor || "#dddddd");
  var sBorderColor = (oSettings.bordercolor || "#aaaaaa");
  var iPadding = ((oSettings.padding || 10) * 2);
  var sZoom = (oSettings.zoom || #PlaceHolderCountryNames#);
  var iOffsetX = 0;
  var iOffsetY = 0;

  // get canvas dimensions, set bgcolor
  var oCanvas = document.getElementById(oSettings.id);
  if (!oCanvas) {
    alert("Error: missing or incorrect canvas 'id'");
  }
  var iCanvasWidth = oCanvas.width;
  var iCanvasHeight = oCanvas.height;
  oCanvas.style.backgroundColor = sBGColor;

  // create drawing area
  var oCTX = (oCanvas).getContext('2d');
  oCTX.clearRect(0, 0, iCanvasWidth, iCanvasHeight);
  oCTX.lineWidth = (oSettings.borderwidth || 1);

  // calculate zoom: create variables
  var aZoom = sZoom.split(",");
  var iMinX = oMap[aZoom[0]][0][0][0];
  var iMaxX = oMap[aZoom[0]][0][0][0];
  var iMinY = oMap[aZoom[0]][0][0][1];
  var iMaxY = oMap[aZoom[0]][0][0][1];

  // calculate zoom: find map range
  for (var iCountry = 0; iCountry < aZoom.length; iCountry++) {
    for (var iPath = 0; iPath < oMap[aZoom[iCountry]].length; iPath++) {
      for (iCoord = 0; iCoord < oMap[aZoom[iCountry]][iPath].length; iCoord++) {
        iMinX = Math.min( iMinX, oMap[aZoom[iCountry]][iPath][iCoord][0] );
        iMaxX = Math.max( iMaxX, oMap[aZoom[iCountry]][iPath][iCoord][0] );
        iMinY = Math.min( iMinY, oMap[aZoom[iCountry]][iPath][iCoord][1] );
        iMaxY = Math.max( iMaxY, oMap[aZoom[iCountry]][iPath][iCoord][1] );
      }
    }
  }

  // calculate zoom ratio
  var iRatio = Math.min( ((iCanvasWidth - iPadding) / (iMaxX - iMinX)), ((iCanvasHeight - iPadding) / (iMaxY - iMinY)) );

  // calculate zoom offsets
  var iMidX = (iMinX + ((iMaxX - iMinX) / 2));
  var iMidY = (iMinY + ((iMaxY - iMinY) / 2));
  iOffsetX = ((iMidX * iRatio) - (iCanvasWidth / 2));
  iOffsetY = ((iMidY * iRatio) - (iCanvasHeight / 2));

  // draw "plain" countries
  for (var sCountry in oMap) {
    Draw(sCountry, sFGColor);
  }

  // draw "details" countries
  for (var sCountry in oSettings.detail) {
    if (oMap[sCountry]) {
      Draw(sCountry, oSettings.detail[sCountry]);
    }
  }

  // private draw function
  function Draw(sCountry, sColor) {
    oCTX.fillStyle = sColor;
    oCTX.strokeStyle = sBorderColor;
    oCTX.beginPath();

    // loop through paths
    var bIE = (navigator.userAgent.indexOf("MSIE") > -1);
    for (var iPath = 0; iPath < oMap[sCountry].length; iPath++) {
      oCTX.moveTo((oMap[sCountry][iPath][0][0] * iRatio) - iOffsetX, (oMap[sCountry][iPath][0][1] * iRatio) - iOffsetY);
      for (iCoord = 1; iCoord < oMap[sCountry][iPath].length; iCoord++) {
        oCTX.lineTo((oMap[sCountry][iPath][iCoord][0] * iRatio) - iOffsetX, (oMap[sCountry][iPath][iCoord][1] * iRatio) - iOffsetY);
      }
      oCTX.closePath();
      oCTX.fill();

      // IE, again...
      if (bIE == true) {
        oCTX.beginPath();
        oCTX.moveTo((oMap[sCountry][iPath][0][0] * iRatio) - iOffsetX, (oMap[sCountry][iPath][0][1] * iRatio) - iOffsetY);
        for (iCoord = 1; iCoord < oMap[sCountry][iPath].length; iCoord++) {
          oCTX.lineTo((oMap[sCountry][iPath][iCoord][0] * iRatio) - iOffsetX, (oMap[sCountry][iPath][iCoord][1] * iRatio) - iOffsetY);
        }
        oCTX.closePath();
      }
      oCTX.stroke();
    }
  }
}

var oMap=#PlaceHolderOMapData#