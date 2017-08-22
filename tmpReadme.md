# Gametime Automatic Ticket Reader

[![Build Status](https://travis-ci.com/gametimesf/ticket-reader.svg?token=yBkyuzfzbLzpTTsAUN9z&branch=master)](https://travis-ci.com/gametimesf/ticket-reader)
[![Docker Repository on Quay](https://quay.io/repository/gametime/ticket_reader/status?token=f5e8fae4-5b66-4e9f-b1d6-4c49f7b7f844 "Docker Repository on Quay")](https://quay.io/repository/gametime/ticket_reader)

A system for reading photos or PDFs of event tickets and extracting important
information from them:

![](https://cloud.githubusercontent.com/assets/896692/18621320/3c3cf35c-7dd6-11e6-813b-7b00e84ef12d.png)

## Basic Approach

Given a ticket to an event (as a photograph or pdf file), our goal is to
identify the strings in the ticket that represent important elements:

* Section number
* Row number
* Seat number
* Barcode
* Price
* Teams
* Venues
* Date and Time of event

First, we pass the ticket through an OCR step to convert the image into
strings and their coordinates on the page. For photographs, we do this
using Google Vision OCR. For PDF files, we just extract the strings
and their positions directly using the pdftotext tool.

The result is each document is transformed into a list of individual
strings that make it up (with varying accuracy) and the position of
each string.

Second, we need to create features for each string we found in the
document. The string alone doesn't tell us if it is an important ticket
element.

For example, the string '22' alone might be a seat number, a row number, a
section number, or nothing useful at all. We need more information than
just the string to classify it. So for each string, we calculate the
following features (in classifier/features.py):

* character count
* line count
* is string all numerals?
* is string the word "GA"?
* is string one single letter?
* is string a double_letter (i.e. AA)?
* Does the string match any of three common barcode formats?
* Does the string appear in a list of known seat, row or sections?
* Does the string have a known alpha prefix commonly used in section names (like VR, B, etc)
* How far (distance) was the string in the image from the closest string "SEAT", "SECTION", or "ROW"
* Where was the text found in the image and how large was it (in pixels)?

Those are the features we will pass into our classifer.

Third, we create a [Gradient Boosting](https://en.wikipedia.org/wiki/Gradient_boosting)
classifier. It will take in a list of features for a string and guess
it's class - i.e if it is a seat, row, section, barcode, or "none of the above".

The classes identified by the classifier are:

* `0` - String is not interesting
* `1` - Seat
* `2` - Barcode
* `3` - Row
* `4` - Section

Finally after finding all the strings in the image that are classified
as a seat, row, section or barcode, it returns one of each with the
highest likelihood of being correct.

Also, the image is passed through a barcode reader. Data read from
a valid barcode or QR code is trusted above data from the classifier.

The end result is a JSON response like this:

```json
{
    "barcode": {
        "confidence": 0.997734,
        "value": "SDF7-WR9GB7ZC"
    },
    "date": {
        "raw": "Friday, September 16, 2016 7:15PM",
        "value": "2016-09-16T19:15:00"
    },
    "price": {
        "value": "$22.00"
    },
    "row": {
        "confidence": 0.997729,
        "value": "33"
    },
    "seat": {
        "confidence": 0.997858,
        "value": "7"
    },
    "section": {
        "confidence": 0.999935,
        "value": "LB132"
    },
    "teams": {
        "value": [
            "San Francisco Giants",
            "St. Louis Cardinals"
        ]
    },
    "venue": {
        "value": "AT&T Park"
    }
}
```

## Conventions

### Document coordinates

Following Google OCR conventions, coordinates are managed as follows:

* `width` - Horizontal size of an image in pixels
* `height` - Vertical height of an image in pixels
* `x` - Pixel location in *vertical* direction, with 0 being the *top* of the image
* `y` - Pixel location in *horizontal* direction, with 0 being the *left* of the image

When passing bounding boxes, we follow CSS order conventions:

* (top, right, bottom, left)

So a bounding box of `(504, 901, 1914, 363)` means:

* Top-Left corner = (x: 504, y: 363)
* Bottom-Right corner = (x: 1914, y: 901)

### Barcode types

Some tickets contain multiple graphical barcodes - one for the ticket
itself and one or more for coupons / advertisements. More information on
barcode types is available in BARCODES.md

## Docker-based Production Setup

You can run this service as a Docker image.

```bash
$ make start
```

This runs the service in production mode on port 8000. You can test it via curl:

```bash
curl -F "pdf=@./sample-files/giants.pdf" -H "Content-Type: multipart/form-data" -XPOST localhost:8000/read_ticket
```

## Local Dev Setup

This is tested using python3, but it should be simple to make it work
with python2 if needed.

`python3` can be installed on OSX with brew on or linux with your regular
package manager.

### Dependencies

First, we need gcc in order to compile fortran code required by scipy.

```
$ brew install gcc
```

Ticket Reader depends on several python libraries like Flask (for the web service) and
sklearn, numpy, etc for the classifier. 

```
$ pip3 install -r requirements.txt
```

For processing PDF files, it also requires that 'pdftotext' is installed.
On linux:

```
$ sudo apt-get install poppler-utils
```

On OSX, you can install with homebrew:

```
$ brew install poppler
```

For reading barcodes, it requires that 'zbar' is installed.
On linux:

```
$ sudo apt-get install zbar-tools
```

On OSX, you can install with homebrew:

```
$ brew install zbar
```

This also depends on [XGBoost](https://xgboost.readthedocs.io/en/latest/build.html)
and the XGBoost Python bindings.

You can install XGBoost on Ubuntu like this:

```bash
git clone --recursive https://github.com/dmlc/xgboost
cd xgboost; make -j4
```

And the [python bindings](http://xgboost.readthedocs.io/en/latest/build.html#python-package-installation) like this:

```bash
cd python-package; sudo python3 setup.py install
```

Note that XGBoost doesn't support multi-threading by default on OSX, so
it's a lot slower on OSX. But it is possible to install it on OSX
[with multi-threading support](https://xgboost.readthedocs.io/en/latest/build.html#building-on-osx).

## Running Unit Tests

You can run the unit tests with [nosetests](http://nose.readthedocs.io/en/latest/):

```
$ python3 -m "nose"

.......
-------
Ran 7 tests in 0.561s

OK
```

## Classifier

## Re-training the classifiers (Changes in Progress)

NOTE: This is optional. Pre-trained classifiers are already `./models/`.

There are two classifiers used in ticket-reader; one for classifying pdfs, and the other for classifying images. Retraining the classifier involves performing the 4 steps listed below for the respective model. The pdfbased model is retrained in parallel due to the sheer size of ticket data we have available.

Run the following commands after logging into the ec2 instance described by the ticket-reader cloudformation stack (found here: https://console.aws.amazon.com/cloudformation/home?region=us-east-1)

Feel free to use the '-d' parameter so that the docker container runs in the background

#### Step 1: Dump known values from Redshift and generate a CSV with training data.

##### pdf_based model:
```
docker run -e PYTHONPATH=. quay.io/gametime/ticket_reader:master python3 ./utils/ticket_data_extractor.py
```
##### image_based model:
```
docker run -e PYTHONPATH=. quay.io/gametime/ticket_reader:master python3 ./utils/image_data_extractor.py
```

NOTE: pass the -o argument to image_data_extractor.py if you wish to retrain using the old data set

#### Step 2: Generate an OCR cache of all features known in the PDF.

This will take a long time at first, but the second time after most of the tickets are cached it will get quicker.

The pdfbased ocr generation will need to create a docker container while inside a docker container. To do this, we pass the host's docker.sock through the -v parameter as well as the docker binary. This prevents a 'docker container within a docker container' scenario

##### pdf_based model:
```
docker run -v /var/run/docker.sock:/var/run/docker.sock -v $(which docker):/bin/docker quay.io/gametime/ticket_reader:master ./scripts/run_ocr_cache.sh
```

##### image_based model:
```
docker run -e PYTHONPATH=. quay.io/gametime/ticket_reader:master python3 ./utils/generate_image_ocr_cache.py
```

#### Step 3: Create the training set from the OCR cache

##### pdf_based model:
```
docker run -e PYTHONPATH=. -v /var/run/docker.sock:/var/run/docker.sock -v $(which docker):/bin/docker quay.io/gametime/ticket_reader:master python3 ./scripts/multipart_training_set.py
```

##### image_based model:
```
docker run -e PYTHONPATH=. quay.io/gametime/ticket_reader:master python3 ./classifier/create_image_training_set.py
```

NOTE: pass the -o argument to create_image_training_set.py if you wish to retrain using the old data set

#### Step 4: Re-train the classifiers

##### pdf_based model:
```
docker run -e PYTHONPATH=. quay.io/gametime/ticket_reader:master python3 ./classifier/train.py
```

##### image_based model:
```
docker run -e PYTHONPATH=. quay.io/gametime/ticket_reader:master python3 ./classifier/train.py --type imagebased
```

These will take a while. The train script runs in parallel using all cpu cores, so it
will run a lot faster on a machine with multiple cores than on a
single-core instance.

The final result is written to the ./models/ folder of the docker container as well as in s3 (s3://gametime-ml-production/ticket-reader/training/{ pdfbased OR imagebased}/models/). 

Grab a copy from the finished container or s3 and you can use it for your local webservice!

## Webservice

### Running

```
GOOGLE_APPLICATION_CREDENTIALS=./googleauth.json PYTHONPATH=. python3 webservice/app.py
```

Service will be running on http://127.0.0.1:8000/

### Swagger 2.0 API Docs

This service is supports Swagger 2.0. You can access the Swagger json
data by hitting the /api/swagger.json path.

If you're running locally in debug mode, that would be:

http://127.0.0.1:8000/api/swagger.json

### Calling the webservice

With a photo:

```
curl -F "image=@./sample-files/real-ticket-photo.jpg" -H "Content-Type: multipart/form-data" -XPOST localhost:8000/read_ticket
```

With the url to a photo:

```
curl -XPOST -d 'image_url=http://3v6x691yvn532gp2411ezrib.wpengine.netdna-cdn.com/wp-content/uploads/sites/default/files/20131212-ticket2.png' localhost:8000/read_ticket
```

With a ticket PDF:

```
curl -F "pdf=@./sample-files/giants.pdf" -H "Content-Type: multipart/form-data" -XPOST localhost:8000/read_ticket
```

With the url to a pdf:

```
curl -XPOST -d 'pdf_url=http://tic.gametime.co/57fc23eaa2e3a966f29b9f1c/337731506001161831372992819074659674636.pdf' localhost:8000/read_ticket
```

## Image Model Statistics

If you decide to make changes to the image-based model, you can easily test its new performance by doing the following:

#### Step 1:

Retrieve true data from Redshift by running the following script:

```
PYTHONPATH=. python3 utils/full_image_data_extractor.py
```

#### Step 2:

Download a copy of the previous results from s3 and store it locally. Redshift dump is located at: /gametime-ml-production/ticket-reader/training/imagebased/tickets\_(timstamp).csv.

Then run the following report script supplying a path (-p parameter) to your local copy of ticket data:

```
PYTHONPATH=. GOOGLE_APPLICATION_CREDENTIALS=googleauth.json python3 scripts/image_model_report.py -p data/tickets_20170711.csv
```

## Code Docs

You can view the code docs in a browser with pydoc3:

```
pydoc3 -b
```
