﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace MyGame
{
	public class Sprite
	{
		public Texture2D Tex{get;protected set;}
		public Vector2 Pos{get;set;}
		
		protected int width,height,frameWidth,frameHeight,offset;
		private int currentFrame;
		protected int CurrentRow{get;set;}
		protected double FrameRate{get;set;}
		private double prevTime;
		private int[] rowFrames;
		private string asset;
		private Rectangle drawRegion;

		public Sprite(string asset,Vector2 position,int frameWidth,int frameHeight,int offset)
		{
			this.asset=asset;
			this.Pos=position;
			this.frameWidth=frameWidth;
			this.frameHeight=frameHeight;
			this.offset=offset;
			CurrentRow=0;
			FrameRate=.1;
		}

		public Sprite(string asset,Vector2 position,Vector2 origin,int frameWidth,int frameHeight,int offset)
		{
			this.asset=asset;
			this.frameWidth=frameWidth;
			this.frameHeight=frameHeight;
			this.offset=offset;
			CurrentRow=0;
			FrameRate=.1;
			Pos=position;
		}		

		public void loadContent(ContentManager cManager)
		{
			Tex=cManager.Load<Texture2D>(asset);
			
			width=Tex.Width;
			height=Tex.Height;
			prevTime=-4.0;
			drawRegion = new Rectangle(0, 0, frameWidth, frameHeight);
			getRowFrames();
		}

		public void update(GameTime gameTime)
		{
			if (prevTime==-4.0)
			{
				prevTime=gameTime.TotalGameTime.TotalSeconds;
			}

			if (gameTime.TotalGameTime.TotalSeconds-prevTime>=FrameRate)
			{
				if (++currentFrame>=rowFrames[CurrentRow])
				{
					currentFrame=0;
				}
				prevTime=gameTime.TotalGameTime.TotalSeconds;
			}

			if (frameWidth==width && frameHeight==height)
			{
				drawRegion=new Rectangle(0,0,frameWidth,frameHeight);
			}
			else
			drawRegion=getDrawArea();
		}

		public void draw(SpriteBatch sBatch)
		{
			sBatch.Draw(Tex,Pos,drawRegion,Color.White);
		}

		//Gets the number of frames in each row
		private void getRowFrames()
		{
			int cols=width/(frameWidth+offset);
			int rows=height/(frameHeight+offset);

			rowFrames=new int[rows];
			
			//Init rowFrames array with the max number of frames possible.
			for(int i=0;i<rowFrames.Length;i++)
			{
				rowFrames[i]=cols;
			}

			Rectangle extracted;
			Color[] rawColor=new Color[frameWidth*frameHeight];

			for(int i=0;i<rows;i++)
			{
				for(int j=0;j<cols;j++)
				{
					extracted=new Rectangle((j*frameWidth)+(j*offset)+offset,(i*frameHeight)+(i*offset)+offset,frameWidth,frameHeight);
					Tex.GetData<Color>(0,extracted,rawColor,0,frameWidth*frameHeight);

					if (checkFilled(rawColor,i,j))
					{
						rowFrames[i]=j;
						break;
					}
				}
			}
		}

		//Checks if a given array of pixels are all the same color (the frame is filled).
		public bool checkFilled(Color[] frame,int row,int col)
		{
			Color first=frame[1];
			for(int i=0;i<frame.Length;i++)
			{
				if (!first.Equals(frame[i]))
				{
					return(false);
				}
			}
			Console.WriteLine("Found blank at row " + row + " and column " + col);
			return(true);
		}

		public Rectangle getDrawArea()
		{
			int x=(currentFrame*frameWidth)+(currentFrame*offset)+offset;
			int y=(CurrentRow*frameHeight)+(CurrentRow*offset)+offset;

			return(new Rectangle(x,y,frameWidth,frameHeight));
		}
	}
}