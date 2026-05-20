# LSB Steganography

This program implements a basic steganography system using the Least Significant Bit (LSB) technique to hide and extract messages from images.



The input message is first converted into ASCII byte representation. Each bit of the encoded message is then embedded into the least significant bit of the red channel (R) of the RGB values of image pixels. This allows the message to be hidden with minimal visible changes to the original image.



Tech: C#

Purpose: Educational project for understanding steganography and data hiding techniques

