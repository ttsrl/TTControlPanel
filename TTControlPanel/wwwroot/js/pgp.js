var query = getQuery();
var currentParameter;
var mLoadMenuCount = 0;
var mCurrentMode = -1;
var mLinearSpeedActive = -1;
var isMnemonic = false;
var mProgress = {
    count: 0,
    total: 4
};
var mReadParameter = null;
var mFirstDisplay = true;
var delayStateCheck = true;
var oReadRequest = new classRequestReadWithType();
var oReadRequestRepeat = new classRequestReadWithType();
oReadRequestRepeat.isMonitorRequest = true;
oReadRequest.addParameter(11, 84, 0, function (a) {
    mCurrentMode = a.currentValue;
    LoadMenus();
});
ctdatabase.loadData(function () {
    LoadMenus();
    oRequestQueue.stateCheck();
});

function LoadMenus() {
    mLoadMenuCount++;
    AddProgress(mProgress);
    if (mLoadMenuCount == 3) {
        if (mCurrentMode == 2 || mCurrentMode == 3) {
            oReadRequestRepeat.addParameter(1, 55, 0, function (a) {
                mLinearSpeedActive = a.currentValue;
            });
        }
        oReadRequestRepeat.addParameter(11, 84, 0, function (a) {
            mCurrentMode = a.currentValue;
        });
        $("#optionSlot0").val(mProductFamily + "." + mProductModel).attr("disabled", false);
        $("#selectSlot").val(mProductFamily + "." + mProductModel);
        ctdatabase.getPath(mProductFamily + "." + mProductModel);
        ctdatabase.populateMenuDropDown(mCurrentMode, query.menu);
        $("#selectMenu, #selectParameter").attr("disabled", false);
        refreshValue($("#selectMenu").val(), $("#selectParameter").val());
    }
}
oReadRequest.addParameter(15, 1, 0, function (a) {
    if (a.currentValue == 0) {
        $("#optionSlot1").text("1 - " + getTT("Not Fitted", "tt_59"));
    } else {
        $("#optionSlot1").text("1 - " + ctdatabase.getDeviceName(a.currentValue)).val(a.currentValue).prop("disabled", false);
    }
});
oReadRequest.addParameter(16, 1, 0, function (a) {
    if (a.currentValue == 0) {
        $("#optionSlot2").text("2 - " + getTT("Not Fitted", "tt_59"));
    } else {
        $("#optionSlot2").text("2 - " + ctdatabase.getDeviceName(a.currentValue)).val(a.currentValue).prop("disabled", false);
    }
});
oReadRequest.addParameter(17, 1, 0, function (a) {
    if (a.currentValue == 0) {
        $("#optionSlot3").text("3 - " + getTT("Not Fitted", "tt_59"));
    } else {
        $("#optionSlot3").text("3 - " + ctdatabase.getDeviceName(a.currentValue)).val(a.currentValue).prop("disabled", false);
    }
});
oReadRequest.addParameter(24, 1, 0, function (a) {
    if (a.currentValue == 0) {
        $("#optionSlot4").text("4 - " + getTT("Not Fitted", "tt_59"));
    } else {
        $("#optionSlot4").text("4 - " + ctdatabase.getDeviceName(a.currentValue)).val(a.currentValue).prop("disabled", false);
    }
});
oRequestQueue.addRequest(oReadRequest);
oRequestQueue.addRequest(oReadRequestRepeat);
$("#selectParameter").change(function () {
    refreshValue($("#selectMenu").val(), $("#selectParameter").val(), 1);
    clearFields();
    clearWarnings();
});
$("#selectMenu").change(function () {
    mCurrentMenuIndex = ctdatabase.populateParameterDropDown(mCurrentMode, $("#selectMenu").val(), $("#selectParameter").val());
    refreshValue($("#selectMenu").val(), $("#selectParameter").val(), 1);
    clearFields();
    clearWarnings();
});
$("#selectSlot").change(function () {
    ctdatabase.getPath($("#selectSlot").val());
    ctdatabase.populateMenuDropDown(mCurrentMode, query.menu);
    refreshValue($("#selectMenu").val(), $("#selectParameter").val(), 1);
    clearFields();
    clearWarnings();
});
$("#writeValueCheck").change(function () {
    if ($(this).prop("checked") == true) {
        $("#writeValueUnits").val(getTT("On", "tt_62"));
    } else {
        $("#writeValueUnits").val(getTT("Off", "tt_61"));
    }
});
$("#saveButton").click(function () {
    $(this).val(getTT("Saving...", "tt_75"));
    saveAll(1001);
});

function refreshValue(c, e, a) {
    clearTimeout(oRequestQueue.stateCheckTimeout);
    for (var b = 0; b < oRequestQueue.requestList.length; b++) {
        if (oRequestQueue.requestList[b].requestCode == 19) {
            oRequestQueue.requestList.splice(b, 1);
        }
    }
    oReadRequestRepeat.parameterList.length = 0;
    var d = new classRequestObjectInfo();
    mFirstDisplay = true;
    mReadParameter = null;
    var f = document.getElementById("selectSlot").selectedIndex;
    if (f === undefined) {
        f = 0;
    }
    $("#currentValue").text(getTT("Getting value...", "tt_52"));
    if (mCurrentMode == 2 || mCurrentMode == 3) {
        oReadRequestRepeat.addParameter(1, 55, 0, function (g) {
            mLinearSpeedActive = g.currentValue;
        });
    }
    if (c < 0) {
        f = parseInt($('#selectMenu option[value="' + c + '"]').attr("data-slot"));
        c = 0;
    }
    d.addParameter(c, e, f, function (g) {
        if (isCurrentParameter(g.menuId, g.parameterId)) {
            mReadParameter = g;
            oReadRequestRepeat.addParameter(c, e, f, function (h) {
                if (mReadParameter != null) {
                    if (isCurrentParameter(h.menuId, h.parameterId)) {
                        mReadParameter.currentValue = h.currentValue;
                        mReadParameter.decimalPlaces = h.decimalPlaces;
                        mReadParameter.dataType = h.dataType;
                        mReadParameter.unitText = h.unitText;
                        mReadParameter.accessStatus = h.accessStatus;
                        currentParameter = displayValue(mReadParameter, a);
                    }
                }
                a = 0;
            });
            oReadRequestRepeat.wakeUp();
            oRequestQueue.stateCheck("ObjectInfo Returned");
        }
    }, true);
    oRequestQueue.addRequest(d);
    oRequestQueue.stateCheck("refreshValue");
}

function getQuery() {
    var a = {};
    return a;
}

function setFormatTextBox(b) {
    var a;
    if (!isMnemonic) {
        if (b.dataType == 0) {
            a = "checkbox";
            if (b.currentValue == 1) {
                $("#writeValueCheck").prop("checked", true);
                $("#writeValueUnits").val(getTT("On", "tt_62"));
            } else {
                $("#writeValueCheck").prop("checked", false);
                $("#writeValueUnits").val(getTT("Off", "tt_61"));
            }
        } else {
            $("#writeValueUnits").val(b.unitText);
            switch (b.formatId) {
                case 1:
                case 2:
                case 8:
                    a = "textboxes";
                    break;
                case 0:
                default:
                    a = "textbox";
                    break;
            }
        }
    } else {
        a = "select";
    }
    switch (a) {
        default:
        case "select":
            $("#writeCheckboxContainer, #writeValue1, #writeValue2, #writeValue3, #writeValueUnits").hide(); $("#writeSelect").show();
            break;
        case "textbox":
            $("#writeCheckboxContainer, #writeSelect").hide(); $("#writeValue1").removeClass("write3").addClass("write1").show(); $("#writeValueUnits").show(); $("#writeValue2, #writeValue3").hide();
            break;
        case "textboxes":
            $("#writeCheckboxContainer, #writeSelect").hide(); $("#writeValue1").removeClass("write1").addClass("write3").show(); $("#writeValue2, #writeValue3, #writeValueUnits").show();
            break;
        case "checkbox":
            $("#writeValue1, #writeValue2, #writeValue3, #writeSelect").hide(); $("#writeCheckboxContainer, #writeValueUnits").show();
            break;
    }
}

function clearFields() {
    $("#writeValue1, #writeValue2, #writeValue3").val("");
}

function clearWarnings() {
    $("#warningBar, #savedMessage").hide();
}

function isCurrentParameter(a, b) {
    if ((a == $("#selectMenu").val() && b == $("#selectParameter").val()) || (parseInt($("#selectMenu").val()) < 0 && a == 0 && b == $("#selectParameter").val())) {
        return true;
    } else {
        return false;
    }
}

function displayValue(b, a) {
    if (isCurrentParameter(b.menuId, b.parameterId)) {
        AddProgress(mProgress);
        var c = ctdatabase.getMnemonic(b, mFirstDisplay);
        if (c == null) {
            isMnemonic = false;
            c = ctdatabase.setFormatValue(b);
        } else {
            isMnemonic = true;
        }
        b.unitText = ctdatabase.setModeDependentUnits(mCurrentMode, mLinearSpeedActive, b.unitText);
        if (mFirstDisplay) {
            if (b.writable) {
                setFormatTextBox(b);
                $("#writeReadOnly").hide();
                $("#writeForm").show();
            } else {
                $("#writeForm").hide();
                $("#writeReadOnly").show();
            }
            if (a == 1 || isMnemonic) {
                switch (b.formatId) {
                    case 1:
                    case 2:
                        $("#writeValue1").val(c.substr(0, 2));
                        $("#writeValue2").val(c.substr(3, 2));
                        $("#writeValue3").val(c.substr(6, 2));
                        break;
                    case 8:
                        $("#writeValue1").val(c.substr(0, 1));
                        $("#writeValue2").val(c.substr(2, 2));
                        $("#writeValue3").val(c.substr(5, 3));
                        break;
                    default:
                        $("#writeValue1").val(c);
                        break;
                }
            }
            mFirstDisplay = false;
        }
        if (b.unitText != "") {
            c = c + " " + b.unitText;
        }
        $("#currentValue").text(c);
        return b;
    }
}

function resetDrive() {
    $("#resetButton").val(getTT("Resetting...", "tt_76"));
    var b = new classRequestWrite();
    var a = new classWriteParameter(10, 38, 0, 100, 2, 0);
    b.successCallBack = function (c) {
        if (c.accessStatus != 0) {
            alert(getTT("Action was unsuccessful.", "tt_94"));
        }
        $("#resetButton").val(getTT("Reset Drive", "tt_69"));
    };
    b.parameterList.push(a);
    oRequestQueue.addRequest(b);
    oRequestQueue.stateCheck();
}

function saveAll(c) {
    $("#saveButton").val(getTT("Saving...", "tt_75"));
    var b = new classRequestWrite();
    var a = new classWriteParameter(0, 0, 0, c, 4, 0);
    b.successCallBack = function (d) {
        if (d.accessStatus != 0) {
            alert(getTT("Action was unsuccessful.", "tt_94"));
        } else {
            resetDrive();
        }
        $("#saveButton").val(getTT("Saved", "tt_74")).delay(2000).val(getTT("Save All", "tt_73"));
    };
    b.parameterList.push(a);
    oRequestQueue.addRequest(b);
    oRequestQueue.stateCheck();
}

function writeValue() {
    mFirstDisplay = true;
    $("#newValueButton").val(getTT("Submitting", "tt_77"));
    $("#warningBar").hide().text(ctdatabase.getErrorMessage(9));
    var b = $("#writeValue1").val();
    if (isMnemonic) {
        b = parseInt($("#writeSelect").val());
    }
    var c = $("#writeValue2").val();
    var d = $("#writeValue3").val();
    if (currentParameter !== undefined) {
        var g = currentParameter;
        if (b === "" && g.dataType != 0) {
            $("#warningBar").text(getTT("Error, please enter a value.", "tt_27")).show();
        } else {
            var a = 0;
            var h = 10;
            if (g.dataType == 0) {
                if (writeForm.writeValueCheck.checked == true) {
                    g.currentValue = 1;
                } else {
                    g.currentValue = 0;
                }
            } else {
                h = ctdatabase.isValidInput(g, b, c, d);
            }
            if (h == 10 && g.formatId == 0 && (g.dataType > 0 && g.dataType < 22)) {
                if (b < (g.min / Math.pow(10, g.decimalPlaces)) || b > (g.max / Math.pow(10, g.decimalPlaces))) {
                    h = 246;
                    a = g.decimalPlaces;
                }
            }
            if (h == 10) {
                if (g.dataType != 0) {
                    g = ctdatabase.setWriteFormat(g, b, c, d);
                }
                var f = new classRequestWrite();
                var e = new classWriteParameter(g.menuId, g.parameterId, g.slotId, g.currentValue, g.dataType, g.decimalPlaces);
                f.successCallBack = function (i) {
                    accessStatus = i.accessStatus;
                    if (accessStatus != 0) {
                        $("#warningBar").text(ctdatabase.getErrorMessage(accessStatus, g, a)).show();
                    } else {
                        refreshValue($("#selectMenu").val(), $("#selectParameter").val(), 1);
                        clearWarnings();
                    }
                    $("#newValueButton").val(getTT("Submit", "tt_85"));
                };
                f.parameterList.push(e);
                oRequestQueue.addRequest(f);
                oRequestQueue.stateCheck();
            } else {
                $("#warningBar").text(ctdatabase.getErrorMessage(h, currentParameter, a, currentParameter.dataType)).show();
                $("#newValueButton").val(getTT("Submit", "tt_85"));
            }
        }
    }
}