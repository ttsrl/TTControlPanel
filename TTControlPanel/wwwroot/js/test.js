if (!window.console) {
    window.console = {};
}
if (!window.console.log) {
    window.console.log = function () { };
}
var DEBUG = true;
var mLanguage = null;
var mProductFamily = null;
var mProductModel = null;
var mLastTick = 0;
var REQUEST_READ_WITH_TYPE = 17;
var REQUEST_WRITE = 18;
var REQUEST_OBJECT_INFO = 19;
var REQUEST_INTERVAL = 1000;

function getIntBytes(d, b) {
    var a = [];
    var c = b;
    do {
        a[--c] = d & (255);
        d = d >> 8;
    } while (c);
    return a;
}

function classRequestQueue() {
    this.requestList = new Array();
    this.maxRequestsAllowed = 100;
    this.ajaxTimeout = REQUEST_INTERVAL;
    this.cycleMilliseconds = REQUEST_INTERVAL;
    this.queueState = "Initialised";
    this.stateCheckTimeout;
    this.runAgainImmediately = false;
}
classRequestQueue.prototype.addRequest = function (a) {
    if (this.requestList.length >= this.maxRequestsAllowed) {
        return false;
    } else {
        a.queueReference = this;
        this.requestList.push(a);
        return true;
    }
};
classRequestQueue.prototype.stateCheck = function (a) {
    a = typeof a !== "undefined" ? a : "Unkonwn";
    mLastTick = new Date();
    var c = this;
    c.runAgainImmediately = false;
    if (this.queueState != "Processing") {
        if (this.requestList.length > 0) {
            var e = 0;
            var d = this.requestList.length;
            while (e < d) {
                if (this.queueState != "Processing") {
                    if (this.requestList[0].executionState == "Awake") {
                        this.queueState = "Processing";
                        var b = this.requestList[0];
                        b.executionState = "Processing";
                        $.ajax({
                            url: b.urlString,
                            cache: false,
                            async: true,
                            contentType: "text/plain; charset=utf-8",
                            type: "POST",
                            data: "ecmp=" + b.buildMessage(),
                            dataType: "text",
                            success: b.handleSuccess,
                            error: b.handleError,
                            callingReference: b,
                            timeout: this.ajaxTimeout
                        });
                        this.requestList.push(this.requestList.shift());
                        this.queueState = "Pending";
                    } else {
                        if (this.requestList[0].executionState == "Complete") {
                            this.requestList.shift();
                        } else {
                            this.requestList.push(this.requestList.shift());
                        }
                    }
                    e++;
                } else {
                    c.runAgainImmediately = true;
                    break;
                }
            }
        }
    } else {
        c.runAgainImmediately = true;
    }
    if (typeof c.stateCheckTimeout !== "undefined") {
        clearTimeout(c.stateCheckTimeout);
    }
    c.stateCheckTimeout = setTimeout(function () {
        c.stateCheck("Ticker");
    }, c.cycleMilliseconds);
};

function classRequest() {
    this.outboundDestinationAddressingScheme = 1;
    this.outboundDestinationAddress = 1;
    this.outboundTransactionId = 1;
    this.messageString = "";
    this.inboundDestinationAddressingScheme = 1;
    this.inboundDestinationAddressing = 1;
    this.inboundTransactionId = 1;
    this.requestStatus = 0;
    this.executionState = "Awake";
    this.submissionInterval = REQUEST_INTERVAL;
    this.isMonitorRequest = false;
    this.urlString = "/ecmp";
    this.queueReference = 0;
    this.maxResponseSize = 1024;
    this.requestCode = 0;
    this.optionCode = 0;
    this.byteArray = "";
    this.successCallBack = null;
    this.unitStrings = ["", "", "mm", "m", "UU", "revs", "°", "", "modeDependent8", "mm/s", "UU/ms", "rpm", "Hz", "kHz", "MHz", "modeDependent15", "modeDependent16", "s/m/s", "UU/mm/s", "s/1000rpm", "s/100Hz", "s", "s", "s²/1000mm/s", "s²/UU/ms", "s²/1000rpm", "s²/100Hz", "modeDependent27", "modeDependent28", "Msg/s", "Hours", "Mins", "s", "ms", "μs", "ns", "V", "A", "Ohms", "mH", "kW", "kVAr", "MWh", "kWh", "°C", "/°C", "kgm²", "Nm", "Nm/A", "V/1000rpm", "Bits", "Bytes", "kB", "MB", "Bit/s", "Baud", "kBaud", "MBaud", "PolePairs", "%", "V/ms"];
}
classRequest.prototype.decimalToHex = function (a, c) {
    if (a < 0) {
        a = 255 + a + 1;
    }
    var b = Number(a).toString(16);
    c = typeof (c) === "undefined" || c === null ? c = 2 : c;
    while (b.length < c) {
        b = "0" + b;
    }
    return "%" + b;
};
classRequest.prototype.buildMessage = function () {
    this.messageString = "";
    this.messageString += this.decimalToHex(this.outboundDestinationAddressingScheme);
    if (this.outboundDestinationAddressingScheme > 1) {
        this.messageString += this.decimalToHex(this.outboundDestinationAddress);
    }
    this.messageString += this.decimalToHex(this.inboundDestinationAddressingScheme);
    if (this.inboundDestinationAddressingScheme > 1) {
        this.messageString += this.decimalToHex(this.inboundDestinationAddressing);
    }
    this.messageString += this.decimalToHex(this.outboundTransactionId);
    this.messageString += this.multiByteToHex(this.maxResponseSize, 2);
    this.messageString += this.decimalToHex(this.requestCode);
    this.messageString += this.decimalToHex(0);
};
classRequest.prototype.handleSuccess = function (a, c, b) {
    this.byteArray = a.split("%");
    this.byteArray.shift();
    this.outboundDestinationAddressingScheme = this.byteArray.shift();
    if (this.outboundDestinationAddressingScheme > 1) {
        this.outboundDestinationAddressing = this.byteArray.shift();
    }
    this.inboundDestinationAddressingScheme = this.byteArray.shift();
    if (this.inboundDestinationAddressingScheme > 1) {
        this.inboundDestinationAddressing = this.byteArray.shift();
    }
    this.inboundTransactionId = this.byteArray.shift();
    this.requestStatus = parseInt(this.byteArray.shift(), 16) % 256;
    if (this.requestStatus != 0) { }
};
classRequest.prototype.handleError = function (b, c, a) {
    if (b.status == 423) {
        $("#notificationArea").html(getTT("Access denied, session has timed out.", "tt_103")).show();
        this.callingReference.terminate();
    } else {
        if (this.callingReference.isMonitorRequest == false) {
            this.callingReference.persist();
        } else {
            this.callingReference.terminate();
        }
    }
};
classRequest.prototype.multiByteToHex = function (e, f) {
    switch (f) {
        case 2:
            var h = Math.floor(e / 256);
            var g = e % 256;
            return this.decimalToHex(h) + this.decimalToHex(g);
            break;
        case 4:
            var d = Math.floor(e / 16777216);
            var c = Math.floor((e - (d * 16777216)) / 65536);
            var b = Math.floor((e - (d * 16777216) - (c * 65536)) / 256);
            var a = e % 256;
            return this.decimalToHex(d) + this.decimalToHex(c) + this.decimalToHex(b) + this.decimalToHex(a);
            break;
    }
};
classRequest.prototype.bytesToString = function (b) {
    var c = "";
    for (var a = 0; a < b.length; a++) {
        c += String.fromCharCode(parseInt(b[a], 16));
    }
    return c;
};
classRequest.prototype.terminate = function () {
    if (this.isMonitorRequest == false) {
        this.executionState = "Complete";
    } else {
        this.executionState = "Awake";
    }
    var a = this.queueReference;
    a.queueState = "Pending";
    if (a.runAgainImmediately) {
        a.stateCheck("runAgainImmediately");
    }
};
classRequest.prototype.persist = function () {
    this.executionState = "Dormant";
    var a = this.submissionInterval;
    var b = this;
    setTimeout(function () {
        b.wakeUp();
    }, (a / 2));
    this.queueReference.queueState = "Pending";
};
classRequest.prototype.wakeUp = function () {
    this.executionState = "Awake";
};
classRequest.prototype.getDataValue = function (s) {
    var a = new Array();
    var t = "";
    switch (s) {
        case 0:
        case 1:
        case 2:
            t = parseInt(this.byteArray.shift(), 16);
            break;
        case 3:
        case 4:
            var c = parseInt(this.byteArray.shift(), 16) * 256;
            var b = parseInt(this.byteArray.shift(), 16);
            t = c + b;
            break;
        case 5:
        case 6:
            var k = parseInt(this.byteArray.shift(), 16) * 16777216;
            var j = parseInt(this.byteArray.shift(), 16) * 65536;
            var c = parseInt(this.byteArray.shift(), 16) * 256;
            var b = parseInt(this.byteArray.shift(), 16);
            t = k + j + c + b;
            break;
        case 7:
        case 8:
            var o = parseInt(this.byteArray.shift(), 16) * 7.205759403792794e+16;
            var n = parseInt(this.byteArray.shift(), 16) * 281474976710656;
            var m = parseInt(this.byteArray.shift(), 16) * 1099511627776;
            var l = parseInt(this.byteArray.shift(), 16) * 4294967296;
            var k = parseInt(this.byteArray.shift(), 16) * 16777216;
            var j = parseInt(this.byteArray.shift(), 16) * 65536;
            var c = parseInt(this.byteArray.shift(), 16) * 256;
            var b = parseInt(this.byteArray.shift(), 16);
            t = o + n + m + l + k + j + c + b;
            break;
        case 9:
        case 10:
            var i = parseInt(this.byteArray.shift(), 16) * 1.329227995784916e+36;
            var h = parseInt(this.byteArray.shift(), 16) * 5.192296858534828e+33;
            var g = parseInt(this.byteArray.shift(), 16) * 2.028240960365167e+31;
            var f = parseInt(this.byteArray.shift(), 16) * 7.922816251426434e+28;
            var e = parseInt(this.byteArray.shift(), 16) * 3.094850098213451e+26;
            var d = parseInt(this.byteArray.shift(), 16) * 1.208925819614629e+24;
            var q = parseInt(this.byteArray.shift(), 16) * 4.722366482869645e+21;
            var p = parseInt(this.byteArray.shift(), 16) * 1.844674407370955e+19;
            var o = parseInt(this.byteArray.shift(), 16) * 7.205759403792794e+16;
            var n = parseInt(this.byteArray.shift(), 16) * 281474976710656;
            var m = parseInt(this.byteArray.shift(), 16) * 1099511627776;
            var l = parseInt(this.byteArray.shift(), 16) * 4294967296;
            var k = parseInt(this.byteArray.shift(), 16) * 16777216;
            var j = parseInt(this.byteArray.shift(), 16) * 65536;
            var c = parseInt(this.byteArray.shift(), 16) * 256;
            var b = parseInt(this.byteArray.shift(), 16);
            t = i + h + g + f + e + d + q + p + o + n + m + l + k + j + c + b;
            break;
        case 30:
            a.push(this.byteArray.shift());
            a.push(this.byteArray.shift());
            t = this.utfDecode(a);
            break;
        case 31:
            var r = (parseInt(this.byteArray.shift(), 16) * 256) + parseInt(this.byteArray.shift(), 16);
            for (var u = 0; u < r; u++) {
                a.push(this.byteArray.shift());
            }
            t = this.utfDecode(a);
            break;
        case 128:
            for (var u = 0; u < 128; u++) {
                a.push(this.byteArray.shift());
            }
            t = this.utfDecode(a);
            break;
        default:
            break;
    }
    return t;
};
classRequest.prototype.utfDecode = function (b) {
    var e = "";
    var d = 0;
    var a = c1 = c2 = 0;
    while (d < b.length) {
        a = parseInt(b[d], 16);
        if (a < 128) {
            e += String.fromCharCode(a);
            d++;
        } else {
            if ((a > 191) && (a < 224)) {
                c2 = parseInt(b[d + 1], 16);
                e += String.fromCharCode(((a & 31) << 6) | (c2 & 63));
                d += 2;
            } else {
                c2 = parseInt(b[d + 1], 16);
                c3 = parseInt(b[d + 2], 16);
                e += String.fromCharCode(((a & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                d += 3;
            }
        }
    }
    return e;
};
classRequest.prototype.utfEncode = function (a) {
    return unescape(encodeURIComponent(a));
};

function classRequestReadWithType() {
    classRequest.call(this);
    this.addressingScheme = 1;
    this.requestCode = REQUEST_READ_WITH_TYPE;
    this.parameterList = new Array();
    this.returnedParameters = new Array();
    this.returnCount = 0;
}
classRequestReadWithType.prototype = new classRequest();
classRequestReadWithType.prototype.buildMessage = function () {
    classRequest.prototype.buildMessage.call(this);
    this.messageString += this.decimalToHex(this.addressingScheme);
    this.messageString += this.getReadParameterList();
    return this.messageString;
};
classRequestReadWithType.prototype.getReadParameterList = function () {
    var a = this.decimalToHex(this.parameterList.length);
    for (var b = 0; b < this.parameterList.length; b++) {
        switch (this.addressingScheme) {
            case 0:
                a += this.multiByteToHex(this.parameterList[b].menuId, 2) + this.multiByteToHex(this.parameterList[b].parameterId, 2);
                break;
            case 1:
                a += this.decimalToHex(this.parameterList[b].slotId);
                a += this.multiByteToHex(this.parameterList[b].menuId, 2) + this.multiByteToHex(this.parameterList[b].parameterId, 2);
                break;
        }
    }
    return a;
};
classRequestReadWithType.prototype.addParameter = function (b, d, e, a) {
    var c = new classReadParameter(b, d, e);
    c.callBack = a;
    this.parameterList.push(c);
};
classRequestReadWithType.prototype.handleSuccess = function (a, d, b) {
    classRequest.prototype.handleSuccess.call(this.callingReference, a, d, b);
    switch (this.callingReference.requestStatus) {
        case 0:
            this.callingReference.byteArray.shift();
            var c = parseInt(this.callingReference.byteArray.shift(), 16);
            this.callingReference.byteArray.shift();
            switch (c) {
                case 145:
                    this.callingReference.returnCount = parseInt(this.callingReference.byteArray.shift(), 16);
                    this.callingReference.returnedParameters = this.callingReference.disectParameters();
                    if (this.callingReference.parameterList.length != this.callingReference.returnedParameters.length) { }
                    for (var e = 0; e < this.callingReference.returnedParameters.length; e++) {
                        if (e < this.callingReference.parameterList.length) {
                            this.callingReference.parameterList[e].dataType = this.callingReference.returnedParameters[e].dataType;
                            this.callingReference.parameterList[e].decimalPlaces = this.callingReference.returnedParameters[e].decimalPlaces;
                            this.callingReference.parameterList[e].unitText = this.callingReference.returnedParameters[e].unitText;
                            this.callingReference.parameterList[e].updateValue(this.callingReference.returnedParameters[e].currentValue);
                            this.callingReference.parameterList[e].accessStatus = this.callingReference.returnedParameters[e].accessStatus;
                            if (this.callingReference.parameterList[e].callBack != null) {
                                this.callingReference.parameterList[e].callBack(this.callingReference.parameterList[e]);
                            }
                        }
                    }
                    if (this.callingReference.successCallBack != null) {
                        this.callingReference.successCallBack(this.callingReference);
                    }
                    break;
            }
            break;
        default:
            break;
    }
    this.callingReference.terminate();
};
classRequestReadWithType.prototype.disectParameters = function () {
    var a = new Array();
    var f = new Array();
    var e = 0;
    while (this.byteArray.length > 0 && e == 0) {
        var e = parseInt(this.byteArray.shift(), 16) % 256;
        if (e == 0) {
            var b = parseInt(this.byteArray.shift(), 16);
            var c = this.getDataValue(b);
            var d = parseInt(this.byteArray.shift(), 16);
            var h = parseInt(this.byteArray.shift(), 16);
            var j = "";
            if (h == 255) {
                var i = (parseInt(this.byteArray.shift(), 16) * 256) + parseInt(this.byteArray.shift(), 16);
                for (var k = 0; k < i; k++) {
                    a.push(this.byteArray.shift());
                }
                j = this.bytesToString(a);
            } else {
                if (h in this.unitStrings) {
                    j = this.unitStrings[h];
                } else {
                    j = "";
                }
            }
            var g = new classReadParameter(0, 0, "");
            g.currentValue = c;
            g.dataType = b;
            g.decimalPlaces = d;
            g.unitText = j;
            g.accessStatus = e;
            f.push(g);
        } else { }
    }
    return f;
};

function classRequestObjectInfo() {
    classRequest.call(this);
    this.addressingScheme = 1;
    this.requestCode = REQUEST_OBJECT_INFO;
    this.parameterList = new Array();
    this.returnedObjectInfo = new Array();
    this.returnCount = 0;
}
classRequestObjectInfo.prototype = new classRequest();
classRequestObjectInfo.prototype.buildMessage = function () {
    classRequest.prototype.buildMessage.call(this);
    this.messageString += this.decimalToHex(this.addressingScheme);
    this.messageString += this.getReadParameterList();
    return this.messageString;
};
classRequestObjectInfo.prototype.getReadParameterList = function () {
    var b = this.decimalToHex(this.parameterList.length * 3);
    for (var c = 0; c < this.parameterList.length; c++) {
        var a = "";
        switch (this.addressingScheme) {
            case 0:
                a += this.multiByteToHex(this.parameterList[c].menuId, 2) + this.multiByteToHex(this.parameterList[c].parameterId, 2);
                break;
            case 1:
                a += this.decimalToHex(this.parameterList[c].slotId);
                a += this.multiByteToHex(this.parameterList[c].menuId, 2) + this.multiByteToHex(this.parameterList[c].parameterId, 2);
                break;
        }
        b += this.decimalToHex(3) + a;
        b += this.decimalToHex(4) + a;
        b += this.decimalToHex(5) + a;
    }
    return b;
};
classRequestObjectInfo.prototype.addParameter = function (b, d, e, a) {
    var c = new classReadParameter(b, d, e);
    c.callBack = a;
    this.parameterList.push(c);
};
classRequestObjectInfo.prototype.handleSuccess = function (a, d, b) {
    classRequest.prototype.handleSuccess.call(this.callingReference, a, d, b);
    switch (this.callingReference.requestStatus) {
        case 0:
            this.callingReference.byteArray.shift();
            var c = parseInt(this.callingReference.byteArray.shift(), 16);
            this.callingReference.byteArray.shift();
            if (c == 147) {
                this.callingReference.returnCount = parseInt(this.callingReference.byteArray.shift(), 16);
                this.callingReference.returnedObjectInfo = this.callingReference.disectObjectInfo();
                if (this.callingReference.parameterList.length != this.callingReference.returnedObjectInfo.length) { }
                for (var e = 0; e < this.callingReference.returnedObjectInfo.length; e++) {
                    if (e < this.callingReference.parameterList.length) {
                        this.callingReference.parameterList[e].accessStatus = this.callingReference.returnedObjectInfo[e].accessStatus;
                        this.callingReference.parameterList[e].writable = this.callingReference.returnedObjectInfo[e].writable;
                        this.callingReference.parameterList[e].readNotAllowed = this.callingReference.returnedObjectInfo[e].readNotAllowed;
                        this.callingReference.parameterList[e].string = this.callingReference.returnedObjectInfo[e].string;
                        this.callingReference.parameterList[e].formatId = this.callingReference.returnedObjectInfo[e].formatId;
                        this.callingReference.parameterList[e].min = this.callingReference.returnedObjectInfo[e].min;
                        this.callingReference.parameterList[e].minDataType = this.callingReference.returnedObjectInfo[e].minDataType;
                        this.callingReference.parameterList[e].max = this.callingReference.returnedObjectInfo[e].max;
                        this.callingReference.parameterList[e].maxDataType = this.callingReference.returnedObjectInfo[e].maxDataType;
                        if (this.callingReference.parameterList[e].callBack != null) {
                            this.callingReference.parameterList[e].callBack(this.callingReference.parameterList[e]);
                        }
                    }
                }
                if (this.callingReference.successCallBack != null) {
                    this.callingReference.successCallBack(this.callingReference);
                }
            } else { }
            break;
        default:
            break;
    }
    this.callingReference.terminate();
};
classRequestObjectInfo.prototype.disectObjectInfo = function () {
    var p = new Array();
    var a = 0;
    var n = -1;
    while (this.byteArray.length > 0 && a == 0) {
        var q = new classReadParameter(0, 0, "");
        var a = parseInt(this.byteArray.shift(), 16) % 256;
        if (a == 0) {
            infoType = parseInt(this.byteArray.shift(), 16);
            var f = parseInt(this.byteArray.shift(), 16);
            var e = parseInt(this.byteArray.shift(), 16);
            var d = parseInt(this.byteArray.shift(), 16);
            var c = parseInt(this.byteArray.shift(), 16);
            var s = (c >> 1) & 1;
            var o = (c >> 2) & 1;
            var r = (d >> 5) & 1;
            var j = (e >> 1) & 15;
            var q = new classReadParameter(0, 0, "");
            q.writable = s;
            q.readNotAllowed = o;
            q.string = r;
            q.formatId = j;
            q.accessStatus = a;
            a = parseInt(this.byteArray.shift(), 16) % 256;
            infoType = parseInt(this.byteArray.shift(), 16);
            var l = parseInt(this.byteArray.shift(), 16);
            var m = 0;
            var g;
            var h = false;
            switch (l) {
                case 0:
                case 1:
                case 2:
                    g = 0;
                    break;
                case 3:
                case 4:
                    g = 1;
                    break;
                case 5:
                case 6:
                    g = 3;
                    break;
                case 7:
                case 8:
                    g = 7;
                    break;
                case 9:
                case 10:
                    g = 15;
                    break;
                case 20:
                    alert("WARNING: Data type Id not handled (single precision floating point)");
                    break;
                case 21:
                    alert("WARNING: Data type Id not handled (double precision floating point)");
                    break;
                default:
                    break;
            }
            if (l <= 10) {
                for (var k = g; k >= 0; k--) {
                    m = m + (parseInt(this.byteArray.shift(), 16) * Math.pow(2, (8 * k)));
                }
                if (l % 2 == 1) {
                    var b = ((g + 1) * 8) - 1;
                    if ((m >> b) & 1 == 1) {
                        m = m - Math.pow(2, (b + 1));
                    }
                }
            }
            q.min = m;
            q.minDataType = l;
            a = parseInt(this.byteArray.shift(), 16) % 256;
            infoType = parseInt(this.byteArray.shift(), 16);
            l = parseInt(this.byteArray.shift(), 16);
            m = 0;
            g;
            h = false;
            switch (l) {
                case 0:
                case 1:
                case 2:
                    g = 0;
                    break;
                case 3:
                case 4:
                    g = 1;
                    break;
                case 5:
                case 6:
                    g = 3;
                    break;
                case 7:
                case 8:
                    g = 7;
                    break;
                case 9:
                case 10:
                    g = 15;
                    break;
                case 20:
                    alert("WARNING: Data type Id not handled (single precision floating point)");
                    break;
                case 21:
                    alert("WARNING: Data type Id not handled (double precision floating point)");
                    break;
                default:
                    break;
            }
            if (l <= 10) {
                for (var k = g; k >= 0; k--) {
                    m = m + (parseInt(this.byteArray.shift(), 16) * Math.pow(2, (8 * k)));
                }
                if (l % 2 == 1) {
                    var b = ((g + 1) * 8) - 1;
                    if ((m >> b) & 1 == 1) {
                        m = m - Math.pow(2, (b + 1));
                    }
                }
            }
            q.max = m;
            q.maxDataType = l;
        }
        p.push(q);
    }
    return p;
};
classReadParameter = function (a, b, c) {
    this.menuId = a;
    this.parameterId = b;
    this.currentValue = "";
    this.dataType = "";
    this.decimalPlaces = 0;
    this.unitText = "";
    this.slotId = (c === undefined) ? 0 : c;
    this.callBack = null;
    this.writable = false;
    this.readNotAllowed = false;
    this.string = false;
    this.formatId = 0;
    this.min = 0;
    this.minDataType = 0;
    this.max = 0;
    this.maxDataType = 0;
    this.infoTypeID = 0;
    this.accessStatus = 0;
    this.forceUpdate = false;
};
classReadParameter.prototype.updateValue = function (a) {
    this.currentValue = a;
    if (this.successCallBack != null) {
        this.successCallBack(this);
    }
};
classWriteParameter = function (c, d, e, f, a, b) {
    this.menuId = c;
    this.parameterId = d;
    this.currentValue = "";
    this.decimalPlaces = 0;
    this.slotId = (e === undefined) ? 0 : e;
    this.callBack = null;
    this.dataType = a;
    this.decimalPlaces = b;
    this.value = f;
    this.accessStatus = 0;
};

function classRequestWrite() {
    classRequest.call(this);
    this.addressingScheme = 1;
    this.requestCode = REQUEST_WRITE;
    this.parameterList = new Array();
    this.returnedParameters = new Array();
    this.returnCount = 0;
}
classRequestWrite.prototype = new classRequest();
classRequestWrite.prototype.buildMessage = function () {
    classRequest.prototype.buildMessage.call(this);
    this.messageString += this.decimalToHex(this.addressingScheme);
    this.messageString += this.getWriteParameterList();
    return this.messageString;
};
classRequestWrite.prototype.getWriteParameterList = function () {
    var e = this.decimalToHex(this.parameterList.length);
    for (var g = 0; g < this.parameterList.length; g++) {
        switch (this.addressingScheme) {
            case 0:
                e += this.multiByteToHex(this.parameterList[g].menuId, 2) + this.multiByteToHex(this.parameterList[g].parameterId, 2);
                break;
            case 1:
                e += this.decimalToHex(this.parameterList[g].slotId);
                e += this.multiByteToHex(this.parameterList[g].menuId, 2) + this.multiByteToHex(this.parameterList[g].parameterId, 2);
                break;
        }
        e += this.decimalToHex(this.parameterList[g].dataType);
        if (this.parameterList[g].dataType == 0 || this.parameterList[g].dataType > 22) {
            this.parameterList[g].decimalPlaces = 255;
        }
        e += this.decimalToHex(this.parameterList[g].decimalPlaces);
        var f = ctdatabase.getValueSize(this.parameterList[g].dataType);
        var c = new Array();
        for (var b = 0; b < f; b++) {
            c[b] = (this.parameterList[g].value >> (8 * b)) % 256;
        }
        for (var d = f; d > 0; d--) {
            e += this.decimalToHex(c[d - 1]);
        }
    }
    return e;
};
classRequestWrite.prototype.addParameter = function (c, e, f, a, b, g) {
    var d = new classWriteParameter(c, e, f, a, b, g);
    this.parameterList.push(d);
};
classRequestWrite.prototype.handleSuccess = function (a, e, b) {
    classRequest.prototype.handleSuccess.call(this.callingReference, a, e, b);
    switch (this.callingReference.requestStatus) {
        case 0:
            this.callingReference.byteArray.shift();
            var d = parseInt(this.callingReference.byteArray.shift(), 16);
            this.callingReference.byteArray.shift();
            switch (d) {
                case 146:
                    this.callingReference.returnCount = parseInt(this.callingReference.byteArray.shift(), 16);
                    var c = 0;
                    if (this.callingReference.byteArray.length > 0) {
                        while (this.callingReference.byteArray.length > 0) {
                            c = parseInt(this.callingReference.byteArray.shift(), 16);
                            this.callingReference.accessStatus = c;
                        }
                    }
                    if (this.callingReference.successCallBack != null) {
                        this.callingReference.successCallBack(this.callingReference);
                    }
                    break;
            }
            break;
        default:
            break;
    }
    this.callingReference.terminate();
};
Number.prototype.padLeft = function (b, a) {
    if (!a) {
        a = " ";
    }
    if (("" + this).length >= b) {
        return "" + this;
    } else {
        return arguments.callee.call(a + this, b, a);
    }
};

function UserLogout() {
    $("#userInfo").text(getTT("Signing out...", "tt_83"));
    $.ajax({
        url: "/logout",
        cache: false,
        async: true,
        type: "POST",
        data: "logout=" + $.cookie("token"),
        dataType: "text",
        complete: function () {
            $.removeCookie("token", {
                path: "/"
            });
            $.removeCookie("user", {
                path: "/"
            });
            localStorage.removeItem("user");
            $("body").css("cursor", "progress");
            window.location.assign("/index.html");
        },
        timeout: function () {
            $.removeCookie("token", {
                path: "/"
            });
            $.removeCookie("user", {
                path: "/"
            });
            localStorage.removeItem("user");
            $("body").css("cursor", "progress");
            window.location.assign("/index.html");
        }
    });
}

function AddProgress(a) {
    if (a.count < a.total) {
        a.count++;
        $("#progressBar").animate({
            width: Math.round((a.count / a.total) * 100) + "%"
        }, 500);
    }
}

function getTT(b, c) {
    if (mLanguage == null) {
        return b;
    } else {
        var a = parseInt(c.replace("tt_", ""));
        return ctdatabase.getCaption(a);
    }
}
var oRequestQueue = new classRequestQueue();
var namePartsReceived = 0;
var nameParts = new Array();
var receivedNamePart = function (c) {
    namePartsReceived++;
    if (c.parameterId == 27) {
        mProductFamily = ((c.currentValue >> 9) & 31) - 2;
        mProductModel = (c.currentValue >> 4) & 31;
        if (typeof LoadMenus === "function") {
            LoadMenus();
        }
        $("#modelNumber").text(ctdatabase.getDeviceName(mProductFamily + "." + mProductModel));
    } else {
        nameParts[(c.parameterId - 79)] = "";
        bytes = getIntBytes(c.currentValue, 4);
        for (var a = 0; a < bytes.length; a++) {
            if (bytes[a] != 0) {
                nameParts[(c.parameterId - 79)] += String.fromCharCode(bytes[a]);
            }
        }
    }
    if (namePartsReceived == 5) {
        var b = nameParts[0] + nameParts[1] + nameParts[2] + nameParts[3];
        if (b == "") {
            b = "Unnamed";
            document.title = $("#modelNumber").text();
        } else {
            document.title = b + " - " + $("#modelNumber").text();
        }
        $("#driveName").text(b);
        $("#driveInfo").fadeIn();
    }
};
if ($("#driveInfo").length) {
    var oDriveInfoReadRequest = new classRequestReadWithType();
    oDriveInfoReadRequest.addParameter(11, 53, 0, function (a) {
        $("#msSerial").text(a.currentValue);
    });
    oDriveInfoReadRequest.addParameter(11, 52, 0, function (a) {
        $("#lsSerial").text(a.currentValue.padLeft(9, "0"));
    });
    oDriveInfoReadRequest.addParameter(11, 29, 0, function (b) {
        var c = b.currentValue.padLeft(8, "0");
        var a = c.substring(0, 2) + "." + c.substring(2, 4) + "." + c.substring(4, 6) + "." + c.substring(6);
        $("#firmwareVersion").text(a);
    });
    oDriveInfoReadRequest.addParameter(11, 79, 0, receivedNamePart);
    oDriveInfoReadRequest.addParameter(11, 80, 0, receivedNamePart);
    oDriveInfoReadRequest.addParameter(11, 81, 0, receivedNamePart);
    oDriveInfoReadRequest.addParameter(11, 82, 0, receivedNamePart);
    oDriveInfoReadRequest.addParameter(2, 27, 100, receivedNamePart);
    oRequestQueue.addRequest(oDriveInfoReadRequest);
}
$(function () {
    if ($.cookie("token") !== null) {
        var a = null;
        if (typeof localStorage !== "undefined") {
            a = localStorage.getItem("user");
        } else {
            a = $.cookie("user");
        }
        if (a !== null) {
            $("#userInfo").text(getTT("Signed in as", "tt_81") + " " + a);
            $("#loginMenu").html('<a href="#" onclick="UserLogout()">' + getTT("SIGN OUT", "tt_80") + "</a>");
        }
    }
    $(document).ajaxSend(function () {
        $("#ajax").css("background-color", "gray").show();
    }).ajaxSuccess(function () {
        $("#ajax").css("background-color", "green");
    }).ajaxError(function () {
        $("#ajax").css("background-color", "red");
    }).ajaxComplete(function () {
        $("#ajax").fadeOut("slow");
    });
    if (typeof delayStateCheck === "undefined") {
        oRequestQueue.stateCheck();
    }
});
(function (a, d, g) {
    var e = /\+/g;

    function f(h) {
        return h;
    }

    function c(h) {
        return decodeURIComponent(h.replace(e, " "));
    }
    var b = a.cookie = function (n, r, o) {
        if (r !== g) {
            o = a.extend({}, b.defaults, o);
            if (r === null) {
                o.expires = -1;
            }
            if (typeof o.expires === "number") {
                var k = o.expires,
                    q = o.expires = new Date();
                q.setDate(q.getDate() + k);
            }
            r = b.json ? JSON.stringify(r) : String(r);
            return (d.cookie = [encodeURIComponent(n), "=", b.raw ? r : encodeURIComponent(r), o.expires ? "; expires=" + o.expires.toUTCString() : "", o.path ? "; path=" + o.path : "", o.domain ? "; domain=" + o.domain : "", o.secure ? "; secure" : ""].join(""));
        }
        var l = b.raw ? f : c;
        var j = d.cookie.split("; ");
        for (var m = 0, p;
            (p = j[m] && j[m].split("=")); m++) {
            if (l(p.shift()) === n) {
                var h = l(p.join("="));
                return b.json ? JSON.parse(h) : h;
            }
        }
        return null;
    };
    b.defaults = {};
    a.removeCookie = function (h, i) {
        if (a.cookie(h) !== null) {
            a.cookie(h, null, i);
            return true;
        }
        return false;
    };
})(jQuery, document);
(function (a) {
    var b = null;
    var d = null;
    var c = -1;
    if (!window.ctdatabase) {
        window.ctdatabase = {
            loadData: function (e) {
                a.ajax({
                    url: "/js/ct_database.min.json",
                    async: true,
                    dataType: "json",
                    success: function (f) {
                        b = f;
                    },
                    error: function (f, g) {
                        a("#notificationArea").text("Database error, please refresh. " + g).show();
                    },
                    complete: function (f, g) {
                        e();
                    },
                    timeout: 30000,
                    cache: true
                });
            },
            getMnemonic: function (q, h) {
                var l, p, k, n, g;
                g = ctdatabase.setSignedValue(q, 1);
                l = q.menuId;
                p = q.parameterId;
                if (d !== null) {
                    if (d.menu !== undefined) {
                        for (var j = 0; j < d.menu.length; j++) {
                            if (d.menu[j].n == l) {
                                k = d.menu[j];
                                break;
                            }
                        }
                        if (k !== undefined) {
                            if (k.mnemonic != null) {
                                if (k.mnemonic.hasOwnProperty(p)) {
                                    n = k.mnemonic[p];
                                    if (b.Devices.mnemonic !== undefined) {
                                        if (b.Devices.mnemonic.hasOwnProperty(n)) {
                                            var m = b.Devices.mnemonic[n];
                                            var o, f;
                                            if (h) {
                                                a("#writeSelect").empty();
                                                for (var e = 0; e < m.v.length; e++) {
                                                    if (m.v[e] <= q.max && m.v[e] >= q.min) {
                                                        o = '<option value="' + m.v[e] + '">' + ctdatabase.getCaption(m.c[e]) + "</option>";
                                                        a("#writeSelect").append(o);
                                                        if (m.v[e] == g) {
                                                            f = m.c[e];
                                                            a("#writeSelect").val(String(m.v[e]));
                                                        }
                                                    }
                                                }
                                            } else {
                                                for (var e = 0; e < m.v.length; e++) {
                                                    if (m.v[e] == g) {
                                                        f = m.c[e];
                                                    }
                                                }
                                            }
                                            return (ctdatabase.getCaption(f));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return null;
            },
            getCaption: function (e) {
                if (b.Devices.en != null) {
                    return b.Devices.en[e];
                }
                return "";
            },
            getDeviceName: function (f) {
                var g = {
                    "0.7": "CSD100",
                    "0.8": "Elevator E200",
                    "0.9": "Elevator E300",
                    "0.0": "GT Base",
                    "0.10": "HVAC Drive H300",
                    "0.11": "Powerdrive F300",
                    "0.12": "Unidrive HS70",
                    "0.13": "Unidrive HS71",
                    "0.14": "Unidrive HS72",
                    "0.5": "Unidrive M600",
                    "0.1": "Unidrive M700",
                    "0.2": "Unidrive M701",
                    "0.3": "Unidrive M702",
                    "1.0": "OL Base",
                    "1.7": "Unidrive HS30",
                    "1.3": "Unidrive M200",
                    "1.4": "Unidrive M201",
                    "1.5": "Unidrive M300",
                    "1.6": "Unidrive M400",
                    "430": "Ethernet",
                    "448": "SI-CANopen",
                    "447": "SI-DeviceNet",
                    "431": "SI-EtherCAT",
                    "433": "SI-Ethernet",
                    "443": "SI-PROFIBUS",
                    "432": "SI-PROFINET RT",
                    "434": "SI-PROFINET RT V2",
                    "311": "MCi200",
                    "310": "MCi210",
                    "304": "SI-Applications Plus",
                    "105": "SI-Encoder",
                    "106": "SI-Universal Encoder",
                    "209": "SI-IO"
                };
                var e = "Device not listed (" + f + ")";
                if (g !== null) {
                    if (g[f] !== undefined) {
                        e = g[f];
                    }
                }
                return e;
            },
            setModeDependentUnits: function (f, e, h) {
                if (h.substr(0, 13) == "modeDependent") {
                    var g = parseInt(h.substr(13));
                    switch (g) {
                        case 8:
                            switch (f) {
                                case 1:
                                    h = "Revs";
                                    break;
                                case 2:
                                case 3:
                                    if (e) {
                                        h = "mm";
                                    } else {
                                        h = "Revs";
                                    }
                                    break;
                                default:
                                    h = "";
                                    break;
                            }
                            break;
                        case 15:
                            switch (f) {
                                case 1:
                                    h = "Hz";
                                    break;
                                case 2:
                                case 3:
                                    if (e) {
                                        h = "mm/s";
                                    } else {
                                        h = "rpm";
                                    }
                                    break;
                                default:
                                    h = "";
                                    break;
                            }
                            break;
                        case 16:
                            switch (f) {
                                case 1:
                                    h = "rpm";
                                    break;
                                case 2:
                                case 3:
                                    if (e) {
                                        h = "mm/s";
                                    } else {
                                        h = "rpm";
                                    }
                                    break;
                                default:
                                    h = "";
                                    break;
                            }
                            break;
                        case 27:
                            switch (f) {
                                case 1:
                                    h = "s²/100Hz";
                                    break;
                                case 2:
                                case 3:
                                    if (e) {
                                        h = "s²/1000mm/s";
                                    } else {
                                        h = "s²/1000rpm";
                                    }
                                    break;
                                default:
                                    h = "";
                                    break;
                            }
                            break;
                        case 28:
                            switch (f) {
                                case 1:
                                    h = "s²/1000rpm";
                                    break;
                                case 2:
                                case 3:
                                    if (e) {
                                        h = "s²/1000mm/s";
                                    } else {
                                        h = "s²/1000rpm";
                                    }
                                    break;
                                default:
                                    h = "";
                                    break;
                            }
                            break;
                        default:
                            h = "";
                            break;
                    }
                }
                return h;
            },
            setSignedValue: function (h, e) {
                var j = h.currentValue;
                var g = 0;
                var f = 0;
                switch (h.dataType) {
                    case 1:
                        g = 8;
                        f = 255;
                        break;
                    case 3:
                        g = 16;
                        f = 65535;
                        break;
                    case 5:
                        g = 32;
                        f = 4294967295;
                        break;
                    case 7:
                        g = 64;
                        f = 1.844674407370955e+19;
                        break;
                    case 9:
                        g = 128;
                        f = 3.402823669209385e+38;
                        break;
                    default:
                        g = 0;
                        break;
                }
                g = parseInt(g);
                if (g != 0 && ((j) >> (g - 1)) & 1 == 1) {
                    if (e == 1) {
                        j = j - Math.pow(2, g);
                    } else {
                        if (e == 2) {
                            var i = ctdatabase.decimalToHexString(j, f);
                            j = parseInt(i, 16);
                        }
                    }
                }
                return j;
            },
            setFormatValue: function (j) {
                var l = j.currentValue;
                var k;
                switch (j.dataType) {
                    case 0:
                        switch (l) {
                            case 0:
                                k = getTT("Off", "tt_61");
                                break;
                            case 1:
                                k = getTT("On", "tt_62");
                                break;
                            default:
                                k = getTT("Unknown Boolean Value", "tt_92");
                        }
                        break;
                    default:
                        switch (j.formatId) {
                            case 0:
                                l = ctdatabase.setSignedValue(j, 1);
                                if (j.decimalPlaces != 0) {
                                    k = (l / Math.pow(10, j.decimalPlaces)).toFixed(j.decimalPlaces);
                                } else {
                                    k = l;
                                }
                                break;
                            case 1:
                                l = l.toString();
                                for (var h = l.length; h < 6; h++) {
                                    l = "0" + l;
                                }
                                k = l.substr(0, 2) + "-" + l.substr(2, 2) + "-" + l.substr(4, 2);
                                break;
                            case 2:
                                l = l.toString();
                                for (var h = l.length; h < 6; h++) {
                                    l = "0" + l;
                                }
                                k = l.substr(0, 2) + ":" + l.substr(2, 2) + ":" + l.substr(4, 2);
                                break;
                            case 3:
                                k = "";
                                var f;
                                var g = l.toString(16);
                                l = new Array();
                                for (var h = 0; h < 4; h += 1) {
                                    l[h] = parseInt(g.substr((h * 2), 2), 16);
                                    k += String.fromCharCode(l[h]);
                                }
                                break;
                            case 4:
                                k = l.toString(2);
                                for (var h = k.length; h < j.max.toString(2).length; h++) {
                                    k = "0" + k;
                                }
                                break;
                            case 5:
                                k = "";
                                var f = new Array();
                                f[0] = (l >> 24) & 255;
                                f[1] = (l >> 16) & 255;
                                f[2] = (l >> 8) & 255;
                                f[3] = (l) & 255;
                                for (var e = 0; e < 4; e++) {
                                    f[e] = f[e].toString();
                                    for (var h = f[e].length; h < 3; h++) {
                                        f[e] = "0" + f[e];
                                    }
                                    k = k + f[e];
                                    if (e != 3) {
                                        k = k + ".";
                                    }
                                }
                                break;
                            case 6:
                                var g = l.toString(16);
                                l = "";
                                for (var h = 0; h < g.length; h += 2) {
                                    l += (g.substr(h, 2).toString()).toUpperCase();
                                }
                                for (var h = l.length; h < 12; h++) {
                                    l = "0" + l;
                                }
                                k = l.substr(0, 2) + ":" + l.substr(2, 2) + ":" + l.substr(4, 2) + ":" + l.substr(6, 2) + ":" + l.substr(8, 2) + ":" + l.substr(10, 2);
                                break;
                            case 7:
                                l = l.toString();
                                for (var h = l.length; h < 8; h++) {
                                    l = "0" + l;
                                }
                                k = l.substr(0, 2) + "." + l.substr(2, 2) + "." + l.substr(4, 2) + "." + l.substr(6, 2);
                                break;
                            case 8:
                                l = l.toString();
                                for (var h = l.length; h < 6; h++) {
                                    l = "0" + l;
                                }
                                k = l.substr(0, 1) + "." + l.substr(1, 2) + "." + l.substr(3, 3);
                                break;
                            case 9:
                                l = l.toString();
                                k = "00" + l;
                                break;
                            default:
                                k = l;
                                break;
                        }
                        break;
                }
                return k;
            },
            setWriteFormat: function (h, r, s, t) {
                var j = 255;
                var o;
                var k = h.formatId;
                h.currentValue = r;
                switch (k) {
                    case 0:
                    case 9:
                        var q = new String();
                        q = r.toString();
                        if (q.indexOf(".") > -1) {
                            j = q.length - q.indexOf(".") - 1;
                        } else {
                            j = 0;
                        }
                        h.currentValue = r * (Math.pow(10, j));
                        o = ctdatabase.setSignedValue(h, 2);
                        break;
                    case 1:
                    case 2:
                        o = (r * 10000) + (s * 100) + (t * 1);
                        break;
                    case 3:
                        var g = new Array();
                        o = "";
                        for (var m = 0; m < r.length; m++) {
                            g[m] = r.substr(m, 1);
                            g[m] = g[m].charCodeAt(0);
                            g[m] = g[m].toString(16);
                            o += g[m];
                        }
                        o = parseInt(o, 16);
                        break;
                    case 4:
                        var n = false;
                        r = r.toString();
                        for (var m = 0; m < r.length; m++) {
                            if (r.substr(m, 1) != "0" && r.substr(m, 1) != "1") {
                                n = true;
                                o = 0;
                                break;
                            }
                        }
                        if (!n) {
                            r = r.toString(2);
                            o = parseInt(r, 2);
                        }
                        break;
                    case 5:
                        var p = "";
                        var l = new Array();
                        var f = r.split(".");
                        for (var e = 0; e < f.length; e++) {
                            l[e] = (parseInt(f[e], 10));
                            l[e] = (ctdatabase.decimalToHexString(l[e])).toString();
                            for (var m = l[e].length; m < 2; m++) {
                                l[e] = "0" + l[e];
                            }
                            p = p + l[e];
                        }
                        o = parseInt(p, 16);
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        o = (r * 100000) + (s * 1000) + (t * 1);
                        break;
                    default:
                        break;
                }
                h.currentValue = o;
                h.decimalPlaces = j;
                return h;
            },
            isValidInput: function (g, o, p, q) {
                var n = 10;
                var h = g.formatId;
                g.currentValue = o;
                var r = new Array(o, p, q);
                switch (h) {
                    case 0:
                    case 9:
                        if (isNaN(o)) {
                            n = 11;
                        }
                        break;
                    case 1:
                        for (var e = 0; e < 3; e++) {
                            var m = r[e].toString();
                            if (m.length > 2) {
                                n = 12;
                            }
                            for (var k = 0; k < m.length; k++) {
                                if (isNaN(m.substr(k, 1)) || r[e] < 0) {
                                    n = 13;
                                    break;
                                }
                            }
                        }
                        if (n == 10) {
                            if (o > 31 || p > 12 || q > 99) {
                                n = 14;
                            }
                        }
                        break;
                    case 2:
                        for (var e = 0; e < 3; e++) {
                            var m = r[e].toString();
                            for (var k = 0; k < m.length; k++) {
                                if (isNaN(m.substr(k, 1)) || r[e] < 0) {
                                    n = 16;
                                    break;
                                }
                            }
                        }
                        if (n == 10) {
                            if (o > 23 || p > 59 || q > 59) {
                                n = 17;
                            }
                        }
                        break;
                    case 3:
                        if (o.length > ctdatabase.getValueSize(g.dataType)) {
                            n = 18;
                        }
                        break;
                    case 4:
                        o = o.toString();
                        if (o.length > (ctdatabase.getValueSize(g.dataType) * 8)) {
                            n = 19;
                            break;
                        }
                        for (var k = 0; k < o.length; k++) {
                            if (o.substr(k, 1) != "0" && o.substr(k, 1) != "1") {
                                n = 20;
                                break;
                            }
                        }
                        break;
                    case 5:
                        var l = "";
                        var j = new Array();
                        var f = o.split(".");
                        if (f.length != 4) {
                            n = 21;
                        }
                        for (var e = 0; e < f.length; e++) {
                            var m = f[e].toString();
                            if (m.length > 3 || m.length < 1) {
                                n = 21;
                            }
                            for (var k = 0; k < m.length; k++) {
                                if (isNaN(m.substr(k, 1)) || f[e] < 0 || f[e] > 255) {
                                    n = 23;
                                    break;
                                }
                            }
                        }
                        break;
                    case 8:
                        for (var e = 0; e < 3; e++) {
                            var m = r[e].toString();
                            for (var k = 0; k < m.length; k++) {
                                if (isNaN(m.substr(k, 1)) || r[e] < 0) {
                                    n = 24;
                                    break;
                                }
                            }
                        }
                        if (n == 10) {
                            if (o > 4 || p > 99 || q > 999) {
                                n = 25;
                            }
                        }
                        break;
                    default:
                        break;
                }
                return n;
            },
            getValueSize: function (e) {
                switch (e) {
                    case 0:
                    case 1:
                    case 2:
                        return 1;
                        break;
                    case 3:
                    case 4:
                    case 30:
                        return 2;
                        break;
                    case 5:
                    case 6:
                    case 20:
                        return 4;
                        break;
                    case 7:
                    case 8:
                    case 21:
                        return 8;
                        break;
                    case 9:
                    case 10:
                        return 16;
                        break;
                    default:
                        return 0;
                        break;
                }
            },
            getPath: function (g) {
                if (b !== null) {
                    var f = b.Devices;
                    for (var e = 0; e < f.Product.length; e++) {
                        if (f.Product[e].id == g) {
                            f = f.Product[e];
                            break;
                        }
                    }
                    d = f;
                }
            },
            decimalToHexString: function (f, e) {
                if (f < 0) {
                    f = e + parseInt(f, 10) + 1;
                }
                return f.toString(16).toUpperCase();
            },
            populateMenuDropDown: function (e, h) {
                var l = new classRequestReadWithType();
                a("#selectMenu").empty();
                if (d !== null) {
                    if (d.menu != null) {
                        for (var g = 0; g < d.menu.length; g++) {
                            var f = d.menu[g];
                            var k = false;
                            if (a("#selectSlot").find("option:selected").attr("id") != "optionSlot0" && a("#selectSlot").val() != undefined) {
                                k = true;
                            } else {
                                if (f !== undefined) {
                                    if (f.Modes === undefined) {
                                        if (f.slot !== undefined) {
                                            a("#selectMenu").append('<option value="-' + f.n + '" data-slot="' + f.slot + '" disabled>' + f.n + " - Checking slot " + f.slot + "...</option>");
                                            l.addParameter(f.n, 1, 0, function (m) {
                                                if (m.currentValue == 0) {
                                                    a('#selectMenu option[value="-' + m.menuId + '"]').text(m.menuId + " - Option Not Fitted");
                                                } else {
                                                    a('#selectMenu option[value="-' + m.menuId + '"]').text(m.menuId + " - " + ctdatabase.getDeviceName(m.currentValue)).val(-m.currentValue).prop("disabled", false);
                                                }
                                            });
                                        }
                                    } else {
                                        if (f.Modes.Mode.length !== undefined) {
                                            for (var j = 0; j < f.Modes.Mode.length; j++) {
                                                var i = f.Modes.Mode[j];
                                                if (i.n == e) {
                                                    k = true;
                                                    break;
                                                }
                                            }
                                        } else {
                                            if (f.Modes.Mode.n == e) {
                                                k = true;
                                            }
                                        }
                                    }
                                }
                            }
                            if (k) {
                                a("#selectMenu").append('<option value="' + f.n + '">' + f.n + " - " + ctdatabase.getCaption(f.c) + "</option>");
                            }
                        }
                    }
                }
                if (h !== undefined) {
                    a("#selectMenu").val(h);
                } else {
                    h = a("#selectMenu").val();
                }
                oRequestQueue.addRequest(l);
                ctdatabase.populateParameterDropDown(e, h);
            },
            populateParameterDropDown: function (e, h) {
                c = -1;
                if (e === undefined) {
                    e = 1;
                }
                if (h === undefined) {
                    return;
                }
                a("#selectParameter").empty();
                var n = false;
                if (a("#selectSlot").find("option:selected").attr("id") != "optionSlot0" && a("#selectSlot").val() != undefined) {
                    n = true;
                }
                var o = false;
                if (parseInt(h) < 0) {
                    o = true;
                    n = true;
                    ctdatabase.getPath(-parseInt(h));
                    h = 0;
                }
                if (d !== null) {
                    for (var g = 0; g < d.menu.length; g++) {
                        var f = d.menu[g];
                        if (f.n == h) {
                            var j = n;
                            if (f.Modes !== undefined) {
                                if (f.Modes.Mode.length !== undefined) {
                                    for (var i = 0; i < f.Modes.Mode.length; i++) {
                                        mode = f.Modes.Mode[i];
                                        if (mode.n == e) {
                                            j = true;
                                            break;
                                        }
                                    }
                                } else {
                                    if (f.Modes.Mode.n == e) {
                                        j = true;
                                    }
                                }
                            }
                            if (j && f.p !== undefined) {
                                c = g;
                                for (var l = 0; l < f.p.length; l++) {
                                    var k = f.p[l];
                                    if (k !== undefined && k != 0) {
                                        var m = ctdatabase.getCaption(k);
                                        if (m == "") {
                                            m = getTT("Parameter mm.", "tt_65") + l;
                                        }
                                        a("#selectParameter").append('<option value="' + l + '">' + l + " - " + m + "</option>");
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                if (o) {
                    ctdatabase.getPath(a("#selectSlot").val());
                }
                a("#selectParameter").prop("selectedIndex", 0);
            },
            getParameterCaption: function (g, l, j) {
                if (d !== null) {
                    for (var f = 0; f < d.menu.length; f++) {
                        var e = d.menu[f];
                        if (e.n == g) {
                            var i = false;
                            if (e.Modes !== undefined) {
                                if (e.Modes.Mode.length !== undefined) {
                                    for (var h = 0; h < e.Modes.Mode.length; h++) {
                                        mode = e.Modes.Mode[h];
                                        if (mode.n == j) {
                                            i = true;
                                            break;
                                        }
                                    }
                                } else {
                                    if (e.Modes.Mode.n == j) {
                                        i = true;
                                    }
                                }
                            }
                            if (i && e.p !== undefined) {
                                var k = e.p[l];
                                if (k !== undefined && k != 0) {
                                    var m = ctdatabase.getCaption(k);
                                    if (m == "") {
                                        m = "#" + g.padLeft(2, "0") + "." + l.padLeft(3, "0");
                                    }
                                    return m;
                                }
                            }
                        }
                    }
                }
                return "#" + g.padLeft(2, "0") + "." + l.padLeft(3, "0");
            },
            getErrorMessage: function (i, e, g, f) {
                var h = "";
                switch (i) {
                    case 0:
                        h = getTT("The value was read or written successfully", "tt_71");
                        break;
                    case 9:
                        h = getTT("Error - The value could not be written to the drive", "tt_51");
                        break;
                    case 11:
                        h = getTT("Error - Value must be a number", "tt_35");
                        break;
                    case 12:
                        h = getTT("Error - The date format must be xx-xx-xx", "tt_20");
                        break;
                    case 13:
                        h = getTT("Error - The day, month and year must be positive integers", "tt_21");
                        break;
                    case 14:
                        h = getTT("Error - Date value out of range", "tt_22");
                        break;
                    case 16:
                        h = getTT("Error - The time values must be positive integers", "tt_40");
                        break;
                    case 17:
                        h = getTT("Error - Time value out of range", "tt_41");
                        break;
                    case 18:
                        h = getTT("Error - The input value has too many characters. Max characters:", "tt_33") + " " + ctdatabase.getValueSize(f);
                        break;
                    case 19:
                        h = getTT("Error - The input value has too many bits. Max bits:", "tt_32") + " " + ctdatabase.getValueSize(f) * 8;
                        break;
                    case 20:
                        h = getTT("Error - The value must be represented in bits (ie. only 0s and 1s)", "tt_18");
                        break;
                    case 21:
                        h = getTT("Error - The format is incorrect. IP addresses must be in the format xxx.xxx.xxx.xxx", "tt_30");
                        break;
                    case 23:
                        h = getTT("Error - Input is out of range. Each value between the dots must be a positive integer (from 0 to 255)", "tt_31");
                        break;
                    case 24:
                        h = getTT("Error - The values must be positive integer", "tt_37");
                        break;
                    case 25:
                        h = getTT("Error - Input value out of range. Format: Slot.Menu.Param (x.xx.xxx)", "tt_36");
                        break;
                    case 130:
                        h = getTT("The value was written successfully; however it clamped to fit a limit.", "tt_19");
                        break;
                    case 243:
                        h = getTT("Error - The decimal place information is invalid (i.e. out of range of allowed values)", "tt_23");
                        break;
                    case 246:
                        if (e.formatId == 4) {
                            h = getTT("Error - Input out of range. Value must be between:", "tt_17") + " " + (e.min.toString(2)) + " and " + (e.max.toString(2));
                        } else {
                            h = getTT("Error - Input out of range. Value must be between:", "tt_17") + " " + (e.min / Math.pow(10, g)) + " and " + (e.max / Math.pow(10, g));
                        }
                        break;
                    case 247:
                        h = getTT("Error - The data could not be written as the destination does not allow write access", "tt_15");
                        break;
                    default:
                        h = getTT("Error - The value could not be written, reason unknown", "tt_44");
                }
                return h;
            }
        };
    }
})(jQuery);